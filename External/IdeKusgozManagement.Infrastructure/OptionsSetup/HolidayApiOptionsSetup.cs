using IdeKusgozManagement.Application.DTOs.OptionDTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace IdeKusgozManagement.Infrastructure.OptionsSetup
{
    public class HolidayApiOptionsSetup : IConfigureOptions<HolidayApiOptionsDTO>
    {
        private readonly IConfiguration _configuration;

        public HolidayApiOptionsSetup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(HolidayApiOptionsDTO options)
        {
            _configuration.GetSection("HolidayApiConfiguration").Bind(options);
        }
    }
}