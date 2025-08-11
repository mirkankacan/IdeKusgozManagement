using System.Text;
using IdeKusgozManagement.Application.DTOs.OptionDTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace IdeKusgozManagement.Infrastructure.OptionsSetup
{
    public sealed class JwtBearerOptionsSetup : IPostConfigureOptions<JwtBearerOptions>
    {
        private readonly JwtOptionsDTO _jwtOptions;

        public JwtBearerOptionsSetup(IOptions<JwtOptionsDTO> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
        }

        public void PostConfigure(string? name, JwtBearerOptions options)
        {
            options.TokenValidationParameters.ValidateIssuer = true;
            options.TokenValidationParameters.ValidateAudience = true;
            options.TokenValidationParameters.ValidateLifetime = true;
            options.TokenValidationParameters.ValidateIssuerSigningKey = true;
            options.TokenValidationParameters.ValidIssuer = _jwtOptions.Issuer;
            options.TokenValidationParameters.ValidAudience = _jwtOptions.Audience;
            options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
            options.TokenValidationParameters.ClockSkew = TimeSpan.FromMinutes(1);

        }
    }
}