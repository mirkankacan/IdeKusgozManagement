using IdeKusgozManagement.Application.DTOs.WorkRecordDTOs;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Domain.Enums;
using IdeKusgozManagement.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class WorkRecordsController(IWorkRecordService workRecordService, IIdentityService identityService) : ControllerBase
    {
        /// <summary>
        /// Oturumdaki kullanıcının belirtilen ay ve yıldaki puantaj kayıtlarını getirir
        /// </summary>
        /// <param name="date">Tarih</param>

        [HttpGet("my-records/date/{date:datetime}")]
        public async Task<IActionResult> GetMyWorkRecordsByDate(DateTime date, CancellationToken cancellationToken)
        {
            var currentUserId = identityService.GetUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return BadRequest("Kullanıcı kimliği bulunamadı");
            }

            var result = await workRecordService.GetWorkRecordsByUserIdAndDateAsync(currentUserId, date, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Tarih, kullanıcıya göre onaylanmış puantajları getirir
        /// </summary>
        /// <param name="date">Tarih</param>
        /// <param name="userId">Kullanıcı ID'si</param>
        [HttpGet("approved/user/{userId}/date/{date:datetime}")]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> GetApprovedWorkRecordsByUser(string userId, DateTime date, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si gereklidir");
            }
            var result = await workRecordService.GetApprovedWorkRecordsByUserAsync(userId, date, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Tarih,kullanıcıya ve puantaj durumuna göre puantaj kayıtlarını getirir
        /// </summary>
        /// <param name="date">Tarih</param>
        /// <param name="userId">Kullanıcı ID'si</param>
        /// <param name="status">Puantaj durumu</param>
        [HttpGet("user/{userId}/date/{date:datetime}/status/{status:int}")]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> GetWorkRecordsByUserIdDateStatus(string userId, DateTime date, WorkRecordStatus status, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si gereklidir");
            }
            var result = await workRecordService.GetWorkRecordsByUserIdDateStatusAsync(userId, date, status, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Tarih ve kullanıcıya göre puantaj kayıtlarını getirir
        /// </summary>
        /// <param name="date">Tarih</param>
        /// <param name="userId">Kullanıcı ID'si</param>
        [HttpGet("user/{userId}/date/{date:datetime}")]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> GetWorkRecordsByDateAndUser(string userId, DateTime date, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si gereklidir");
            }
            var result = await workRecordService.GetWorkRecordsByUserIdAndDateAsync(userId, date, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Toplu puantaj kaydı oluşturur
        /// </summary>
        /// <param name="createWorkRecordDTOs">Puantaj kaydı listesi</param>
        [HttpPost("batch-create-modify")]
        public async Task<IActionResult> BatchCreateOrModifyWorkRecords([FromForm] List<CreateOrModifyWorkRecordDTO> createWorkRecordDTOs, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (createWorkRecordDTOs == null || !createWorkRecordDTOs.Any())
            {
                return BadRequest("İşlenecek kayıt bulunamadı");
            }
            var result = await workRecordService.BatchCreateOrModifyWorkRecordsAsync(createWorkRecordDTOs, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Toplu puantaj kayıtlarını günceller
        /// </summary>
        /// <param name="userId">Kullanıcı ID'si</param>
        /// <param name="updateWorkRecordDTO">Güncellenecek bilgiler</param>
        [HttpPut("batch-update/user/{userId}")]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> BatchUpdateWorkRecordByUser(string userId, [FromForm] List<CreateOrModifyWorkRecordDTO> updateWorkRecordDTO, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si gereklidir");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await workRecordService.BatchUpdateWorkRecordsByUserIdAsync(userId, updateWorkRecordDTO, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Puantajları toplu onaylar
        /// </summary>
        /// <param name="userId">Kullanıcı ID'si</param>
        /// <param name="date">Tarih</param>
        [HttpPut("batch-approve/user/{userId}/date/{date:datetime}")]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> BatchApproveWorkRecordByUserAndDate(string userId, DateTime date, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si gereklidir");
            }
            var result = await workRecordService.BatchApproveWorkRecordsByUserIdAndDateAsync(userId, date, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Puantajları toplu reddeder
        /// </summary>
        /// <param name="userId">Kullanıcı ID'si</param>
        /// <param name="date">Tarih</param>
        [HttpPut("batch-reject/user/{userId}/date/{date:datetime}")]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> BatchRejectWorkRecordByUserAndDate(string userId, DateTime date, [FromQuery] string? rejectReason, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si gereklidir");
            }
            var result = await workRecordService.BatchRejectWorkRecordsByUserIdAndDateAsync(userId, date, rejectReason, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Puantajı reddeder
        /// </summary>
        /// <param name="id">Puantaj ID'si</param>
        [HttpPut("{id}/reject")]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> RejectWorkRecordById(string id, [FromQuery] string? rejectReason, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Puantaj ID'si gereklidir");
            }
            var result = await workRecordService.RejectWorkRecordByIdAsync(id, rejectReason, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Puantajı onaylar
        /// </summary>
        /// <param name="id">Puantaj ID'si</param>
        [HttpPut("{id}/approve")]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> ApproveWorkRecordById(string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Puantaj ID'si gereklidir");
            }
            var result = await workRecordService.ApproveWorkRecordByIdAsync(id, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}