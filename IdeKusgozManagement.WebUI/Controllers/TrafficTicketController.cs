using IdeKusgozManagement.WebUI.Extensions;
using IdeKusgozManagement.WebUI.Models.TrafficTicketModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Controllers
{
    [Route("trafik-ceza")]
    [Authorize(Roles = "Admin, Yönetici")]
    public class TrafficTicketController : Controller
    {
        private readonly ITrafficTicketApiService _trafficTicketApiService;

        public TrafficTicketController(ITrafficTicketApiService trafficTicketApiService)
        {
            _trafficTicketApiService = trafficTicketApiService;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("liste")]
        public async Task<IActionResult> GetTrafficTickets(CancellationToken cancellationToken)
        {
            var response = await _trafficTicketApiService.GetTrafficTicketsAsync(cancellationToken);
            return response.ToActionResult();
        }

        [HttpGet("{ticketId}")]
        public async Task<IActionResult> GetTrafficTicketById(string ticketId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(ticketId))
            {
                return BadRequest("Trafik cezası ID'si gereklidir");
            }

            var response = await _trafficTicketApiService.GetTrafficTicketByIdAsync(ticketId, cancellationToken);
            return response.ToActionResult();
        }

        [ValidateAntiForgeryToken]
        [HttpPost("")]
        public async Task<IActionResult> CreateTrafficTicket([FromForm] CreateTrafficTicketViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _trafficTicketApiService.CreateTrafficTicketAsync(model, cancellationToken);
            return response.ToActionResult();
        }

        [ValidateAntiForgeryToken]
        [HttpPut("{ticketId}")]
        public async Task<IActionResult> UpdateTrafficTicket(string ticketId, [FromForm] UpdateTrafficTicketViewModel model, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(ticketId))
            {
                return BadRequest("Trafik cezası ID'si gereklidir");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _trafficTicketApiService.UpdateTrafficTicketAsync(ticketId, model, cancellationToken);
            return response.ToActionResult();
        }

        [ValidateAntiForgeryToken]
        [HttpDelete("{ticketId}")]
        public async Task<IActionResult> DeleteTrafficTicket(string ticketId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(ticketId))
            {
                return BadRequest("Trafik cezası ID'si gereklidir");
            }

            var response = await _trafficTicketApiService.DeleteTrafficTicketAsync(ticketId, cancellationToken);
            return response.ToActionResult();
        }
    }
}