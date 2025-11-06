using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.AIDTOs;

namespace IdeKusgozManagement.Application.Contracts.Services
{
    public interface IAIService
    {
        Task<ApiResponse<AIDateResponse>> AnalyzeDocumentDateAsync(byte[] documentBytes, string contentType, string documentTypeName);
    }
}