using IdeKusgozManagement.Application.DTOs.EquipmentDTOs;
using IdeKusgozManagement.Application.Interfaces;
using IdeKusgozManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class EquipmentsController : ControllerBase
    {
        private readonly IEquipmentService _equipmentService;

        public EquipmentsController(IEquipmentService equipmentService)
        {
            _equipmentService = equipmentService;
        }

        /// <summary>
        /// Tüm ekipmanları getirir
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllEquipments(CancellationToken cancellationToken = default)
        {
            var result = await _equipmentService.GetAllEquipmentsAsync(cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// ID'ye göre ekipman getirir
        /// </summary>
        /// <param name="id">Ekipman ID'si</param>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEquipmentById(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Ekipman ID'si gereklidir");
            }

            var result = await _equipmentService.GetEquipmentByIdAsync(id, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Yeni ekipman oluşturur
        /// </summary>
        /// <param name="createEquipmentDTO">Ekipman bilgileri</param>
        [HttpPost]
        [RoleFilter("Admin", "Yönetici")]
        public async Task<IActionResult> CreateEquipment([FromBody] CreateEquipmentDTO createEquipmentDTO, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _equipmentService.CreateEquipmentAsync(createEquipmentDTO, cancellationToken);
            return result.IsSuccess ? CreatedAtAction(nameof(GetEquipmentById), new { id = result.Data }, result) : BadRequest(result);
        }

        /// <summary>
        /// Ekipman günceller
        /// </summary>
        /// <param name="id">Ekipman ID'si</param>
        /// <param name="updateEquipmentDTO">Güncellenecek ekipman bilgileri</param>
        [HttpPut("{id}")]
        [RoleFilter("Admin", "Yönetici")]
        public async Task<IActionResult> UpdateEquipment(string id, [FromBody] UpdateEquipmentDTO updateEquipmentDTO, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Ekipman ID'si gereklidir");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _equipmentService.UpdateEquipmentAsync(id, updateEquipmentDTO, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Ekipman siler
        /// </summary>
        /// <param name="id">Ekipman ID'si</param>
        [HttpDelete("{id}")]
        [RoleFilter("Admin", "Yönetici")]
        public async Task<IActionResult> DeleteEquipment(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Ekipman ID'si gereklidir");
            }

            var result = await _equipmentService.DeleteEquipmentAsync(id, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
