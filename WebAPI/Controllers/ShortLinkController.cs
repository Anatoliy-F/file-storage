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

            if (result.IsSuccess && result.Data != null && result.Data.AppFileNav != null)
            {
                new FileExtensionContentTypeProvider().TryGetContentType(result.Data.UntrustedName, out string? contentType);
                return File(result.Data.AppFileNav.Content, contentType ?? "text/plain", result.Data.UntrustedName);
            }

            return BadRequest(result.ErrorMessage);
        }

        //TODO: file preview by short link
        [Authorize(Roles = "RegisteredUser")]
        [HttpGet("/preview/{link:length(6)}")]
        public async Task<IActionResult> GetFileData(string link)
        {
            var result = await _shortLinkService.GetShortFileDataAsync(link);
            if(result.IsSuccess && result.Data != null)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.ErrorMessage);
        }
    }
}
