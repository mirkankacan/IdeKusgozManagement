using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.ParameterDTOs;

namespace IdeKusgozManagement.Application.Contracts.Services
{
    public interface IParameterService
    {
        Task<ServiceResult<IEnumerable<ParameterDTO>>> GetParametersAsync(CancellationToken cancellationToken = default);

        Task<ServiceResult<ParameterDTO>> GetParameterByKeyAsync(string key, CancellationToken cancellationToken = default);

        Task<ServiceResult<string>> CreateParameterAsync(CreateParameterDTO dto, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> UpdateParameterAsync(string id, UpdateParameterDTO dto, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> DeleteParameterAsync(string id, CancellationToken cancellationToken = default);
    }
}