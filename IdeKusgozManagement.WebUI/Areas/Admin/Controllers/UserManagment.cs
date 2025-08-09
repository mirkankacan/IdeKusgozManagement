using IdeKusgozManagement.WebUI.Models.UserModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Route("admin/kullanici-yonetimi")]
    public class UserManagment : Controller
    {
        private readonly IUserApiService _userApiService;

        public UserManagment(IUserApiService userApiService)
        {
            _userApiService = userApiService;
        }

        [HttpGet("")]
        [HttpGet("liste")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("liste-datatable")]
        public async Task<IActionResult> GetUsersDataTable(CancellationToken cancellationToken = default)
        {
            var response = await _userApiService.GetAllUsersAsync(cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpPost("olustur")]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model, CancellationToken cancellationToken = default)
        {
            var response = await _userApiService.CreateUserAsync(model, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpPost("rol-ata")]
        public async Task<IActionResult> AssignRoleToUser(AssignRoleViewModel model, CancellationToken cancellationToken = default)
        {
            var response = await _userApiService.AssignRoleToUserAsync(model, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpGet("kullanici/i/{userId}")]
        public async Task<IActionResult> GetUserById(string userId, CancellationToken cancellationToken)
        {
            var response = await _userApiService.GetUserByIdAsync(userId, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("sil/i/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId, CancellationToken cancellationToken)
        {
            var response = await _userApiService.DeleteUserAsync(userId, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpPost("aktif/i/{userId}")]
        public async Task<IActionResult> ActivateUser(string userId, CancellationToken cancellationToken = default)
        {
            var response = await _userApiService.ActivateUserAsync(userId, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpPost("inaktif/i/{userId}")]
        public async Task<IActionResult> DeactivateUser(string userId, CancellationToken cancellationToken = default)
        {
            var response = await _userApiService.DeactivateUserAsync(userId, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpPut("guncelle/i/{userId}")]
        public async Task<IActionResult> UpdateUser(string userId, UpdateUserViewModel model, CancellationToken cancellationToken = default)
        {
            var response = await _userApiService.UpdateUserAsync(userId, model, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}