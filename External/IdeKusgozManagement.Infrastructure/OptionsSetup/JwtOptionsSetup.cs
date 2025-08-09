using IdeKusgozManagement.Application.DTOs.OptionDTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace IdeKusgozManagement.Infrastructure.OptionsSetup
{
    public class JwtOptionsSetup : IConfigureOptions<JwtOptionsDto>
    {
        private readonly IConfiguration _configuration;

        public JwtOptionsSetup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(JwtOptionsDto options)
        {
            _configuration.GetSection("JwtConfiguration").Bind(options);
        }
    }
}