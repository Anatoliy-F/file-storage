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

        public AccountController(UserManager<AppUser> userManager, JwtHandler jwtHandler,
            RoleManager<IdentityRole<Guid>> roleManager, IUserService userService)
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

            var user = new AppUser
            {
                UserName = userForRegistration.UserName,
                Email = userForRegistration.Email,
            };

            if (await _roleManager.FindByNameAsync("RegisteredUser") == null)
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

            if (user == null || !await _userManager.CheckPasswordAsync(user, loginRequest.Password))
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

        //TODO: Is I need this endpoint?
        [Authorize(Roles = "RegisteredUser")]
        [HttpGet("isExist/{email}")]
        public async Task<IActionResult> IsUserExist(EmailRequestModel email)
        {
            if(email == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var servRes = await _userService.IsExistByEmailAsync(email.Address);
            if (servRes.ResponseResult == ResponseResult.Success)
            {
                return Ok(new { isSuccess = true });
            }

            return MapResponseFromBLL(servRes);
        }

        [Authorize(Roles = "RegisteredUser")]
        [HttpGet("byEmail/{email}")]
        public async Task<IActionResult> GetByEmail(EmailRequestModel email)
        {
            if(email == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var servRes = await _userService.GetByEmailAsync(email.Address);
            if (servRes.ResponseResult == ResponseResult.Success)
            {
                return Ok(servRes.Data);
            }
            return MapResponseFromBLL(servRes);
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(Guid id)
        {
            var servRes = await _userService.GetByIdAsync(id);

            if (servRes.ResponseResult == ResponseResult.Success && servRes.Data != null)
            {
                return new JsonResult(servRes.Data);
            }

            return MapResponseFromBLL(servRes);
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<ActionResult<PaginationResultModel<UserModel>>> Get([FromQuery] QueryModel query)
        {
            var servRes = await _userService.GetAllAsync(query);

            if (servRes.ResponseResult == ResponseResult.Success && servRes.Data != null)
            {
                return new JsonResult(servRes.Data);
            }

            return MapResponseFromBLL(servRes);
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id, [FromBody] UserModel userModel)
        {
            if(id != userModel.Id)
            {
                return BadRequest();
            }

            var servResp = await _userService.DeleteAsync(userModel);

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
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] UserModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            var servResp = await _userService.UpdateAsync(model);

            if (servResp.ResponseResult == ResponseResult.Success)
            {
                return Ok();
            }
            else
            {
                return MapResponseFromBLL(servResp);
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
