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
using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Manage SignIn/SignUp and operations with users
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IUserService _userService;
        private readonly JwtHandler _jwtHandler;

        /// <summary>
        /// Initialize new instance of FileController
        /// </summary>
        /// <param name="userManager">Provides the APIs for managing user in a persistence store</param>
        /// <param name="jwtHandler">JwtHandler instanse. Generate JWT. Retrive userId from JWT</param>
        /// <param name="roleManager">Managing user roles in a persistence store</param>
        /// <param name="userService">UserService instanse. Provides operations with users objects</param>
        public AccountController(UserManager<AppUser> userManager, JwtHandler jwtHandler,
            RoleManager<IdentityRole<Guid>> roleManager, IUserService userService)
        {
            _userManager = userManager;
            _jwtHandler = jwtHandler;
            _roleManager = roleManager;
            _userService = userService;
        }

        /// <summary>
        /// Performs user registration (sign up) in the application
        /// </summary>
        /// <param name="userForRegistration">Represent data for sign up request <see cref="RegistrationRequestModel"/></param>
        /// <returns>Result of registration attempt <see cref="RegistrationResponseModel"/></returns>
        [HttpPost("Registration")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(200, "The execution was successful")]
        [SwaggerResponse(400, "The request was invalid")]
        public async Task<IActionResult> RegisterUser([FromBody] RegistrationRequestModel userForRegistration)
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

        /// <summary>
        /// Performs user registration (sign in) in the application
        /// </summary>
        /// <param name="loginRequest">Represent data for sign in request <see cref="LoginRequestModel"/></param>
        /// <returns>Result of login attempt <see cref="LoginResponseModel"/></returns>
        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(200, "The execution was successful")]
        [SwaggerResponse(400, "The request was invalid")]
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

        /// <summary>
        /// Deprecated
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [Authorize(Roles = "RegisteredUser")]
        [HttpGet("isExist/{email}")]
        public async Task<IActionResult> IsUserExist(string email)
        {
            if(email == null)
            {
                return BadRequest(ModelState);
            }

            var servRes = await _userService.IsExistByEmailAsync(email);
            if (servRes.ResponseResult == ResponseResult.Success)
            {
                return Ok(new { isSuccess = true });
            }

            return MapResponseFromBLL(servRes);
        }

        /// <summary>
        /// Return user model object by email
        /// </summary>
        /// <param name="email">Email address</param>
        /// <returns>Objects represents user data <see cref="UserModel"/></returns>
        [Authorize(Roles = "RegisteredUser")]
        [HttpGet("byEmail/{email}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(200, "The execution was successful")]
        [SwaggerResponse(404, "Not found user with this email")]
        [SwaggerResponse(400, "The request was invalid")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            if(email == null)
            {
                return BadRequest(ModelState);
            }

            var servRes = await _userService.GetByEmailAsync(email);
            if (servRes.ResponseResult == ResponseResult.Success)
            {
                return Ok(servRes.Data);
            }
            return MapResponseFromBLL(servRes);
        }

        /// <summary>
        /// Returns an object describing user
        /// For execution needs "Administrator" permissions
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>Objects represents user data <see cref="UserModel"/></returns>
        [Authorize(Roles = "Administrator")]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(200, "The execution was successful")]
        [SwaggerResponse(404, "Not found user with this email")]
        [SwaggerResponse(400, "The request was invalid")]
        public async Task<ActionResult> GetById(Guid id)
        {
            var servRes = await _userService.GetByIdAsync(id);

            if (servRes.ResponseResult == ResponseResult.Success && servRes.Data != null)
            {
                return new JsonResult(servRes.Data);
            }

            return MapResponseFromBLL(servRes);
        }

        /// <summary>
        /// Returns a page of sorted and filtered objects describing users
        /// Page size, query for filtering, property for sorting and sorting order are defined by QueryModel object
        /// For execution needs "Administrator" permissions
        /// </summary>
        /// <param name="query">QueryModel object, incapsulate query options for pagination, sorting, filtering <see cref="QueryModel"/></param>
        /// <returns>Returns filtered, sorted page of users data</returns>
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(200, "The execution was successful")]
        [SwaggerResponse(400, "The request was invalid")]
        public async Task<ActionResult<PaginationResultModel<UserModel>>> Get([FromQuery] QueryModel query)
        {
            var servRes = await _userService.GetAllAsync(query);

            if (servRes.ResponseResult == ResponseResult.Success && servRes.Data != null)
            {
                return new JsonResult(servRes.Data);
            }

            return MapResponseFromBLL(servRes);
        }

        /// <summary>
        /// Deleting user. For execution needs "Administrator" permissions
        /// Can't delete user with "Administrator" rights
        /// </summary>
        /// <param name="id">User Id</param>
        /// <param name="userModel">Objects represents user data <see cref="UserModel"/></param>
        /// <returns>Status 200 Ok, if file delete successfuly, error code otherwise</returns>
        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(200, "The execution was successful")]
        [SwaggerResponse(403, "Access denied, user has \"Administrator\" permissions")]
        [SwaggerResponse(400, "The request was invalid")]
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

        /// <summary>
        /// Update user object
        /// </summary>
        /// <param name="id">User Id</param>
        /// <param name="model">Objects represents user data <see cref="UserModel"/></param>
        /// <returns>Status 200 Ok, if file update successfuly, error code otherwise</returns>
        [Authorize(Roles = "Administrator")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(200, "The execution was successful")]
        [SwaggerResponse(400, "The request was invalid")]
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
