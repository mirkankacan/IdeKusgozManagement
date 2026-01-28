using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.DocumentDTOs;
using IdeKusgozManagement.Infrastructure.Authorization;
using IdeKusgozManagement.WebAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [RoleFilter("Admin", "Yönetici", "Şef")]
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController(IDocumentService documentService) : ControllerBase
    {
        /// <summary>
        /// Tüm doküman tiplerini getirir
        /// </summary>
        [HttpGet("types")]
        public async Task<IActionResult> GetDocumentTypes(CancellationToken cancellationToken)
        {
            var result = await documentService.GetDocumentTypesAsync(cancellationToken);
            return result.ToActionResult();
        }

        /// <summary>
        /// ID'ye göre doküman tipi getirir
        /// </summary>
        /// <param name="documentTypeId">Doküman tipi ID'si</param>
        [HttpGet("types/{documentTypeId}")]
        public async Task<IActionResult> GetDocumentTypeById(string documentTypeId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(documentTypeId))
            {
                return BadRequest("Doküman tipi ID'si gereklidir");
            }

            var result = await documentService.GetDocumentTypeByIdAsync(documentTypeId, cancellationToken);
            return result.ToActionResult();
        }

        /// <summary>
        /// Yeni doküman tipi oluşturur
        /// </summary>
        /// <param name="createDocumentTypeDTO">Doküman tipi bilgileri</param>
        [HttpPost("types")]
        public async Task<IActionResult> CreateDocumentType([FromBody] CreateDocumentTypeDTO createDocumentTypeDTO, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await documentService.CreateDocumentTypeAsync(createDocumentTypeDTO, cancellationToken);
            return result.ToActionResult();
        }

        /// <summary>
        /// Doküman tipi günceller
        /// </summary>
        /// <param name="documentTypeId">Doküman tipi ID'si</param>
        /// <param name="updateDocumentTypeDTO">Güncellenecek doküman tipi bilgileri</param>
        [HttpPut("types/{documentTypeId}")]
        public async Task<IActionResult> UpdateDocumentType(string documentTypeId, [FromBody] UpdateDocumentTypeDTO updateDocumentTypeDTO, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(documentTypeId))
            {
                return BadRequest("Doküman tipi ID'si gereklidir");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await documentService.UpdateDocumentTypeAsync(documentTypeId, updateDocumentTypeDTO, cancellationToken);
            return result.ToActionResult();
        }

        /// <summary>
        /// Doküman tipi siler
        /// </summary>
        /// <param name="documentTypeId">Doküman tipi ID'si</param>
        [HttpDelete("types/{documentTypeId}")]
        public async Task<IActionResult> DeleteDocumentType(string documentTypeId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(documentTypeId))
            {
                return BadRequest("Doküman tipi ID'si gereklidir");
            }

            var result = await documentService.DeleteDocumentTypeAsync(documentTypeId, cancellationToken);
            return result.ToActionResult();
        }

        [HttpGet("{departmentDutyId}/types")]
        public async Task<IActionResult> GetDocumentTypesByDuty(string departmentDutyId,/* [FromQuery] string? companyId, */CancellationToken cancellationToken)
        {
            var result = await documentService.GetDocumentTypesByDutyAsync(departmentDutyId, /*companyId,*/ cancellationToken);
            return result.ToActionResult();
        }

        /// <summary>
        /// Departman doküman gerekliliği oluşturur
        /// </summary>
        /// <param name="createDTO">Departman doküman gerekliliği bilgileri</param>
        [HttpPost("requirements")]
        public async Task<IActionResult> CreateDepartmentDocumentRequirment([FromBody] CreateDepartmentDocumentRequirmentDTO createDTO, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await documentService.CreateDepartmentDocumentRequirmentAsync(createDTO, cancellationToken);
            return result.ToActionResult();
        }

        /// <summary>
        /// Tüm departman doküman gerekliliklerini getirir
        /// </summary>
        [HttpGet("requirements")]
        public async Task<IActionResult> GetDepartmentDocumentRequirments(CancellationToken cancellationToken)
        {
            var result = await documentService.GetDepartmentDocumentRequirmentsAsync(cancellationToken);
            return result.ToActionResult();
        }

        /// <summary>
        /// Departman doküman gerekliliği siler
        /// </summary>
        /// <param name="requirementId">Gereklilik ID'si</param>
        [HttpDelete("requirements/{requirementId}")]
        public async Task<IActionResult> DeleteDepartmentDocumentRequirment(string requirementId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(requirementId))
            {
                return BadRequest("Gereklilik ID'si gereklidir");
            }

            var result = await documentService.DeleteDepartmentDocumentRequirmentAsync(requirementId, cancellationToken);
            return result.ToActionResult();
        }

        [HttpGet("check")]
        public async Task<IActionResult> GetRequiredDocuments([FromQuery] string departmentId, [FromQuery] string departmentDutyId, [FromQuery] string? companyId, [FromQuery] string? targetId, [FromQuery] string? documentTypeId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(departmentId))
                return BadRequest("Departman ID'si gereklidir");
            if (string.IsNullOrEmpty(departmentDutyId))
                return BadRequest("Departman görev ID'si gereklidir");

            var result = await documentService.GetRequiredDocumentsAsync(departmentId, departmentDutyId, companyId, targetId, documentTypeId, cancellationToken);
            return result.ToActionResult();
        }
    }
}