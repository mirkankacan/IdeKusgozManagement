using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.TrafficTicketDTOs;
using IdeKusgozManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [RoleFilter("Admin", "Yönetici")]
    [Route("api/[controller]")]
    [ApiController]
    public class TrafficTicketsController(ITrafficTicketService trafficTicketService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetTrafficTickets(CancellationToken cancellationToken = default)
        {
            var result = await trafficTicketService.GetTrafficTicketsAsync(cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{ticketId}")]
        public async Task<IActionResult> GetProjectById(string ticketId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(ticketId))
            {
                return BadRequest("Trafik cezası ID'si gereklidir");
            }

            var result = await trafficTicketService.GetTrafficTicketByIdAsync(ticketId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTrafficTicket([FromForm] CreateTrafficTicketDTO createTrafficTicketDTO, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await trafficTicketService.CreateTrafficTicketAsync(createTrafficTicketDTO, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{ticketId}")]
        public async Task<IActionResult> UpdateTrafficTicket(string ticketId, [FromForm] UpdateTrafficTicketDTO updateTrafficTicketDTO, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(ticketId))
            {
                return BadRequest("Trafik cezası ID'si gereklidir");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await trafficTicketService.UpdateTrafficTicketAsync(ticketId, updateTrafficTicketDTO, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{ticketId}")]
        public async Task<IActionResult> DeleteTrafficTicket(string ticketId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(ticketId))
            {
                return BadRequest("Trafik cezası ID'si gereklidir");
            }

            var result = await trafficTicketService.DeleteTrafficTicketAsync(ticketId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}