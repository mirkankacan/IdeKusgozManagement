using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.DeviceDTOs;

namespace IdeKusgozManagement.Application.Contracts.Services
{
    public interface IDeviceService
    {
        Task<ServiceResult<bool>> RegisterDeviceAsync(RegisterDeviceDTO dto, CancellationToken cancellationToken = default);
    }
}