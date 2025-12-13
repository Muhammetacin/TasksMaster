using Application.Interfaces;
using Domain.Identities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;

        public TokenService(IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<string> GetTokenAsync(ApplicationUser applicationUser)
        {
            // 1. Configuratie ophalen
            // Haal secret uit van appsettings.json
            var secret = _configuration["JwtSettings:Secret"];
            var expiryMinutes = Convert.ToDouble(_configuration["JwtSettings:ExpiryMinutes"]);

            // 2. Security Key maken (Encoding en Symmetrische Key)
            // Zet om naar bytes, Security Key werkt met bytes
            var keyBytes = Encoding.UTF8.GetBytes(secret);

            // Nieuw Security Key aanmaken met secret in bytes
            var securityKey = new SymmetricSecurityKey(keyBytes);

            // 3. Claims opbouwen (ID, Naam, E-mail)
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, applicationUser.Id),
                new Claim(ClaimTypes.Email, applicationUser.Email),
                new Claim(ClaimTypes.Name, applicationUser.UserName)
            };

            // 4. Rollen claims toevoegen via UserManager
            // Rollen ophalen en toevoegen aan claims
            var roles = await _userManager.GetRolesAsync(applicationUser);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // 5. Token Descriptor definiëren
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            // 6. Token aanmaken en retourneren als string
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
