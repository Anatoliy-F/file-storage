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
using BuisnessLogicLayer.Enums;
using DataAccessLayer.Interfaces;
using BuisnessLogicLayer.Interfaces;
using WebAPI.Utilities;
using Microsoft.AspNetCore.StaticFiles;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShortLinkController : ControllerBase
    {
        private readonly IShortLinkService _shortLinkService;

        public ShortLinkController(IShortLinkService shortLinkService)
        {
            _shortLinkService = shortLinkService;
        }

        [HttpGet("/{link:length(6)}")]
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

        [Authorize(Roles = "RegisteredUser")]
        [HttpGet("preview/{link:length(6)}")]
        public async Task<IActionResult> GetFileData(string link)
        {
            var result = await _shortLinkService.GetShortFileDataAsync(link);
            if(result.ResponseResult == ResponseResult.Success && result.Data != null)
            {
                return Ok(result.Data);
            }

            return MapResponseFromBLL(result);
        }

        //TODO: return shortLinkModel response
        [Authorize(Roles = "RegisteredUser")]
        [HttpPost("{id}")]
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

        //TODO: dont return anything else 
        [Authorize(Roles = "RegisteredUser")]
        [HttpDelete("{link:length(6)}")]
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
