using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.FileDTOs;
using IdeKusgozManagement.WebAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController(IFileService fileService) : ControllerBase
    {
        [RequestSizeLimit(100 * 1024 * 1024)]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] List<UploadFileDTO> files, CancellationToken cancellationToken)
        {
            if (!files.Any())
            {
                return BadRequest("Dosya(lar) seçilmedi veya boş dosya");
            }

            var result = await fileService.UploadFileAsync(files, cancellationToken);
            return result.ToActionResult();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFileById(string id, CancellationToken cancellationToken)
        {
            var result = await fileService.GetFileByIdAsync(id, cancellationToken);
            return result.ToActionResult();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(string id, CancellationToken cancellationToken)
        {
            var result = await fileService.DeleteFileAsync(id, cancellationToken);
            return result.ToActionResult();
        }

        [HttpGet("by-params")]
        public async Task<IActionResult> GetFilesByParams([FromQuery] string? userId, [FromQuery] string? documentType, [FromQuery] string? departmentId, CancellationToken cancellationToken)
        {
            var result = await fileService.GetFilesByParamsAsync(userId, documentType, departmentId, cancellationToken);
            return result.ToActionResult();
        }

        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadFile(string id, CancellationToken cancellationToken)
        {
            var result = await fileService.GetFileStreamByIdAsync(id, cancellationToken);
            if (result.IsSuccess)
            {
                return File(result.Data.fileStream, result.Data.contentType, result.Data.originalName);
            }
            return result.ToActionResult();
        }
    }
}