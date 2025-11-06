using IdeKusgozManagement.Application.DTOs.OptionDTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace IdeKusgozManagement.Infrastructure.OptionsSetup
{
    public class GeminiAiOptionsSetup : IConfigureOptions<GeminiAiOptionsDTO>
    {
        private readonly IConfiguration _configuration;

        public GeminiAiOptionsSetup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(GeminiAiOptionsDTO options)
        {
            _configuration.GetSection("GeminiAiConfiguration").Bind(options);
        }
    }
}