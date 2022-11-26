using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using WebAPI.Filters;
using WebAPI.Utilities;
using Microsoft.Net.Http.Headers;
using System.Net;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.StaticFiles;

namespace WebAPI.Controllers
{
    //TODO: remove in rigth space
    public class FormData
    {
        public string Note { get; set; }
    }


    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly long _filesizeLimit;
        private readonly string[] _permittedExtensions = { ".txt", ".png" };
        // Get the default form options so that we can use them to set the default 
        // limits for request body data.
        //TODO: Should i use it???
        private static readonly FormOptions _defaultFormoptions = new FormOptions();

        //TODO: inject UoW, logger;
        private readonly IUnitOfWork _unitOfWork;

        public UploadController(IConfiguration config, IUnitOfWork unitOfWork)
        {
            _filesizeLimit = config.GetValue<long>("FileSizeLimit");
            _unitOfWork = unitOfWork;   
        }

        [HttpGet]
        public async Task<IActionResult> ShortUrl()
        {
            var shortUrl = WebEncoders.Base64UrlEncode(new Guid("7FE43A9F-DDC4-4D7D-70FA-08DACB19CE30").ToByteArray().Take(4).ToArray());
            return new JsonResult(new {Key = shortUrl});
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> DownloadFile(Guid id)
        {
            AppFileData? afd = await _unitOfWork.AppFileDataRepository
                .GetAppFileDataWithContentAsync(id);
            if(afd == null)
            {
                return BadRequest();
            }
            //"7FE43A9F-DDC4-4D7D-70FA-08DACB19CE30"
            //byte[] data = afd.AppFileNav.Content;
            new FileExtensionContentTypeProvider().TryGetContentType(afd.UnstrustedName, out string? contentType);
            return File(afd.AppFileNav?.Content, contentType, afd.UnstrustedName);

            
            

        }

        [HttpPost]
        [DisableFormValueModelBinding]
        //[ValidateAntiForgeryToken]
        [Authorize(Roles = "RegisteredUser")]
        public async Task<IActionResult> UploadDatabase()
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                ModelState.AddModelError("File", $"The request couldn't be processed (Error 1 : Wrong content type)");
                //TODO: Log error
                return BadRequest(ModelState);
            }

            // Accumulate the form data key-value pairs in the request (formAccumulator).
            var formAccumulator = new KeyValueAccumulator();
            var trustedFileNameForDisplay = string.Empty;
            var untrustedFileNameForStorage = string.Empty;
            var streamedFileContent = Array.Empty<byte>();

            var boundary = MultipartRequestHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(Request.ContentType),
                _defaultFormoptions.MultipartBoundaryLengthLimit);

            var reader = new MultipartReader(boundary, HttpContext.Request.Body);

            var section = await reader.ReadNextSectionAsync();

            while(section != null)
            {
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(
                    section.ContentDisposition, out var contentDisposition);

                if (hasContentDispositionHeader)
                {
                    if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                    {
                        untrustedFileNameForStorage = contentDisposition.FileName.Value;
                        // Don't trust the file name sent by the client. To display
                        // the file name, HTML-encode the value.
                        //TO DO: should i need it?
                        trustedFileNameForDisplay = WebUtility.HtmlEncode(contentDisposition.FileName.Value);
                        streamedFileContent = await FileHelpers.ProcessStreamedFile(
                            section, contentDisposition, ModelState, _permittedExtensions, _filesizeLimit);

                        if (!ModelState.IsValid)
                        {
                            return BadRequest(ModelState);
                        }
                    }
                    else if (MultipartRequestHelper.HasFormDataContentDisposition(contentDisposition))
                    {
                        // Don't limit the key name length because the 
                        // multipart headers length limit is already in effect.
                        var key = HeaderUtilities.RemoveQuotes(contentDisposition.Name).Value;
                        var encoding = MultipartRequestHelper.GetEncoding(section);

                        if(encoding == null)
                        {
                            ModelState.AddModelError("File", $"The request couldn't be processed (Error 2).");
                            //TODO: logError
                            return BadRequest(ModelState);
                        }

                        using(var streamReader = new StreamReader(
                            section.Body, encoding,
                            detectEncodingFromByteOrderMarks: true,
                            bufferSize: 1024,
                            leaveOpen: true))
                        {
                            // The value length limit is enforced by
                            // MultipartBodyLengthLimit
                            var value = await streamReader.ReadToEndAsync();
                            if(string.Equals(value, "undefined", StringComparison.OrdinalIgnoreCase))
                            {
                                value = string.Empty;
                            }

                            formAccumulator.Append(key, value);

                            if(formAccumulator.ValueCount > _defaultFormoptions.ValueCountLimit)
                            {
                                //Form key count limit of
                                // _defaultFormOptions.ValueCountLimit 
                                // is exceeded.
                                ModelState.AddModelError("File", $"The request couldn't be processed (Error 3).");
                                // TODO: Log error
                                return BadRequest(ModelState);
                            }
                        }
                    }
                }
                // Drain any remaining section body that hasn't been consumed and
                // read the headers for the next section.
                section = await reader.ReadNextSectionAsync();
            }

            // Bind form data to the model
            var formData = new FormData();
            var formValueProvider = new FormValueProvider(
                BindingSource.Form,
                new FormCollection(formAccumulator.GetResults()),
                CultureInfo.CurrentCulture);
            var bindingSuccessful = await TryUpdateModelAsync(formData, prefix: "", valueProvider: formValueProvider);

            if (!bindingSuccessful)
            {
                ModelState.AddModelError("File", "The request couldn't be processed (Error 5)");
                //TODO: log error
                return BadRequest(ModelState);
            }

            var file = new AppFileData()
            {
                AppFileNav = new AppFile { Content = streamedFileContent },
                UnstrustedName = untrustedFileNameForStorage,
                Note = formData.Note,
                Size = streamedFileContent.LongLength,
                UploadDT = DateTime.UtcNow,
                AppUserId = JwtHandler.GetUserId(this.User)//new Guid(this.User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Value)
            };

            await _unitOfWork.AppFileDataRepository.AddAsync(file);
            await _unitOfWork.SaveAsync();
            
            //_context.File.Add(file);
            //await _context.SaveChangesAsync();

            //TODO: implement createdAt
            //return Created(nameof(StreamingController), null);
            return Ok();
        }
    }
}
