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
    }
}