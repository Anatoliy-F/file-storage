﻿using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebAPI.Utilities
{
    public class JwtHandler
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;

        public ILogger<JwtHandler> Logger { get; set; }

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
