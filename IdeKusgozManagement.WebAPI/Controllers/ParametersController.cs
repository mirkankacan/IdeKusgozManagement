using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.ParameterDTOs;
using IdeKusgozManagement.WebAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class ParametersController(IParameterService parameterService) : ControllerBase
    {
        [Authorize(Roles = "Admin,Yönetici")]
        [HttpGet]
        public async Task<IActionResult> GetParameters(CancellationToken cancellationToken)
        {
            var result = await parameterService.GetParametersAsync(cancellationToken);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpGet("{key}")]
        public async Task<IActionResult> GetParameterByKey(string key, CancellationToken cancellationToken)
        {
            var result = await parameterService.GetParameterByKeyAsync(key, cancellationToken);
            return result.ToActionResult();
        }

        [Authorize(Roles = "Admin,Yönetici")]
        [HttpPost]
        public async Task<IActionResult> CreateParameter([FromBody] CreateParameterDTO dto, CancellationToken cancellationToken)
        {
            var result = await parameterService.CreateParameterAsync(dto, cancellationToken);
            return result.ToActionResult();
        }

        [Authorize(Roles = "Admin,Yönetici")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateParameter(string id, [FromBody] UpdateParameterDTO dto, CancellationToken cancellationToken)
        {
            var result = await parameterService.UpdateParameterAsync(id, dto, cancellationToken);
            return result.ToActionResult();
        }

        [Authorize(Roles = "Admin,Yönetici")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParameter(string id, CancellationToken cancellationToken)
        {
            var result = await parameterService.DeleteParameterAsync(id, cancellationToken);
            return result.ToActionResult();
        }
    }
}