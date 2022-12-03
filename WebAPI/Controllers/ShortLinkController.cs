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
    [Route("")]
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
                && result.Data != null && result.Data.AppFileNav != null)
            {
                new FileExtensionContentTypeProvider().TryGetContentType(result.Data.UntrustedName, out string? contentType);
                return File(result.Data.AppFileNav.Content, contentType ?? "text/plain", result.Data.UntrustedName);
            }

            /*if(result.ResponseResult == ResponseResult.NotFound)
            {
                return NotFound();
            }

            if(result.ResponseResult == ResponseResult.AccessDenied)
            {
                return Forbid();
            }

            return BadRequest(result.ErrorMessage);*/
            return MapResponseFromBLL(result);
        }

        //TODO: file preview by short link
        //[Authorize(Roles = "RegisteredUser")]
        [HttpGet("/preview/{link:length(6)}")]
        public async Task<IActionResult> GetFileData(string link)
        {
            var result = await _shortLinkService.GetShortFileDataAsync(link);
            if(result.ResponseResult == ResponseResult.Success && result.Data != null)
            {
                return Ok(result.Data);
            }
            /*if (result.ResponseResult == ResponseResult.NotFound)
            {
                return NotFound();
            }

            if (result.ResponseResult == ResponseResult.AccessDenied)
            {
                return Forbid();
            }

            return BadRequest(result.ErrorMessage);*/
            return MapResponseFromBLL(result);
        }

        private IActionResult MapResponseFromBLL<T>(ServiceResponse<T> response)
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
