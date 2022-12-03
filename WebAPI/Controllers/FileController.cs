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

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly IShortLinkService _shortLinkService;
        private readonly JwtHandler _jwtHandler;

        public FileController(IFileService fileService, JwtHandler jwtHandler, IShortLinkService shortLinkService)
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

            if (serResp.IsSuccess && serResp.Data != null)
            {
                return new JsonResult(serResp.Data);
            }
            return BadRequest(serResp.ErrorMessage);
            
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
        public async Task<ActionResult> DownloadFile(Guid id)
        {
            var userId = _jwtHandler.GetUserId(this.User);
            var fileData = await _fileService.GetFileByIdAsync(userId, id);
            if (fileData == null || fileData.AppFileNav == null)
            {
                return NotFound();
            }

            new FileExtensionContentTypeProvider().TryGetContentType(fileData.UntrustedName, out string? contentType);
            return File(fileData.AppFileNav.Content, contentType ?? "text/plain", fileData.UntrustedName);
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
                await _fileService.UpdateByUserAsync(userId, model);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                //throw;
            }
        }

        [HttpDelete("short/{link:length(6)}")]
        public async Task<ActionResult> DeleteShortLink(string link, [FromBody] FileDataModel model)
        {
            var result = await _shortLinkService.DeleteLinkAsync(link);
            if (result.IsSuccess)
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

            if (result.IsSuccess)
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
            var fdm = await _fileService.ShareByEmailAsync(userId, email, model.Id);
            if(fdm == null)
            {
                BadRequest();
            }
            return Ok(fdm);
        }
    }
}
