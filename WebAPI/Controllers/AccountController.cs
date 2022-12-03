using BuisnessLogicLayer.Interfaces;
using WebAPI.Models;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using WebAPI.Utilities;
using BuisnessLogicLayer.Enums;
using BuisnessLogicLayer.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IUserService _userService;
        private readonly JwtHandler _jwtHandler;

        public AccountController(UserManager<AppUser> userManager, JwtHandler jwtHandler, RoleManager<IdentityRole<Guid>> roleManager, IUserService userService)
        {
            _userManager = userManager;
            _jwtHandler = jwtHandler;
            _roleManager = roleManager;
            _userService = userService;
        }

        [HttpPost("Registration")]
        public async Task<IActionResult> registerUser([FromBody] RegistrationRequestModel userForRegistration)
        {
            if (userForRegistration == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //User.

            var user = new AppUser
            {
                UserName = userForRegistration.UserName,
                Email = userForRegistration.Email,
            };

            //TODO: init roles before run application
            //create role Registereduser if it don't exist yet
            if(await _roleManager.FindByNameAsync("RegisteredUser") == null)
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>("RegisteredUser"));
            }

            var result = await _userManager.CreateAsync(user, userForRegistration.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new RegistrationResponseModel { Errors = errors });
            }

            await _userManager.AddToRoleAsync(user, "RegisteredUser");

            return Ok(new RegistrationResponseModel()
            {
                Success = true
            });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel loginRequest)
        {
            var user = await _userManager.FindByEmailAsync(loginRequest.Email);
            if(user == null || !await _userManager.CheckPasswordAsync(user, loginRequest.Password))
            {
                return Unauthorized(new LoginResponseModel()
                {
                    Success = false,
                    Message = "Invalid Email or Password."
                });
            }

            bool isAdmin = await _userManager.IsInRoleAsync(user, "Administrator");

            var secToken = await _jwtHandler.GetTokenAsync(user);
            var jwt = new JwtSecurityTokenHandler().WriteToken(secToken);
            return Ok(new LoginResponseModel()
            {
                Success = true,
                Message = "Login successful",
                IsAdmin = isAdmin,
                Token = jwt
            });
        }

        //TODO: is I need this endpoint
        [Authorize(Roles = "RegisteredUser")]
        [HttpGet("isExist/{email}")]
        public async Task<IActionResult> IsUserExist(string email)
        {
            var servRes = await _userService.IsExistByEmailAsync(email);
            if (servRes.ResponseResult == ResponseResult.Success)
            {
                return Ok(new { isSuccess = true });
            }

            return MapResponseFromBLL(servRes);
        }

        [Authorize(Roles = "RegisteredUser")]
        [HttpGet("byEmail/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var servRes = await _userService.GetByEmailAsync(email);
            if(servRes.ResponseResult == ResponseResult.Success)
            {
                return Ok(servRes.Data);
            }
            return MapResponseFromBLL(servRes);
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
