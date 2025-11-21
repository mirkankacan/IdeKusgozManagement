using IdeKusgozManagement.WebUI.Models.EquipmentModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Controllers
{
    [Route("ekipman")]
    public class EquipmentController : Controller
    {
        private readonly IEquipmentApiService _equipmentApiService;

        public EquipmentController(IEquipmentApiService equipmentApiService)
        {
            _equipmentApiService = equipmentApiService;
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [HttpGet("liste")]
        public async Task<IActionResult> GetEquipments(CancellationToken cancellationToken)
        {
            var response = await _equipmentApiService.GetEquipmentsAsync(cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize]
        [HttpGet("aktif-liste")]
        public async Task<IActionResult> GetActiveEquipments(CancellationToken cancellationToken)
        {
            var response = await _equipmentApiService.GetActiveEquipmentsAsync(cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize]
        [HttpGet("{equipmentId}")]
        public async Task<IActionResult> GetEquipmentById(string equipmentId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(equipmentId))
            {
                return BadRequest("Ekipman ID'si gereklidir");
            }

            var response = await _equipmentApiService.GetEquipmentByIdAsync(equipmentId, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [ValidateAntiForgeryToken]
        [HttpPost("")]
        public async Task<IActionResult> CreateEquipment([FromBody] CreateEquipmentViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _equipmentApiService.CreateEquipmentAsync(model, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [ValidateAntiForgeryToken]
        [HttpPut("{equipmentId}")]
        public async Task<IActionResult> UpdateEquipment(string equipmentId, [FromBody] UpdateEquipmentViewModel model, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(equipmentId))
            {
                return BadRequest("Ekipman ID'si gereklidir");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _equipmentApiService.UpdateEquipmentAsync(equipmentId, model, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [ValidateAntiForgeryToken]
        [HttpDelete("{equipmentId}")]
        public async Task<IActionResult> DeleteEquipment(string equipmentId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(equipmentId))
            {
                return BadRequest("Ekipman ID'si gereklidir");
            }

            var response = await _equipmentApiService.DeleteEquipmentAsync(equipmentId, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [ValidateAntiForgeryToken]
        [HttpPut("{equipmentId}/aktif-et")]
        public async Task<IActionResult> EnableEquipment(string equipmentId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(equipmentId))
            {
                return BadRequest("Ekipman ID'si gereklidir");
            }

            var response = await _equipmentApiService.EnableEquipmentAsync(equipmentId, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici, Şef")]
        [ValidateAntiForgeryToken]
        [HttpPut("{equipmentId}/pasif-et")]
        public async Task<IActionResult> DisableEquipment(string equipmentId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(equipmentId))
            {
                return BadRequest("Ekipman ID'si gereklidir");
            }

            var response = await _equipmentApiService.DisableEquipmentAsync(equipmentId, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}