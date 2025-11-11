using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.AIDTOs;
using Microsoft.AspNetCore.Http;

namespace IdeKusgozManagement.Application.Contracts.Services
{
    public interface IAIService
    {
        Task<ApiResponse<AIDateResponse>> AnalyzeDocumentDateAsync(IFormFile file, string documentTypeName, CancellationToken cancellationToken = default);
    }
}