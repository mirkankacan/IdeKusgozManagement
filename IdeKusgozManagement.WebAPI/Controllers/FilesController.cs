using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.FileDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController(IFileService fileService) : ControllerBase
    {
        [RequestSizeLimit(50 * 1024 * 1024)]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] List<UploadFileDTO> files, CancellationToken cancellationToken = default)
        {
            if (!files.Any())
            {
                return BadRequest("Dosya(lar) seçilmedi veya boş dosya");
            }

            var result = await fileService.UploadFileAsync(files, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFileById(string id, CancellationToken cancellationToken = default)
        {
            var result = await fileService.GetFileByIdAsync(id, cancellationToken);
            if (!result.IsSuccess || result.Data == null)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(string id, CancellationToken cancellationToken = default)
        {
            var result = await fileService.DeleteFileAsync(id, cancellationToken);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        [HttpGet("by-params")]
        public async Task<IActionResult> GetFilesByParams([FromQuery] string? userId, [FromQuery] string? documentType, [FromQuery] string? departmentId, CancellationToken cancellationToken = default)
        {
            var result = await fileService.GetFilesByParamsAsync(userId, documentType, departmentId, cancellationToken);
            if (!result.IsSuccess || result.Data == null)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadFile(string id, CancellationToken cancellationToken = default)
        {
            var result = await fileService.GetFileStreamByIdAsync(id, cancellationToken);
            return result.IsSuccess ? File(result.Data.fileStream, result.Data.contentType, result.Data.originalName) : BadRequest(result);
        }
    }
}