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

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly IShortLinkService _shortLinkService;
        private readonly JwtHandler _jwtHandler;

        public FileController(IFileService fileService,
            JwtHandler jwtHandler, IShortLinkService shortLinkService)
        {
            _fileService = fileService;
            _jwtHandler = jwtHandler;
            _shortLinkService = shortLinkService;
        }

        [Authorize(Roles = "RegisteredUser")]
        [HttpGet]
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

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FileDataModel>> GetOWnById(Guid id)
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

        [HttpDelete("{id}")]
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

        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadOwn(Guid id)
        {
            try
            {
                var userId = _jwtHandler.GetUserId(this.User);
                var respRes = await _fileService.GetOwnContentAsync(userId, id);

                if (respRes.ResponseResult == ResponseResult.Success
                    && respRes.Data != null && respRes.Data.AppFileNav != null)
                {
                    new FileExtensionContentTypeProvider().TryGetContentType(respRes.Data.UntrustedName, out string? contentType);
                    return File(respRes.Data.AppFileNav.Content, contentType ?? "text/plain", respRes.Data.UntrustedName);
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

        [HttpPut("share/{email}")]
        public async Task<ActionResult> Share(string email, [FromBody] FileDataModel model)
        {
            //TODO: DELETE THIS
            //TODO: uncomment validation
            /*if(!model.Viewers.Any(fv => fv.Email == email))
            {
                return BadRequest();
            }*/

            var userId = _jwtHandler.GetUserId(this.User);
            var servResp = await _fileService.ShareByEmailAsync(userId, email, model.Id);

            if (servResp.ResponseResult == ResponseResult.Success && servResp.Data != null)
            {
                return Ok(servResp.Data);
            }

            return MapResponseFromBLL(servResp);
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
                && respRes.Data != null && respRes.Data.AppFileNav != null)
            {
                new FileExtensionContentTypeProvider().TryGetContentType(respRes.Data.UntrustedName, out string? contentType);
                return File(respRes.Data.AppFileNav.Content, contentType ?? "text/plain", respRes.Data.UntrustedName);
            }

            return MapResponseFromBLL(respRes);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("admin/{id}")]
        public async Task<ActionResult> Update(Guid Id, [FromBody] FileDataModel model)
        {
            if (Id != model.Id)
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
