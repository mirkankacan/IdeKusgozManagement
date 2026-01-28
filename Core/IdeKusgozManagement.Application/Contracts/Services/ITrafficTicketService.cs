using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.TrafficTicketDTOs;

namespace IdeKusgozManagement.Application.Contracts.Services
{
    public interface ITrafficTicketService
    {
        Task<ServiceResult<IEnumerable<TrafficTicketDTO>>> GetTrafficTicketsAsync(CancellationToken cancellationToken = default);

        Task<ServiceResult<TrafficTicketDTO>> GetTrafficTicketByIdAsync(string trafficTicketId, CancellationToken cancellationToken = default);

        Task<ServiceResult<string>> CreateTrafficTicketAsync(CreateTrafficTicketDTO createTrafficTicketDTO, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> UpdateTrafficTicketAsync(string trafficTicketId, UpdateTrafficTicketDTO updateTrafficTicketDTO, CancellationToken cancellationToken = default);

        Task<ServiceResult<bool>> DeleteTrafficTicketAsync(string trafficTicketId, CancellationToken cancellationToken = default);
    }
}