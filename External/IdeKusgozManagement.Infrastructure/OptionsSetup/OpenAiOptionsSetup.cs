using IdeKusgozManagement.Application.DTOs.OptionDTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace IdeKusgozManagement.Infrastructure.OptionsSetup
{
    public class OpenAiOptionsSetup : IConfigureOptions<OpenAiOptionsDTO>
    {
        private readonly IConfiguration _configuration;

        public OpenAiOptionsSetup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(OpenAiOptionsDTO options)
        {
            _configuration.GetSection("OpenAiConfiguration").Bind(options);
        }
    }
}