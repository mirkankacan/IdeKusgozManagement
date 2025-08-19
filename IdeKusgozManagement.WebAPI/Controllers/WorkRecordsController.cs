using IdeKusgozManagement.Application.DTOs.WorkRecordDTOs;
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
        [HttpGet]
        [RoleFilter("Admin,Yönetici,Şef")]
        public IActionResult GetAllWorkRecords(CancellationToken cancellationToken = default)
        {
            return Ok();
        }

        [HttpGet("{id}")]
        [RoleFilter("Admin,Yönetici,Şef")]
        public IActionResult GetWorkRecordById(string id, CancellationToken cancellationToken = default)
        {
            return Ok();
        }

        [HttpGet("by-user/{userId}")]
        [RoleFilter("Admin,Yönetici,Şef")]
        public async Task<IActionResult> GetWorkRecordsByUserIdDate(string userId, DateTime? date, CancellationToken cancellationToken = default)
        {
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CreateWorkRecord([FromBody] CreateWorkRecordDTO createWorkRecordDTO, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Created();
        }

        [HttpPost("batch")]
        public async Task<IActionResult> BatchCreateWorkRecord([FromBody] CreateWorkRecordDTO createWorkRecordDTO, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Created();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWorkRecord(string id, [FromBody] UpdateWorkRecordDTO updateWorkRecordDTO, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok();
        }

        [HttpPut("expense/{id}")]
        public async Task<IActionResult> UpdateWorkRecordExpense(string id, [FromBody] UpdateWorkRecordExpenseDTO updateWorkRecordExpenseDTO, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok();
        }

        [HttpDelete("{id}")]
        [RoleFilter("Admin,Yönetici,Şef")]
        public async Task<IActionResult> DeleteWorkRecord(string id, CancellationToken cancellationToken = default)
        {
            return Ok();
        }

        [HttpDelete("expense/{id}")]
        public async Task<IActionResult> DeleteWorkRecordExpense(string id, CancellationToken cancellationToken = default)
        {
            return Ok();
        }
    }
}