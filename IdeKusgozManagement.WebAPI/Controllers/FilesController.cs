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
        [HttpPost]
        public async Task<IActionResult> UploadFile([FromForm] UploadFileDTO uploadFileDTO, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await fileService.UploadFileAsync(uploadFileDTO, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}