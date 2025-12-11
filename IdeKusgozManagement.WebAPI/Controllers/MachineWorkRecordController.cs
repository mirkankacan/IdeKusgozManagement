using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.MachineWorkRecordDTOs;
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
    public class MachineWorkRecordsController(IMachineWorkRecordService MachineWorkRecordService, IIdentityService identityService) : ControllerBase
    {
        [HttpGet("my-records/date/{date:datetime}")]
        public async Task<IActionResult> GetMyMachineWorkRecordsByDate(DateTime date, CancellationToken cancellationToken)
        {
            var currentUserId = identityService.GetUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return BadRequest("Kullanıcı kimliği bulunamadı");
            }

            var result = await MachineWorkRecordService.GetMachineWorkRecordsByUserIdAndDateAsync(currentUserId, date, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("approved/user/{userId}/date/{date:datetime}")]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> GetApprovedMachineWorkRecordsByUser(string userId, DateTime date, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si gereklidir");
            }
            var result = await MachineWorkRecordService.GetApprovedMachineWorkRecordsByUserAsync(userId, date, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("user/{userId}/date/{date:datetime}/status/{status:int}")]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> GetMachineWorkRecordsByUserIdDateStatus(string userId, DateTime date, WorkRecordStatus status, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si gereklidir");
            }
            var result = await MachineWorkRecordService.GetMachineWorkRecordsByUserIdDateStatusAsync(userId, date, status, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("user/{userId}/date/{date:datetime}")]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> GetMachineWorkRecordsByDateAndUser(string userId, DateTime date, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si gereklidir");
            }
            var result = await MachineWorkRecordService.GetMachineWorkRecordsByUserIdAndDateAsync(userId, date, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("batch-create-modify")]
        public async Task<IActionResult> BatchCreateOrModifyMachineWorkRecords([FromForm] List<CreateOrModifyMachineWorkRecordDTO> createMachineWorkRecordDTOs, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (createMachineWorkRecordDTOs == null || !createMachineWorkRecordDTOs.Any())
            {
                return BadRequest("İşlenecek kayıt bulunamadı");
            }
            var result = await MachineWorkRecordService.BatchCreateOrModifyMachineWorkRecordsAsync(createMachineWorkRecordDTOs, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("batch-update/user/{userId}")]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> BatchUpdateMachineWorkRecordByUser(string userId, [FromForm] List<CreateOrModifyMachineWorkRecordDTO> updateMachineWorkRecordDTO, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si gereklidir");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await MachineWorkRecordService.BatchUpdateMachineWorkRecordsByUserIdAsync(userId, updateMachineWorkRecordDTO, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("batch-approve/user/{userId}/date/{date:datetime}")]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> BatchApproveMachineWorkRecordByUserAndDate(string userId, DateTime date, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si gereklidir");
            }
            var result = await MachineWorkRecordService.BatchApproveMachineWorkRecordsByUserIdAndDateAsync(userId, date, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("batch-reject/user/{userId}/date/{date:datetime}")]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> BatchRejectMachineWorkRecordByUserAndDate(string userId, DateTime date, [FromQuery] string? rejectReason, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Kullanıcı ID'si gereklidir");
            }
            var result = await MachineWorkRecordService.BatchRejectMachineWorkRecordsByUserIdAndDateAsync(userId, date, rejectReason, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}/reject")]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> RejectMachineWorkRecordById(string id, [FromQuery] string? rejectReason, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Puantaj ID'si gereklidir");
            }
            var result = await MachineWorkRecordService.RejectMachineWorkRecordByIdAsync(id, rejectReason, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}/approve")]
        [RoleFilter("Admin", "Yönetici", "Şef")]
        public async Task<IActionResult> ApproveMachineWorkRecordById(string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Puantaj ID'si gereklidir");
            }
            var result = await MachineWorkRecordService.ApproveMachineWorkRecordByIdAsync(id, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}