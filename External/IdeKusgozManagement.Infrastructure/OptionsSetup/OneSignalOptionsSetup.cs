using IdeKusgozManagement.Application.DTOs.OptionDTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace IdeKusgozManagement.Infrastructure.OptionsSetup
{
    public class OneSignalOptionsSetup : IConfigureOptions<OneSignalOptionsDTO>
    {
        private readonly IConfiguration _configuration;

        public OneSignalOptionsSetup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(OneSignalOptionsDTO options)
        {
            _configuration.GetSection("OneSignalConfiguration").Bind(options);
        }
    }
}