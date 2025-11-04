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
        public async Task<IActionResult> DownloadFile(string id)
        {
            var result = await _fileApiService.DownloadFileAsync(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return File(result.Data.FileStream, result.Data.ContentType, result.Data.FileDownloadName);
        }

        [HttpDelete("{fileId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFile(string fileId)
        {
            if (string.IsNullOrEmpty(fileId))
            {
                return BadRequest("Dosya ID'si gereklidir");
            }
            var result = await _fileApiService.DeleteFileAsync(fileId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("yukle")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadFile([FromForm] List<UploadFileViewModel> files)
        {
            if (!files.Any())
            {
                return BadRequest("Dosya(lar) yükleme zorunludur");
            }

            var result = await _fileApiService.UploadFileAsync(files);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("yukle")]
        public IActionResult Upload()
        {
            return View();
        }
    }
}