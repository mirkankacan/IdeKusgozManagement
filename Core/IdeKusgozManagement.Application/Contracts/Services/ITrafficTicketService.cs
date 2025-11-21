using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.TrafficTicketDTOs;

namespace IdeKusgozManagement.Application.Contracts.Services
{
    public interface ITrafficTicketService
    {
        Task<ServiceResponse<IEnumerable<TrafficTicketDTO>>> GetTrafficTicketsAsync(CancellationToken cancellationToken = default);

        Task<ServiceResponse<TrafficTicketDTO>> GetTrafficTicketByIdAsync(string trafficTicketId, CancellationToken cancellationToken = default);

        Task<ServiceResponse<string>> CreateTrafficTicketAsync(CreateTrafficTicketDTO createTrafficTicketDTO, CancellationToken cancellationToken = default);

        Task<ServiceResponse<bool>> UpdateTrafficTicketAsync(string trafficTicketId, UpdateTrafficTicketDTO updateTrafficTicketDTO, CancellationToken cancellationToken = default);

        Task<ServiceResponse<bool>> DeleteTrafficTicketAsync(string trafficTicketId, CancellationToken cancellationToken = default);
    }
}