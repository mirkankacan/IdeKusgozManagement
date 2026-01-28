using IdeKusgozManagement.Application.DTOs.AuthDTOs;
using IdeKusgozManagement.Application.DTOs.OptionDTOs;
using IdeKusgozManagement.Application.Interfaces.Providers;
using IdeKusgozManagement.Domain.Entities;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<JwtProvider> _logger;
        public JwtProvider(IOptions<JwtOptionsDTO> jwtOptions, UserManager<ApplicationUser> userManager, ILogger<JwtProvider> logger)
        {
            _jwtOptions = jwtOptions.Value;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<TokenDTO> CreateTokenAsync(ApplicationUser applicationUser)
        {
            var userRoles = await _userManager.GetRolesAsync(applicationUser);
            var user = await _userManager.Users.Include(u => u.Department).Include(u => u.DepartmentDuty).FirstOrDefaultAsync(u => u.Id == applicationUser.Id);
            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Jti, NewId.NextGuid().ToString()),
        new Claim("TCNo", applicationUser.TCNo!),
        new Claim("FullName", $"{applicationUser.Name} {applicationUser.Surname}"),
        new Claim(ClaimTypes.NameIdentifier, applicationUser.Id),
        new Claim(ClaimTypes.Role, userRoles.FirstOrDefault()!),
        new Claim("DepartmentName", applicationUser.Department.Name),
        new Claim("DepartmentDutyName", applicationUser.DepartmentDuty.Name),
    };

            DateTime expires = DateTime.UtcNow.AddHours(1);
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expires,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)),
                    SecurityAlgorithms.HmacSha256));

            string token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

            applicationUser.RefreshToken = refreshToken;
            applicationUser.RefreshTokenExpires = DateTime.UtcNow.AddDays(7);
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
                RoleName = userRoles.FirstOrDefault() ?? string.Empty,
                IsExpatriate = applicationUser.IsExpatriate,
                DepartmentName = applicationUser.Department.Name,
                DepartmentDutyName = applicationUser.DepartmentDuty.Name
            };
            // DETAYLI LOG (Test için)
            _logger.LogDebug($"═══════════════════════════════════════════════════");
            _logger.LogDebug($"TOKEN OLUŞTURULDU");
            _logger.LogDebug($"  UserId: {applicationUser.Id}");
            _logger.LogDebug($"  Access Token Expire: {expires:yyyy-MM-dd HH:mm:ss} UTC");
            _logger.LogDebug($"  Access Token (ilk 20 karakter): {token.Substring(0, Math.Min(20, token.Length))}...");
            _logger.LogDebug($"  Refresh Token (ilk 10 karakter): {refreshToken.Substring(0, 10)}...");
            _logger.LogDebug($"  Refresh Token Expire: {applicationUser.RefreshTokenExpires:yyyy-MM-dd HH:mm:ss} UTC");
            _logger.LogDebug($"  Refresh Token DB'ye kaydedildi: ✓");
            _logger.LogDebug($"═══════════════════════════════════════════════════");

            return response;
        }
    }
}