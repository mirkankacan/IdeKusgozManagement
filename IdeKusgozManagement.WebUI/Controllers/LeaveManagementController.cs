using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Controllers
{
    public class LeaveManagementController : Controller
    {
        private readonly ILeaveRequestApiService _leaveRequestApiService;

        public LeaveManagementController(ILeaveRequestApiService leaveRequestApiService)
        {
            _leaveRequestApiService = leaveRequestApiService;
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpGet("liste")]
        public async Task<IActionResult> GetLeaveRequests(CancellationToken cancellationToken = default)
        {
            var response = await _leaveRequestApiService.GetAllLeaveRequestsAsync(cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpGet("aktif-liste")]
        public async Task<IActionResult> GetActiveLeaveRequests(CancellationToken cancellationToken = default)
        {
            var response = await _leaveRequestApiService.GetAllActiveLeaveRequestsAsync(cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLeaveRequestById(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("İzin isteği ID'si gereklidir");
            }

            var response = await _leaveRequestApiService.GetLeaveRequestByIdAsync(id, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpPost("")]
        public async Task<IActionResult> CreateLeaveRequest([FromBody] CreateLeaveRequestViewModel model, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _leaveRequestApiService.CreateLeaveRequestAsync(model, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLeaveRequest(string id, [FromBody] UpdateLeaveRequestViewModel model, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("İzin isteği ID'si gereklidir");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _leaveRequestApiService.UpdateLeaveRequestAsync(id, model, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLeaveRequest(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("İzin isteği ID'si gereklidir");
            }

            var response = await _leaveRequestApiService.DeleteLeaveRequestAsync(id, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpPut("{id}/onayla")]
        public async Task<IActionResult> ApproveLeaveRequest(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("İzin isteği ID'si gereklidir");
            }

            var response = await _leaveRequestApiService.ApproveLeaveRequestAsync(id, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpPut("{id}/reddet")]
        public async Task<IActionResult> RejectLeaveRequest(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("İzin isteği ID'si gereklidir");
            }

            var response = await _leaveRequestApiService.RejectLeaveRequestAsync(id, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}