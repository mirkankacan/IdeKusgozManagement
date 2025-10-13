using IdeKusgozManagement.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace IdeKusgozManagement.Application.DTOs.FileDTOs
{
    public class UploadFileDTO
    {
        public IFormFile FormFile { get; set; }
        public FileType FileType { get; set; }
        public string? TargetUserId { get; set; }
    }
}