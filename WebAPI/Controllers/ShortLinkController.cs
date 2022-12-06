using BuisnessLogicLayer.Enums;
using BuisnessLogicLayer.Interfaces;
using BuisnessLogicLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Define endpoints to perform operations with short links
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ShortLinkController : ControllerBase
    {
        private readonly IShortLinkService _shortLinkService;

        /// <summary>
        /// Initialize new instance of ShortLinkController
        /// </summary>
        /// <param name="shortLinkService">ShortLinkService instanse</param>
        public ShortLinkController(IShortLinkService shortLinkService)
        {
            _shortLinkService = shortLinkService;
        }

        /// <summary>
        /// Download a file via short link
        /// </summary>
        /// <param name="link">Short link</param>
        /// <returns>FileContentResult</returns>
        [AllowAnonymous]
        [HttpGet("/{link:length(6)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(200, "The execution was successful")]
        [SwaggerResponse(404, "Not found by this link")]
        [SwaggerResponse(403, "Access denied, file sharing closed")]
        [SwaggerResponse(400, "The request was invalid")]
        public async Task<IActionResult> DownloadFile(string link)
        {
            var result = await _shortLinkService.GetFileByShortLinkAsync(link);

            if (result.ResponseResult == ResponseResult.Success
                && result.Data != null && result.Data.Content != null)
            {
                new FileExtensionContentTypeProvider().TryGetContentType(result.Data.Name, out string? contentType);
                return File(result.Data.Content, contentType ?? "text/plain", result.Data.Name);
            }

            return MapResponseFromBLL(result);
        }

        /// <summary>
        ///  Returns an object describing the file
        /// </summary>
        /// <param name="link">Short link</param>
        /// <returns>File metadata</returns>
        [Authorize(Roles = "RegisteredUser")]
        [HttpGet("preview/{link:length(6)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(200, "The execution was successful")]
        [SwaggerResponse(404, "Not found by this link")]
        [SwaggerResponse(403, "Access denied, file sharing closed")]
        [SwaggerResponse(400, "The request was invalid")]
        public async Task<IActionResult> GetFileData(string link)
        {
            var result = await _shortLinkService.GetShortFileDataAsync(link);
            if(result.ResponseResult == ResponseResult.Success && result.Data != null)
            {
                return Ok(result.Data);
            }

            return MapResponseFromBLL(result);
        }

        /// <summary>
        /// Create short link for file by id
        /// </summary>
        /// <param name="id">File id</param>
        /// <param name="model">File metadata object</param>
        /// <returns>Short link object</returns>
        [Authorize(Roles = "RegisteredUser")]
        [HttpPost("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(200, "The execution was successful")]
        [SwaggerResponse(404, "Not found by this link")]
        [SwaggerResponse(403, "Access denied, file sharing closed")]
        [SwaggerResponse(400, "The request was invalid")]
        public async Task<ActionResult> CreateShortlink(Guid id, [FromBody] FileDataModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            var result = await _shortLinkService.GenerateForFileByIdAsync(id);

            if (result.ResponseResult == ResponseResult.Success)
            {
                return Ok(result.Data);
            }
            else
            {
                return MapResponseFromBLL(result);
            }
        }

        /// <summary>
        /// Delete short link
        /// </summary>
        /// <param name="link">Short link</param>
        /// <param name="model">File metadata object</param>
        /// <returns>Status 200 Ok, if link delete successfuly, error code otherwise</returns>
        [Authorize(Roles = "RegisteredUser")]
        [HttpDelete("{link:length(6)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(200, "The execution was successful")]
        [SwaggerResponse(404, "Not found by this link")]
        [SwaggerResponse(400, "The request was invalid")]
        public async Task<ActionResult> DeleteShortLink(string link, [FromBody] FileDataModel model)
        {
            if(model.ShortLink != link)
            {
                return BadRequest();
            }
            var result = await _shortLinkService.DeleteLinkAsync(link);
            if (result.ResponseResult == ResponseResult.Success)
            {
                return Ok(result.Data);
            }
            else
            {
                return BadRequest(result.ErrorMessage);
            }
        }

        private ActionResult MapResponseFromBLL<T>(ServiceResponse<T> response)
        {
            return response.ResponseResult switch
            {   
                ResponseResult.Success => Ok(response.Data),
                ResponseResult.NotFound => NotFound(),
                ResponseResult.AccessDenied => Forbid(),
                ResponseResult.Error => BadRequest(response.ErrorMessage),
                _ => BadRequest()
            };
        }
    }
}
