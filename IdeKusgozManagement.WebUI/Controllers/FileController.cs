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
    }
}