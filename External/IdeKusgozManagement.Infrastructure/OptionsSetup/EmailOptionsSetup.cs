using IdeKusgozManagement.Application.DTOs.OptionDTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace IdeKusgozManagement.Infrastructure.OptionsSetup
{
    public class EmailOptionsSetup : IConfigureOptions<EmailOptionsDTO>
    {
        private readonly IConfiguration _configuration;

        public EmailOptionsSetup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(EmailOptionsDTO options)
        {
            _configuration.GetSection("EmailConfiguration").Bind(options);
        }
    }
}