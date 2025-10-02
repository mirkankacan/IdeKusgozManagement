using IdeKusgozManagement.Application.DTOs.EquipmentDTOs;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class EquipmentsController(IEquipmentService equipmentService) : ControllerBase
    {
        /// <summary>
        /// Tüm ekipmanları getirir
        /// </summary>
        [HttpGet]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> GetEquipments(CancellationToken cancellationToken = default)
        {
            var result = await equipmentService.GetEquipmentsAsync(cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Aktif tüm ekipmanları getirir
        /// </summary>
        [HttpGet("active-equipments")]
        public async Task<IActionResult> GetActiveEquipments(CancellationToken cancellationToken = default)
        {
            var result = await equipmentService.GetActiveEquipmentsAsync(cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Ekipmanı aktifleştirir
        /// </summary>
        /// <param name="equipmentId">Ekipman ID'si</param>
        [RoleFilter("Admin", "Yönetici", "Şef")]
        [HttpPut("{equipmentId}/enable")]
        public async Task<IActionResult> EnableEquipment(string equipmentId)
        {
            if (string.IsNullOrWhiteSpace(equipmentId))
            {
                return BadRequest("Ekipman ID'si gereklidir");
            }
            var result = await equipmentService.EnableEquipmentAsync(equipmentId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Ekipmanı pasifleştirir
        /// </summary>
        /// <param name="equipmentId">Ekipman ID'si</param>
        [RoleFilter("Admin", "Yönetici", "Şef")]
        [HttpPut("{equipmentId}/disable")]
        public async Task<IActionResult> DisableEquipment(string equipmentId)
        {
            if (string.IsNullOrWhiteSpace(equipmentId))
            {
                return BadRequest("Ekipman ID'si gereklidir");
            }
            var result = await equipmentService.DisableEquipmentAsync(equipmentId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// ID'ye göre ekipman getirir
        /// </summary>
        /// <param name="equipmentId">Ekipman ID'si</param>
        [HttpGet("{equipmentId}")]
        public async Task<IActionResult> GetEquipmentById(string equipmentId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(equipmentId))
            {
                return BadRequest("Ekipman ID'si gereklidir");
            }

            var result = await equipmentService.GetEquipmentByIdAsync(equipmentId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Yeni ekipman oluşturur
        /// </summary>
        /// <param name="createEquipmentDTO">Ekipman bilgileri</param>
        [HttpPost]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> CreateEquipment([FromBody] CreateEquipmentDTO createEquipmentDTO, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await equipmentService.CreateEquipmentAsync(createEquipmentDTO, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Ekipman günceller
        /// </summary>
        /// <param name="equipmentId">Ekipman ID'si</param>
        /// <param name="updateEquipmentDTO">Güncellenecek ekipman bilgileri</param>
        [HttpPut("{equipmentId}")]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> UpdateEquipment(string equipmentId, [FromBody] UpdateEquipmentDTO updateEquipmentDTO, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(equipmentId))
            {
                return BadRequest("Ekipman ID'si gereklidir");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await equipmentService.UpdateEquipmentAsync(equipmentId, updateEquipmentDTO, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Ekipman siler
        /// </summary>
        /// <param name="equipmentId">Ekipman ID'si</param>
        [HttpDelete("{equipmentId}")]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> DeleteEquipment(string equipmentId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(equipmentId))
            {
                return BadRequest("Ekipman ID'si gereklidir");
            }

            var result = await equipmentService.DeleteEquipmentAsync(equipmentId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}