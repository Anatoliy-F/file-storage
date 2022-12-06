using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Entities;
using BuisnessLogicLayer.Models;
using DataAccessLayer.Interfaces;
using BuisnessLogicLayer.Interfaces;
using WebAPI.Utilities;
using Microsoft.AspNetCore.StaticFiles;
using BuisnessLogicLayer.Enums;
using WebAPI.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.Controllers
{   
    /// <summary>
    /// Define endpoints to perform operations with files
    /// and files metadata
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly JwtHandler _jwtHandler;

        /// <summary>
        /// Initialize new instance of FileController
        /// </summary>
        /// <param name="fileService"> FileService instanse. Provides operations with file objects.</param>
        /// <param name="jwtHandler"> JwtHandler instanse. Generate JWT. Retrive userId from JWT</param>
        public FileController(IFileService fileService,
            JwtHandler jwtHandler)
        {
            _fileService = fileService;
            _jwtHandler = jwtHandler;
        }

        /// <summary>
        /// Returns a page of sorted and filtered objects describing files uploaded by the user.
        /// Page size, query for filtering, property for sorting and sorting order are defined by QueryModel object
        /// </summary>
        /// <param name="query">QueryModel object, incapsulate query options for pagination, sorting, filtering</param>
        /// <returns>Returns filtered, sorted page of file metadata</returns>
        [Authorize(Roles = "RegisteredUser")]
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(200, "The execution was successful")]
        [SwaggerResponse(400, "The request was invalid")]
        public async Task<ActionResult<PaginationResultModel<FileDataModel>>>
            GetOwn([FromQuery] QueryModel query)
        {
            try
            {
                var userId = _jwtHandler.GetUserId(this.User);
                var respRes = await _fileService.GetAllOwnAsync(userId, query);

                if (respRes.ResponseResult == ResponseResult.Success)
                {
                    return Ok(respRes.Data);
                }

                return MapResponseFromBLL(respRes);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Returns an object describing the file owned by the user
        /// </summary>
        /// <param name="id">File Id</param>
        /// <returns>File metadata</returns>
        [Authorize(Roles = "RegisteredUser")]
        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(200, "The execution was successful")]
        [SwaggerResponse(404, "Not found file with this Id")]
        [SwaggerResponse(403, "Access denied, user does not own this file")]
        [SwaggerResponse(400, "The request was invalid")]
        public async Task<ActionResult<FileDataModel>> GetOwnById(Guid id)
        {
            try
            {
                var userId = _jwtHandler.GetUserId(this.User);
                var serResp = await _fileService.GetOwnByIdAsync(userId, id);

                if (serResp.ResponseResult == ResponseResult.Success && serResp.Data != null)
                {
                    return new JsonResult(serResp.Data);
                }

                return MapResponseFromBLL(serResp);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a file owned by the user
        /// </summary>
        /// <param name="id">File id</param>
        /// <param name="fileDataModel">File metadata object</param>
        /// <returns>Status 200 Ok, if file delete successfuly, error code otherwise</returns>
        [Authorize(Roles = "RegisteredUser")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(200, "The execution was successful")]
        [SwaggerResponse(404, "Not found file with this Id")]
        [SwaggerResponse(403, "Access denied, user does not own this file")]
        [SwaggerResponse(400, "The request was invalid")]
        public async Task<ActionResult> DeleteOwnById(Guid id, [FromBody] FileDataModel fileDataModel)
        {
            if (id != fileDataModel.Id)
            {
                return BadRequest();
            }

            try
            {
                var userId = _jwtHandler.GetUserId(this.User);
                var servResp = await _fileService.DeleteOwnAsync(userId, id);

                if (servResp.ResponseResult == ResponseResult.Success)
                {
                    return Ok();
                }
                else
                {
                    return MapResponseFromBLL(servResp);
                }

            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Download a file owned by the user
        /// </summary>
        /// <param name="id">File id</param>
        /// <returns>FileContentResult</returns>
        [HttpGet("download/{id}")]
        [Authorize(Roles = "RegisteredUser")]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(200, "The execution was successful")]
        [SwaggerResponse(404, "Not found file with this Id")]
        [SwaggerResponse(403, "Access denied, user does not own this file")]
        [SwaggerResponse(400, "The request was invalid")]
        public async Task<IActionResult> DownloadOwn(Guid id)
        {
            try
            {
                var userId = _jwtHandler.GetUserId(this.User);
                var respRes = await _fileService.GetOwnContentAsync(userId, id);

                if (respRes.ResponseResult == ResponseResult.Success
                    && respRes.Data != null && respRes.Data.Content != null)
                {
                    new FileExtensionContentTypeProvider().TryGetContentType(respRes.Data.Name, out string? contentType);
                    return File(respRes.Data.Content, contentType ?? "text/plain", respRes.Data.Name);
                }

                return MapResponseFromBLL(respRes);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateOwn(Guid Id, [FromBody] FileDataModel model)
        {
            if (Id != model.Id)
            {
                return BadRequest();
            }

            try
            {
                var userId = _jwtHandler.GetUserId(this.User);
                var respRes = await _fileService.UpdateOwnAsync(userId, model);

                if (respRes.ResponseResult == ResponseResult.Success)
                {
                    return Ok();
                }

                return MapResponseFromBLL(respRes);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "RegisteredUser")]
        [HttpGet("shared")]
        public async Task<ActionResult<PaginationResultModel<ShortFileDataModel>>> GetShared([FromQuery] QueryModel query)
        {
            try
            {
                var userId = _jwtHandler.GetUserId(this.User);
                var respRes = await _fileService.GetSharedAsync(userId, query);

                if (respRes.ResponseResult == ResponseResult.Success)
                {
                    return Ok(respRes.Data);
                }

                return MapResponseFromBLL(respRes);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "RegisteredUser")]
        [HttpGet("shared/{id}")]
        public async Task<ActionResult<FileDataModel>> GetSharedById(Guid id)
        {
            try
            {
                var userId = _jwtHandler.GetUserId(this.User);
                var serResp = await _fileService.GetSharedByIdAsync(userId, id);

                if (serResp.ResponseResult == ResponseResult.Success && serResp.Data != null)
                {
                    return new JsonResult(serResp.Data);
                }

                return MapResponseFromBLL(serResp);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "RegisteredUser")]
        [HttpDelete("shared/{id}")]
        public async Task<ActionResult> RefuseSharedById([FromRoute]Guid id, [FromBody] FileDataModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            try
            {
                var userId = _jwtHandler.GetUserId(this.User);
                var servResp = await _fileService.RefuseSharedAsync(userId, id);

                if (servResp.ResponseResult == ResponseResult.Success)
                {
                    return Ok();
                }
                else
                {
                    return MapResponseFromBLL(servResp);
                }

            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet("admin")]
        public async Task<ActionResult<PaginationResultModel<FileDataModel>>>
            Get([FromQuery] QueryModel query)
        {
            var respRes = await _fileService.GetAllAsync(query);

            if (respRes.ResponseResult == ResponseResult.Success)
            {
                return Ok(respRes.Data);
            }

            return MapResponseFromBLL(respRes);
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet("admin/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FileDataModel>> GetById(Guid id)
        {
            var serResp = await _fileService.GetByIdAsync(id);

            if (serResp.ResponseResult == ResponseResult.Success && serResp.Data != null)
            {
                return new JsonResult(serResp.Data);
            }

            return MapResponseFromBLL(serResp);
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("admin/{id}")]
        public async Task<ActionResult> Delete(Guid id, [FromBody] FileDataModel fileDataModel)
        {
            if (id != fileDataModel.Id)
            {
                return BadRequest();
            }

            var servResp = await _fileService.DeleteAsync(fileDataModel);

            if (servResp.ResponseResult == ResponseResult.Success)
            {
                return Ok();
            }
            else
            {
                return MapResponseFromBLL(servResp);
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet("admin/download/{id}")]
        public async Task<IActionResult> Download(Guid id)
        {
            var respRes = await _fileService.GetContentAsync(id);

            if (respRes.ResponseResult == ResponseResult.Success
                && respRes.Data != null && respRes.Data.Content != null)
            {
                new FileExtensionContentTypeProvider().TryGetContentType(respRes.Data.Name, out string? contentType);
                return File(respRes.Data.Content, contentType ?? "text/plain", respRes.Data.Name);
            }

            return MapResponseFromBLL(respRes);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("admin/{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] FileDataModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            var respRes = await _fileService.UpdateAsync(model);

            if (respRes.ResponseResult == ResponseResult.Success)
            {
                return Ok();
            }

            return MapResponseFromBLL(respRes);
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
