using IdeKusgozManagement.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Controllers
{
    [Route(template: "tatil")]
    public class HolidayController : Controller
    {
        private readonly IHolidayApiService _holidayApiService;

        public HolidayController(IHolidayApiService holidayApiService)
        {
            _holidayApiService = holidayApiService;
        }

        [HttpGet("{year:int}/yili")]
        public async Task<IActionResult> GetHolidaysByYear(int year, CancellationToken cancellationToken = default)
        {
            var response = await _holidayApiService.GetHolidaysByYear(year, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}