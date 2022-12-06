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
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly long _filesizeLimit;
        
        //TODO: read extensions from appsetting
        private readonly string[] _permittedExtensions = { ".txt", ".png" };

        private static readonly FormOptions _defaultFormoptions = new();

        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtHandler _jwtHandler;

        public ILogger<UploadController> Logger { get; set; }

        public UploadController(IConfiguration config, IUnitOfWork unitOfWork, JwtHandler jwtHandler, ILogger<UploadController> logger)
        {
            _filesizeLimit = config.GetValue<long>("FileSizeLimit");
            _unitOfWork = unitOfWork;
            _jwtHandler = jwtHandler;
            Logger = logger;
        }

        [HttpPost]
        [DisableFormValueModelBinding]
        [Authorize(Roles = "RegisteredUser")]
        public async Task<IActionResult> UploadDatabase()
        {
            if (Request.ContentType == null || !MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                ModelState.AddModelError("File", $"The request couldn't be processed (Error 1 : Wrong content type)");
                Logger.LogError("The request couldn't be processed (Error 1 : Wrong content type)");
                return BadRequest(ModelState);
            }

            // Accumulate the form data key-value pairs in the request (formAccumulator).
            var formAccumulator = new KeyValueAccumulator();
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

                if (contentDisposition != null && hasContentDispositionHeader)
                {
                    if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                    {
                        untrustedFileNameForStorage = contentDisposition.FileName.Value;
                        streamedFileContent = await FileHelpers.ProcessStreamedFile(section, 
                            contentDisposition, ModelState, _permittedExtensions, _filesizeLimit);
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
                            Logger.LogError("The request couldn't be processed (Error 2: can't resolve encoding)");
                            return BadRequest(ModelState);
                        }

                        using(var streamReader = new StreamReader(
                            section.Body, encoding,
                            detectEncodingFromByteOrderMarks: true,
                            bufferSize: 1024,
                            leaveOpen: true))
                        {
                            // The value length limit is enforced by MultipartBodyLengthLimit
                            var value = await streamReader.ReadToEndAsync();
                            if(string.Equals(value, "undefined", StringComparison.OrdinalIgnoreCase))
                            {
                                value = string.Empty;
                            }

                            formAccumulator.Append(key, value);

                            if(formAccumulator.ValueCount > _defaultFormoptions.ValueCountLimit)
                            {
                                //Form key count limit of _defaultFormOptions.ValueCountLimit
                                // is exceeded.
                                ModelState.AddModelError("File", $"The request couldn't be processed (Error 3).");
                                Logger.LogError("Form key count limit of _defaultFormOptions.ValueCountLimit is exceeded.");
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
                ModelState.AddModelError("File", "The request couldn't be processed (Error 4)");
                Logger.LogError("Unable bind form data");
                return BadRequest(ModelState);
            }

            try
            {
                //TODO: Create method in service 

                var ownerId = _jwtHandler.GetUserId(this.User);

                var file = new AppFileData()
                {
                    AppFileNav = new AppFile { Content = streamedFileContent },
                    UntrustedName = untrustedFileNameForStorage,
                    Note = formData.Note,
                    Size = streamedFileContent.LongLength,
                    UploadDT = DateTime.UtcNow,
                    OwnerId = ownerId
                };

                await _unitOfWork.AppFileDataRepository.AddAsync(file);
                await _unitOfWork.SaveAsync();

                return Ok();
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }

            //TODO: implement createdAt
            //return Created(nameof(StreamingController), null);
        }
    }
}
