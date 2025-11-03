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
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] UploadFileDTO uploadFileDTO, CancellationToken cancellationToken = default)
        {
            if (uploadFileDTO.FormFile == null || uploadFileDTO.FormFile.Length == 0)
            {
                return BadRequest("Dosya seçilmedi veya boş dosya");
            }

            var result = await fileService.UploadFileAsync(uploadFileDTO, cancellationToken);
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

        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadFile(string id, CancellationToken cancellationToken = default)
        {
            var result = await fileService.GetFileByIdAsync(id, cancellationToken);
            if (!result.IsSuccess || result.Data == null)
            {
                return NotFound(result);
            }

            return File(result.Data.FileStream, result.Data.ContentType, result.Data.OriginalName);
        }
    }
}