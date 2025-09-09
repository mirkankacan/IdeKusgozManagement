using IdeKusgozManagement.Application.DTOs.WorkRecordDTOs;
using IdeKusgozManagement.Application.Interfaces.Services;
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
        private readonly ICurrentUserService _currentUserService;

        public WorkRecordsController(IWorkRecordService workRecordService, ICurrentUserService currentUserService)
        {
            _workRecordService = workRecordService;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Mevcut kullanıcının iş kayıtlarını getirir
        /// </summary>
        [HttpGet("my-records-by-date")]
        public async Task<IActionResult> GetMyWorkRecordsByDate([FromQuery] DateTime date, CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUserService.GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return BadRequest("Kullanıcı kimliği bulunamadı");
            }

            var result = await _workRecordService.GetWorkRecordsByUserIdAndDateAsync(currentUserId, date, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Tarih ve kullanıcıya göre iş kayıtlarını getirir
        /// </summary>
        /// <param name="date">Tarih</param>
        /// <param name="userId">Kullanıcı ID'si</param>
        [HttpGet("by-user-and-date")]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> GetWorkRecordsByDateAndUser([FromQuery] string userId, [FromQuery] DateTime date, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si gereklidir");
            }
            var result = await _workRecordService.GetWorkRecordsByUserIdAndDateAsync(userId, date, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Toplu iş kaydı oluşturur
        /// </summary>
        /// <param name="createWorkRecordDTOs">İş kaydı listesi</param>
        [HttpPost("batch-create")]
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

            var result = await _workRecordService.BatchCreateWorkRecordsAsync(createWorkRecordDTOs, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Toplu iş kaydını günceller
        /// </summary>
        /// <param name="userId">Kullanıcı ID'si</param>
        /// <param name="updateWorkRecordDTO">Güncellenecek bilgiler</param>
        [HttpPut]
        public async Task<IActionResult> BatchUpdateWorkRecordByUser([FromQuery] string userId, [FromBody] List<UpdateWorkRecordDTO> updateWorkRecordDTO, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si gereklidir");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _workRecordService.BatchUpdateWorkRecordsByUserIdAsync(userId, updateWorkRecordDTO, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// İş kaydını onaylar
        /// </summary>
        /// <param name="userId">Kullanıcı ID'si</param>
        /// <param name="date">Tarih</param>
        [HttpPost("approve")]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> BatchApproveWorkRecordByUserAndDate([FromQuery] string userId, [FromQuery] DateTime date, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si gereklidir");
            }
            var result = await _workRecordService.BatchApproveWorkRecordsByUserIdAndDateAsync(userId, date, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// İş kaydını reddeder
        /// </summary>
        /// <param name="userId">Kullanıcı ID'si</param>
        /// <param name="date">Tarih</param>
        [HttpPost("reject")]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> BatchRejectWorkRecordByUserAndDate([FromQuery] string userId, [FromQuery] DateTime date, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si gereklidir");
            }
            var result = await _workRecordService.BatchRejectWorkRecordsByUserIdAndDateAsync(userId, date, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}