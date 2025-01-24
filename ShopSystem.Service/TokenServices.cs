using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ShopSystem.Core.Models.Identity;
using ShopSystem.Core.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ShopSystem.Service
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration configuration;

        // Constructor to initialize the TokenService with IConfiguration
        public TokenService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // Method to create a JWT token for the provided AppUser
        public async Task<string> CreateTokenAsync(AppUser user)
        {
            // Payload [Data] [Claims]
            // 1. Private Claims
            var authClaims = new List<Claim>()
        {
            //new Claim(ClaimTypes.NameIdentifier, "UserID"), // Adding the user ID claim
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Adding the user ID claim
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.GivenName, user.FirstName),
            new Claim(ClaimTypes.GivenName, user.LastName),
        };

            // 2. Register Claims

            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:key"]));
            var token = new JwtSecurityToken(
                            issuer: configuration["JWT:ValidIssuer"],
                            audience: configuration["JWT:ValidAudience"],
                            expires: DateTime.Now.AddDays(double.Parse(configuration["JWT:DurationInDays"])),
                            claims: authClaims,
                            signingCredentials: new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256)
                            );

            // Serialize the JWT token to a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
