using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.ParameterModels;

namespace IdeKusgozManagement.WebUI.Services.Interfaces
{
    public interface IParameterApiService
    {
        Task<ApiResponse<IEnumerable<ParameterViewModel>>> GetParametersAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<ParameterViewModel>> GetParameterByKeyAsync(string key, CancellationToken cancellationToken = default);

        Task<ApiResponse<string>> CreateParameterAsync(CreateParameterViewModel model, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> UpdateParameterAsync(string id, UpdateParameterViewModel model, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeleteParameterAsync(string id, CancellationToken cancellationToken = default);
    }
}