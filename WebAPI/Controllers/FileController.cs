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

        [HttpGet]
        public async Task<PaginationResultModel<FileDataModel>>
            GetFileData([FromQuery] QueryModel query)
        {
            //TODO: wrap in try-catch
            var userId = _jwtHandler.GetUserId(this.User);
            return await _fileService.GetUserFilesDataNoTrackingAsync(userId, query);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FileDataModel>> GetById(Guid id)
        {
            //TODO: wrap in try-catch
            var userId = _jwtHandler.GetUserId(this.User);
            var serResp = await _fileService.GetOwnByIdAsync(userId, id);

            if (serResp.ResponseResult == ResponseResult.Success && serResp.Data != null)
            {
                return new JsonResult(serResp.Data);
            }

            return MapResponseFromBLL(serResp);

            /*if (serResp.ResponseResult == ResponseResult.NotFound)
            {
                return NotFound();
            }

            if (serResp.ResponseResult == ResponseResult.AccessDenied)
            {
                return Forbid();
            }

            return BadRequest(serResp.ErrorMessage);*/
            
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteById(Guid id, [FromBody] FileDataModel fileDataModel)
        {
            if (id != fileDataModel.Id)
            {
                return BadRequest();
            }

            //TODO: add try, or result object
            var userId = _jwtHandler.GetUserId(this.User);
            await _fileService.DeleteOwnAsync(userId, id);
            return Ok();
        }

        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadFile(Guid id)
        {
            var userId = _jwtHandler.GetUserId(this.User);
            var respRes = await _fileService.GetFileByIdAsync(userId, id);

            if(respRes.ResponseResult == ResponseResult.Success 
                && respRes.Data != null && respRes.Data.AppFileNav != null)
            {
                new FileExtensionContentTypeProvider().TryGetContentType(respRes.Data.UntrustedName, out string? contentType);
                return File(respRes.Data.AppFileNav.Content, contentType ?? "text/plain", respRes.Data.UntrustedName);
            }

            return MapResponseFromBLL(respRes);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid Id, [FromBody] FileDataModel model)
        {
            if(Id != model.Id)
            {
                return BadRequest();
            }

            try
            {
                var userId = _jwtHandler.GetUserId(this.User);
                var respRes = await _fileService.UpdateByUserAsync(userId, model);
                
                if(respRes.ResponseResult == ResponseResult.Success)
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

        [HttpDelete("short/{link:length(6)}")]
        public async Task<ActionResult> DeleteShortLink(string link, [FromBody] FileDataModel model)
        {
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

        [HttpPut("short/{id}")]
        public async Task<ActionResult> CreateShortlink(Guid id, [FromBody] FileDataModel model)
        {
            if(id != model.Id)
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
                return BadRequest(result.ErrorMessage);
            }

        }

        [HttpPut("share/{email}")]
        public async Task<ActionResult> Share(string email, [FromBody] FileDataModel model)
        {
            //TODO: uncomment validation
            /*if(!model.Viewers.Any(fv => fv.Email == email))
            {
                return BadRequest();
            }*/

            var userId = _jwtHandler.GetUserId(this.User);
            var servResp = await _fileService.ShareByEmailAsync(userId, email, model.Id);
            
            if(servResp.ResponseResult == ResponseResult.Success && servResp.Data != null)
            {
                return Ok(servResp.Data);
            }

            return MapResponseFromBLL(servResp);
        }



        private ActionResult MapResponseFromBLL<T>(ServiceResponse<T> response)
        {
            //TODO: RESPONSE SUCCESS
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
