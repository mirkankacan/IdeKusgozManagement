using IdeKusgozManagement.WebUI.Models.EquipmentModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Controllers
{
    [Route("ekipman-yonetimi")]
    public class EquipmentManagementController : Controller
    {
        private readonly IEquipmentApiService _equipmentApiService;

        public EquipmentManagementController(IEquipmentApiService equipmentApiService)
        {
            _equipmentApiService = equipmentApiService;
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet("liste")]
        public async Task<IActionResult> GetEquipments(CancellationToken cancellationToken = default)
        {
            var response = await _equipmentApiService.GetAllEquipmentsAsync(cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize]
        [HttpGet("aktif-liste")]
        public async Task<IActionResult> GetActiveEquipments(CancellationToken cancellationToken = default)
        {
            var response = await _equipmentApiService.GetAllActiveEquipmentsAsync(cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEquipmentById(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Ekipman ID'si gereklidir");
            }

            var response = await _equipmentApiService.GetEquipmentByIdAsync(id, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpPost("")]
        public async Task<IActionResult> CreateEquipment([FromBody] CreateEquipmentViewModel model, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _equipmentApiService.CreateEquipmentAsync(model, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEquipment(string id, [FromBody] UpdateEquipmentViewModel model, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Ekipman ID'si gereklidir");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _equipmentApiService.UpdateEquipmentAsync(id, model, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEquipment(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Ekipman ID'si gereklidir");
            }

            var response = await _equipmentApiService.DeleteEquipmentAsync(id, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpPut("{id}/aktif-et")]
        public async Task<IActionResult> ActivateEquipment(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Ekipman ID'si gereklidir");
            }

            var response = await _equipmentApiService.ActivateEquipmentAsync(id, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = "Admin, Yönetici")]
        [HttpPut("{id}/pasif-et")]
        public async Task<IActionResult> DeactivateEquipment(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Ekipman ID'si gereklidir");
            }

            var response = await _equipmentApiService.DeactivateEquipmentAsync(id, cancellationToken);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}
