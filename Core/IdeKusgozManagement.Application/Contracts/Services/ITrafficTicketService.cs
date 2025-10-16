using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.TrafficTicketDTOs;

namespace IdeKusgozManagement.Application.Contracts.Services
{
    public interface ITrafficTicketService
    {
        Task<ApiResponse<IEnumerable<TrafficTicketDTO>>> GetTrafficTicketsAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<TrafficTicketDTO>> GetTrafficTicketByIdAsync(string trafficTicketId, CancellationToken cancellationToken = default);

        Task<ApiResponse<string>> CreateTrafficTicketAsync(CreateTrafficTicketDTO createTrafficTicketDTO, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> UpdateTrafficTicketAsync(string trafficTicketId, UpdateTrafficTicketDTO updateTrafficTicketDTO, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeleteTrafficTicketAsync(string trafficTicketId, CancellationToken cancellationToken = default);
    }
}