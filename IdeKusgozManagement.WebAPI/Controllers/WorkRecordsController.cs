// IdeKusgozManagement.WebAPI/Controllers/WorkRecordsController.cs
using System.Security.Claims;
using IdeKusgozManagement.Application.DTOs.WorkRecordDTOs;
using IdeKusgozManagement.Application.Interfaces;
using IdeKusgozManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class WorkRecordsController : ControllerBase
    {
        private readonly IWorkRecordService _workRecordService;

        public WorkRecordsController(IWorkRecordService workRecordService)
        {
            _workRecordService = workRecordService;
        }

        /// <summary>
        /// Tüm iş kayıtlarını getirir (Sadece yetkili kullanıcılar)
        /// </summary>
        [HttpGet]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> GetAllWorkRecords(CancellationToken cancellationToken = default)
        {
            var result = await _workRecordService.GetAllWorkRecordsAsync();
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// ID'ye göre iş kaydı getirir
        /// </summary>
        /// <param name="id">İş kaydı ID'si</param>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWorkRecordById(string id, CancellationToken cancellationToken = default)
        {
            var result = await _workRecordService.GetWorkRecordByIdAsync(id);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Mevcut kullanıcının iş kayıtlarını getirir
        /// </summary>
        [HttpGet("my-records")]
        public async Task<IActionResult> GetMyWorkRecords(CancellationToken cancellationToken = default)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {
                return BadRequest("Kullanıcı kimliği bulunamadı");
            }

            var result = await _workRecordService.GetWorkRecordsByUserIdAsync(currentUserId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Tarih aralığına göre iş kayıtlarını getirir
        /// </summary>
        /// <param name="date">Tarihi</param>
        /// <param name="userId">Kullanıcı ID'si</param>
        [HttpGet("by-date-and-user")]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> GetWorkRecordsByDateAndUser(
            [FromQuery] DateTime date,
            [FromQuery] string userId,
            CancellationToken cancellationToken = default)
        {
            var result = await _workRecordService.GetWorkRecordsByDateAndUserAsync(date, userId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Yeni iş kaydı oluşturur
        /// </summary>
        /// <param name="createWorkRecordDTO">İş kaydı bilgileri</param>
        [HttpPost]
        public async Task<IActionResult> CreateWorkRecord([FromBody] CreateWorkRecordDTO createWorkRecordDTO, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _workRecordService.CreateWorkRecordAsync(createWorkRecordDTO);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Toplu iş kaydı oluşturur
        /// </summary>
        /// <param name="createWorkRecordDTOs">İş kaydı listesi</param>
        [HttpPost("batch")]
        public async Task<IActionResult> BatchCreateWorkRecord([FromBody] List<CreateWorkRecordDTO> createWorkRecordDTOs, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (createWorkRecordDTOs == null || !createWorkRecordDTOs.Any())
            {
                return BadRequest("İş kaydı listesi boş olamaz");
            }

            var result = await _workRecordService.BatchCreateWorkRecordsAsync(createWorkRecordDTOs);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// İş kaydını günceller
        /// </summary>
        /// <param name="id">İş kaydı ID'si</param>
        /// <param name="updateWorkRecordDTO">Güncellenecek bilgiler</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWorkRecord(string id, [FromBody] UpdateWorkRecordDTO updateWorkRecordDTO, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _workRecordService.UpdateWorkRecordAsync(id, updateWorkRecordDTO);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// İş kaydını onaylar (Sadece yetkili kullanıcılar)
        /// </summary>
        /// <param name="id">İş kaydı ID'si</param>
        [HttpPost("approve/{id}")]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> ApproveWorkRecord(string id, CancellationToken cancellationToken = default)
        {
            var result = await _workRecordService.ApproveWorkRecordAsync(id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// İş kaydını reddeder (Sadece yetkili kullanıcılar)
        /// </summary>
        /// <param name="id">İş kaydı ID'si</param>
        [HttpPost("reject/{id}")]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> RejectWorkRecord(string id, CancellationToken cancellationToken = default)
        {
            var result = await _workRecordService.RejectWorkRecordAsync(id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}