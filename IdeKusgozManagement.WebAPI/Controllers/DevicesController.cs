using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.DeviceDTOs;
using IdeKusgozManagement.WebAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesController(IDeviceService deviceService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> RegisterDevice([FromBody] RegisterDeviceDTO dto, CancellationToken cancellationToken)
        {
            var result = await deviceService.RegisterDeviceAsync(dto, cancellationToken);

            return result.ToActionResult();
        }
    }
}