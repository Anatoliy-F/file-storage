using BuisnessLogicLayer.Enums;
using BuisnessLogicLayer.Interfaces;
using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Swashbuckle.AspNetCore.Annotations;
using System.Globalization;
using WebAPI.Filters;
using WebAPI.Models;
using WebAPI.Utilities;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Implement file uploading with streaming
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly long _filesizeLimit;
        //TODO: read extensions from appsetting
        private readonly string[] _permittedExtensions = { ".txt", ".png" };
        private static readonly FormOptions _defaultFormoptions = new();
        private readonly JwtHandler _jwtHandler;
        private readonly IFileService _fileService;

        private ILogger<UploadController> Logger { get; set; }

        /// <summary>
        /// Initialize new instance of FileController
        /// </summary>
        /// <param name="config">IConfiguration instanse, for access to application configuration</param>
        /// <param name="fileService">FileService instanse, for persist file</param>
        /// <param name="jwtHandler">JwtHandler instanse. Generate JWT. Retrive userId from JWT</param>
        /// <param name="logger">ILogger object to performing error logging</param>
        public UploadController(IConfiguration config, 
            JwtHandler jwtHandler, ILogger<UploadController> logger, IFileService fileService)
        {
            _filesizeLimit = config.GetValue<long>("FileSizeLimit");
            _jwtHandler = jwtHandler;
            Logger = logger;
            _fileService = fileService;
        }

        /// <summary>
        /// Upload file by stream without FormValue model binding disabled
        /// </summary>
        /// <returns>Added file metadata</returns>
        [HttpPost]
        [DisableFormValueModelBinding]
        [Authorize(Roles = "RegisteredUser")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(201, "The execution was successful")]
        [SwaggerResponse(400, "The request was invalid")]
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

                        using var streamReader = new StreamReader(
                            section.Body, encoding,
                            detectEncodingFromByteOrderMarks: true,
                            bufferSize: 1024,
                            leaveOpen: true);
                        // The value length limit is enforced by MultipartBodyLengthLimit
                        var value = await streamReader.ReadToEndAsync();
                        if (string.Equals(value, "undefined", StringComparison.OrdinalIgnoreCase))
                        {
                            value = string.Empty;
                        }

                        formAccumulator.Append(key, value);

                        if (formAccumulator.ValueCount > _defaultFormoptions.ValueCountLimit)
                        {
                            //Form key count limit of _defaultFormOptions.ValueCountLimit
                            // is exceeded.
                            ModelState.AddModelError("File", $"The request couldn't be processed (Error 3).");
                            Logger.LogError("Form key count limit of _defaultFormOptions.ValueCountLimit is exceeded.");
                            return BadRequest(ModelState);
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
                var ownerId = _jwtHandler.GetUserId(this.User);

                var respRes = await _fileService.AddFromScratch(untrustedFileNameForStorage,
                    formData.Note, ownerId, streamedFileContent);

                if(respRes.ResponseResult == ResponseResult.Success && respRes.Data != null)
                {
                    return CreatedAtAction(nameof(FileController.GetOwnById), "File", 
                        new { id = respRes.Data.Id }, respRes.Data);
                }
                else
                {
                    return BadRequest(respRes.ErrorMessage);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
