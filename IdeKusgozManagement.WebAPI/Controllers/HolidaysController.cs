using IdeKusgozManagement.Application.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class HolidaysController(IHolidayService holidayService) : ControllerBase
    {
        [HttpGet("{year:int}")]
        public async Task<IActionResult> GetHolidaysByYear(int year, CancellationToken cancellationToken = default)
        {
            if (year < 1900 || year > 2100)
            {
                return BadRequest("Geçersiz yıl değeri. 1900 - 2100 arasında olmalı");
            }
            var result = await holidayService.GetHolidaysByYearAsync(year, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("start/{startDate:datetime}/end/{endDate:datetime}")]
        public async Task<IActionResult> CalculateWorkingDays(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            if (endDate < startDate)
            {
                return BadRequest("Bitiş tarihi, başlangıç tarihinden önce olamaz.");
            }
            if (startDate < new DateTime(2025, 1, 1))
            {
                return BadRequest("Başlangıç tarihi 2025'ten önce olamaz.");
            }
            var result = await holidayService.CalculateWorkingDaysAsync(startDate, endDate, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}