using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebAPI.Utilities
{
    /// <summary>
    /// Generate JWT
    /// </summary>
    public class JwtHandler
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;
        private ILogger<JwtHandler> Logger { get; set; }

        /// <summary>
        /// Initialize new instance of JwtHandler
        /// </summary>
        /// <param name="configuration">IConfiguration instanse, for access to application configuration</param>
        /// <param name="userManager">Provides the APIs for managing user in a persistence store</param>
        /// <param name="logger">ILogger object to performing error logging</param>
        public JwtHandler(IConfiguration configuration, UserManager<AppUser> userManager, ILogger<JwtHandler> logger)
        {
            _configuration = configuration;
            _userManager = userManager;
            Logger = logger;
        }

        private SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecurityKey"]);
            var secret = new SymmetricSecurityKey(key);
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        /// <summary>
        /// Generate JWT security token
        /// </summary>
        /// <param name="user">AppUser instanse <see cref="AppUser"/></param>
        /// <returns><see cref="JwtSecurityToken"/></returns>
        public async Task<JwtSecurityToken> GetTokenAsync(AppUser user)
        {
            var jwtOptions = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: await GetClaimsAsync(user),
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:ExpirationTimeInMinutes"])),
                signingCredentials: GetSigningCredentials());

            return jwtOptions;
        }

        /// <summary>
        /// Return user Id from JWT
        /// </summary>
        /// <param name="user"><see cref="ClaimsPrincipal"/></param>
        /// <returns>Guid user id</returns>
        /// <exception cref="UnauthorizedAccessException">Throws if token doesn't contains user id</exception>
        public Guid GetUserId(ClaimsPrincipal user)
        {
            string? id = user?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if(id == null)
            {
                Logger.LogError("User don't have claims in bearer token! We under attack");
                throw new UnauthorizedAccessException("Bearer doesn't contain id");
            }
            return new Guid(id);
        }

        private async Task<List<Claim>> GetClaimsAsync(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email)
            };

            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()));

            foreach (var role in await _userManager.GetRolesAsync(user))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            return claims;
        }
    }
}
