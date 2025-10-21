using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.TrafficTicketModels;

namespace IdeKusgozManagement.WebUI.Services.Interfaces
{
    public interface ITrafficTicketApiService
    {
        Task<ApiResponse<IEnumerable<TrafficTicketViewModel>>> GetTrafficTicketsAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<TrafficTicketViewModel>> GetTrafficTicketByIdAsync(string trafficTicketId, CancellationToken cancellationToken = default);

        Task<ApiResponse<string>> CreateTrafficTicketAsync(CreateTrafficTicketViewModel model, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> UpdateTrafficTicketAsync(string trafficTicketId, UpdateTrafficTicketViewModel model, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeleteTrafficTicketAsync(string trafficTicketId, CancellationToken cancellationToken = default);
    }
}