using IdeKusgozManagement.WebUI.Models.FileModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Controllers
{
    [Route("dosya")]
    public class FileController : Controller
    {
        private readonly IFileApiService _fileApiService;

        public FileController(IFileApiService fileApiService)
        {
            _fileApiService = fileApiService;
        }

        [HttpGet("indir/{id}")]
        public async Task<IActionResult> DownloadFile(string id, CancellationToken cancellationToken = default)
        {
            var result = await _fileApiService.DownloadFileAsync(id, cancellationToken);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return File(result.Data.FileStream, result.Data.ContentType, result.Data.FileDownloadName);
        }

        [HttpDelete("{fileId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFile(string fileId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(fileId))
            {
                return BadRequest("Dosya ID'si gereklidir");
            }
            var result = await _fileApiService.DeleteFileAsync(fileId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("yukle")]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(50 * 1024 * 1024)]
        public async Task<IActionResult> UploadFile([FromForm] List<UploadFileViewModel> files, CancellationToken cancellationToken = default)
        {
            if (!files.Any())
            {
                return BadRequest("Dosya(lar) yükleme zorunludur");
            }

            var result = await _fileApiService.UploadFileAsync(files, cancellationToken);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("yukle")]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpGet("liste/p")]
        public async Task<IActionResult> GetFilesByParams([FromQuery] string? userId, [FromQuery] string? documentType, [FromQuery] string? departmentId, CancellationToken cancellationToken = default)
        {
            var result = await _fileApiService.GetFilesByParamsAsync(userId, documentType, departmentId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }
    }
}