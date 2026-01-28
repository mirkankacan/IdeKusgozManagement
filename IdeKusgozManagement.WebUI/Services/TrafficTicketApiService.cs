using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.TrafficTicketModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using System.Net.Http.Headers;

namespace IdeKusgozManagement.WebUI.Services
{
    public class TrafficTicketApiService : ITrafficTicketApiService
    {
        private readonly IApiService _apiService;
        private readonly ILogger<TrafficTicketApiService> _logger;
        private const string BaseEndpoint = "api/traffictickets";

        public TrafficTicketApiService(
            IApiService apiService,
            ILogger<TrafficTicketApiService> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<TrafficTicketViewModel>>> GetTrafficTicketsAsync(CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<TrafficTicketViewModel>>(BaseEndpoint, cancellationToken);
        }

        public async Task<ApiResponse<TrafficTicketViewModel>> GetTrafficTicketByIdAsync(string trafficTicketId, CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<TrafficTicketViewModel>($"{BaseEndpoint}/{trafficTicketId}", cancellationToken);
        }

        public async Task<ApiResponse<string>> CreateTrafficTicketAsync(CreateTrafficTicketViewModel model, CancellationToken cancellationToken = default)
        {
            using var formData = new MultipartFormDataContent();

            // Add form fields
            formData.Add(new StringContent(model.ProjectId), "ProjectId");
            formData.Add(new StringContent(model.EquipmentId), "EquipmentId");
            formData.Add(new StringContent(model.Type.ToString()), "Type");
            formData.Add(new StringContent(model.Amount.ToString()), "Amount");
            formData.Add(new StringContent(model.TicketDate.ToString("yyyy-MM-dd")), "TicketDate");

            if (!string.IsNullOrEmpty(model.TargetUserId) && model.Type == 1)
            {
                formData.Add(new StringContent(model.TargetUserId), "TargetUserId");
            }

            // Add file if exists
            if (model.File?.FormFile != null)
            {
                var fileContent = new StreamContent(model.File.FormFile.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(model.File.FormFile.ContentType);
                formData.Add(fileContent, "File.FormFile", model.File.FormFile.FileName);
            }

            return await _apiService.PostMultipartAsync<string>(BaseEndpoint, formData, cancellationToken);
        }

        public async Task<ApiResponse<bool>> UpdateTrafficTicketAsync(string trafficTicketId, UpdateTrafficTicketViewModel model, CancellationToken cancellationToken = default)
        {
            using var formData = new MultipartFormDataContent();

            // Add form fields
            formData.Add(new StringContent(model.ProjectId), "ProjectId");
            formData.Add(new StringContent(model.EquipmentId), "EquipmentId");
            formData.Add(new StringContent(model.Type.ToString()), "Type");
            formData.Add(new StringContent(model.Amount.ToString()), "Amount");
            formData.Add(new StringContent(model.TicketDate.ToString("yyyy-MM-dd")), "TicketDate");

            if (!string.IsNullOrEmpty(model.TargetUserId) && model.Type == 1)
            {
                formData.Add(new StringContent(model.TargetUserId), "TargetUserId");
            }

            // Add file if exists
            if (model.File?.FormFile != null)
            {
                var fileContent = new StreamContent(model.File.FormFile.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(model.File.FormFile.ContentType);
                formData.Add(fileContent, "File.FormFile", model.File.FormFile.FileName);
            }

            return await _apiService.PutMultipartAsync<bool>($"{BaseEndpoint}/{trafficTicketId}", formData, cancellationToken);
        }

        public async Task<ApiResponse<bool>> DeleteTrafficTicketAsync(string trafficTicketId, CancellationToken cancellationToken = default)
        {
            return await _apiService.DeleteAsync<bool>($"{BaseEndpoint}/{trafficTicketId}", cancellationToken);
        }
    }
}
