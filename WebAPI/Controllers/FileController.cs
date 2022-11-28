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

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly JwtHandler _jwtHandler;

        public FileController(IFileService fileService, JwtHandler jwtHandler)
        {
            _fileService = fileService;
            _jwtHandler = jwtHandler;
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
            var fileData = await _fileService.GetOwnByIdAsync(userId, id);

            if(fileData == null)
            {
                return NotFound();
            }
            return new JsonResult(fileData);
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
            await _fileService.DeleteOwnByIdAsync(userId, id);
            return Ok();
        }
    }
}
