using IdeKusgozManagement.Application.DTOs.AuthDTOs;
using IdeKusgozManagement.Application.DTOs.OptionDTOs;
using IdeKusgozManagement.Application.Interfaces.Providers;
using IdeKusgozManagement.Domain.Entities;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace IdeKusgozManagement.Infrastructure.Authentication
{
    public class JwtProvider : IJwtProvider
    {
        private readonly JwtOptionsDTO _jwtOptions;
        private readonly UserManager<ApplicationUser> _userManager;

        public JwtProvider(IOptions<JwtOptionsDTO> jwtOptions, UserManager<ApplicationUser> userManager)
        {
            _jwtOptions = jwtOptions.Value;
            _userManager = userManager;
        }

        public async Task<TokenDTO> CreateTokenAsync(ApplicationUser applicationUser)
        {
            // Kullanıcının rollerini al
            var userRoles = await _userManager.GetRolesAsync(applicationUser);

            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Jti, NewId.NextGuid().ToString()),
        new Claim("TCNo", applicationUser.TCNo!),
        new Claim("FullName", $"{applicationUser.Name} {applicationUser.Surname}"),
        new Claim(ClaimTypes.NameIdentifier, applicationUser.Id),
        new Claim(ClaimTypes.Role, userRoles.FirstOrDefault()!)
    };

            DateTime expires = DateTime.Now.AddHours(1);
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: DateTime.Now,
                expires: expires,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)),
                    SecurityAlgorithms.HmacSha256));

            string token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

            // Refresh token'ı kullanıcıya kaydet
            applicationUser.RefreshToken = refreshToken;
            applicationUser.RefreshTokenExpires = expires.AddDays(7);
            await _userManager.UpdateAsync(applicationUser);

            TokenDTO response = new()
            {
                Token = token,
                TokenExpires = expires,
                RefreshToken = refreshToken,
                RefreshTokenExpires = applicationUser.RefreshTokenExpires,
                UserId = applicationUser.Id,
                TCNo = applicationUser.TCNo,
                Name = applicationUser.Name,
                Surname = applicationUser.Surname,
                RoleName = userRoles.FirstOrDefault(),
                IsExpatriate = applicationUser.IsExpatriate
            };
            Console.WriteLine($"TOKEN OLUŞTURULDU - Expire: {expires:yyyy-MM-dd HH:mm:ss}");

            return response;
        }
    }
}