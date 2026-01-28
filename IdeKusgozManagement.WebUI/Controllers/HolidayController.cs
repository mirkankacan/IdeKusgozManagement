using IdeKusgozManagement.WebUI.Extensions;
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
        public async Task<IActionResult> GetHolidaysByYear(int year, CancellationToken cancellationToken)
        {
            if (year < 1900 || year > 2100)
            {
                return BadRequest("Geçersiz yıl değeri. 1900 - 2100 arasında olmalı");
            }
            var response = await _holidayApiService.GetHolidaysByYearAsync(year, cancellationToken);
            return response.ToActionResult();
        }

        [HttpGet("baslangic/{startDate:datetime}/bitis/{endDate:datetime}")]
        public async Task<IActionResult> GetHolidaysByYear(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
        {
            if (endDate < startDate)
            {
                return BadRequest("Bitiş tarihi, başlangıç tarihinden önce olamaz.");
            }
            if (startDate < new DateTime(2025, 1, 1))
            {
                return BadRequest("Başlangıç tarihi 2025'ten önce olamaz.");
            }
            var response = await _holidayApiService.CalculateWorkingDaysAsync(startDate, endDate, cancellationToken);
            return response.ToActionResult();
        }
    }
}