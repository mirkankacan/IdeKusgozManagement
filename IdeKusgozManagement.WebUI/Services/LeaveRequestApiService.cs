using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.LeaveRequestModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using System.Net.Http.Headers;

namespace IdeKusgozManagement.WebUI.Services
{
    public class LeaveRequestApiService : ILeaveRequestApiService
    {
        private readonly IApiService _apiService;
        private readonly ILogger<LeaveRequestApiService> _logger;
        private const string BaseEndpoint = "api/leaveRequests";

        public LeaveRequestApiService(
            IApiService apiService,
            ILogger<LeaveRequestApiService> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<LeaveRequestViewModel>>> GetSubordinateLeaveRequestsAsync(CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<LeaveRequestViewModel>>($"{BaseEndpoint}/subordinates", cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<LeaveRequestViewModel>>> GetLeaveRequestsAsync(CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<LeaveRequestViewModel>>(BaseEndpoint, cancellationToken);
        }

        public async Task<ApiResponse<LeaveRequestViewModel>> GetLeaveRequestByIdAsync(string leaveRequestId, CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<LeaveRequestViewModel>($"{BaseEndpoint}/{leaveRequestId}", cancellationToken);
        }

        public async Task<ApiResponse<LeaveRequestViewModel>> CreateLeaveRequestAsync(CreateLeaveRequestViewModel model, CancellationToken cancellationToken = default)
        {
            using var formData = new MultipartFormDataContent();

            // Add form fields
            formData.Add(new StringContent(model.StartDate.ToString("yyyy-MM-dd")), "StartDate");
            formData.Add(new StringContent(model.EndDate.ToString("yyyy-MM-dd")), "EndDate");
            formData.Add(new StringContent(model.Reason), "Reason");

            if (!string.IsNullOrEmpty(model.Description))
                formData.Add(new StringContent(model.Description), "Description");

            // Add file if exists
            if (model.File?.FormFile != null)
            {
                var fileContent = new StreamContent(model.File.FormFile.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(model.File.FormFile.ContentType);
                formData.Add(fileContent, "File.FormFile", model.File.FormFile.FileName);
            }

            return await _apiService.PostMultipartAsync<LeaveRequestViewModel>(BaseEndpoint, formData, cancellationToken);
        }

        public async Task<ApiResponse<bool>> UpdateLeaveRequestAsync(string leaveRequestId, UpdateLeaveRequestViewModel model, CancellationToken cancellationToken = default)
        {
            using var formData = new MultipartFormDataContent();

            // Add form fields
            formData.Add(new StringContent(model.StartDate.ToString("yyyy-MM-dd")), "StartDate");
            formData.Add(new StringContent(model.EndDate.ToString("yyyy-MM-dd")), "EndDate");
            formData.Add(new StringContent(model.Reason), "Reason");

            if (!string.IsNullOrEmpty(model.Description))
                formData.Add(new StringContent(model.Description), "Description");

            // Add file if exists
            if (model.File?.FormFile != null)
            {
                var fileContent = new StreamContent(model.File.FormFile.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(model.File.FormFile.ContentType);
                formData.Add(fileContent, "File.FormFile", model.File.FormFile.FileName);
            }

            return await _apiService.PutMultipartAsync<bool>($"{BaseEndpoint}/{leaveRequestId}", formData, cancellationToken);
        }

        public async Task<ApiResponse<bool>> DeleteLeaveRequestAsync(string leaveRequestId, CancellationToken cancellationToken = default)
        {
            return await _apiService.DeleteAsync<bool>($"{BaseEndpoint}/{leaveRequestId}", cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<LeaveRequestViewModel>>> GetLeaveRequestsByStatusAsync(int status, string? userId, CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<LeaveRequestViewModel>>($"{BaseEndpoint}/status/{status}?userId={userId}", cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<LeaveRequestViewModel>>> GetLeaveRequestsByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<LeaveRequestViewModel>>($"{BaseEndpoint}/user/{userId}", cancellationToken);
        }

        public async Task<ApiResponse<bool>> ApproveLeaveRequestAsync(string leaveRequestId, CancellationToken cancellationToken = default)
        {
            return await _apiService.PutAsync<bool>($"{BaseEndpoint}/{leaveRequestId}/approve", null, cancellationToken);
        }

        public async Task<ApiResponse<bool>> RejectLeaveRequestAsync(string leaveRequestId, string? rejectReason, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/{leaveRequestId}/reject";
            if (!string.IsNullOrEmpty(rejectReason))
            {
                endpoint += $"?rejectReason={Uri.EscapeDataString(rejectReason)}";
            }
            return await _apiService.PutAsync<bool>(endpoint, null, cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<LeaveRequestViewModel>>> GetMyLeaveRequestsAsync(CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<LeaveRequestViewModel>>($"{BaseEndpoint}/my-leave-requests", cancellationToken);
        }
    }
}
