using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.MachineWorkRecordModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;

namespace IdeKusgozManagement.WebUI.Services
{
    public class MachineWorkRecordApiService : IMachineWorkRecordApiService
    {
        private readonly IApiService _apiService;
        private readonly ILogger<MachineWorkRecordApiService> _logger;
        private const string BaseEndpoint = "api/MachineWorkRecords";

        public MachineWorkRecordApiService(
            IApiService apiService,
            ILogger<MachineWorkRecordApiService> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<MachineWorkRecordViewModel>>> GetMyMachineWorkRecordsByDateAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<MachineWorkRecordViewModel>>($"{BaseEndpoint}/my-records/date/{date:yyyy-MM-dd}", cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<MachineWorkRecordViewModel>>> GetMachineWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<MachineWorkRecordViewModel>>($"{BaseEndpoint}/user/{userId}/date/{date:yyyy-MM-dd}", cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<MachineWorkRecordViewModel>>> BatchCreateOrModifyMachineWorkRecordsAsync(IEnumerable<CreateOrModifyMachineWorkRecordViewModel> createMachineWorkRecordViewModels, CancellationToken cancellationToken = default)
        {
            using var formData = new MultipartFormDataContent();
            var recordsList = createMachineWorkRecordViewModels.ToList();

            AddMachineWorkRecordsToFormData(formData, recordsList);

            return await _apiService.PostMultipartAsync<IEnumerable<MachineWorkRecordViewModel>>($"{BaseEndpoint}/batch-create-modify", formData, cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<MachineWorkRecordViewModel>>> BatchUpdateMachineWorkRecordsByUserIdAsync(string userId, IEnumerable<CreateOrModifyMachineWorkRecordViewModel> updateMachineWorkRecordViewModel, CancellationToken cancellationToken = default)
        {
            using var formData = new MultipartFormDataContent();
            var recordsList = updateMachineWorkRecordViewModel.ToList();

            AddMachineWorkRecordsToFormData(formData, recordsList);

            return await _apiService.PutMultipartAsync<IEnumerable<MachineWorkRecordViewModel>>($"{BaseEndpoint}/batch-update/user/{userId}", formData, cancellationToken);
        }

        private void AddMachineWorkRecordsToFormData(MultipartFormDataContent formData, List<CreateOrModifyMachineWorkRecordViewModel> recordsList)
        {
            for (int i = 0; i < recordsList.Count; i++)
            {
                var record = recordsList[i];

                // Ana work record alanları
                formData.Add(new StringContent(record.Date.ToString("yyyy-MM-dd")), $"[{i}].Date");
                formData.Add(new StringContent(record.DailyStatus), $"[{i}].DailyStatus");

                if (record.StartTime.HasValue)
                    formData.Add(new StringContent(record.StartTime.Value.ToString(@"hh\:mm")), $"[{i}].StartTime");

                if (record.EndTime.HasValue)
                    formData.Add(new StringContent(record.EndTime.Value.ToString(@"hh\:mm")), $"[{i}].EndTime");

                if (!string.IsNullOrEmpty(record.ProjectId))
                    formData.Add(new StringContent(record.ProjectId), $"[{i}].ProjectId");

                if (!string.IsNullOrEmpty(record.EquipmentId))
                    formData.Add(new StringContent(record.EquipmentId), $"[{i}].EquipmentId");

                if (!string.IsNullOrEmpty(record.Province))
                    formData.Add(new StringContent(record.Province), $"[{i}].Province");

                if (!string.IsNullOrEmpty(record.District))
                    formData.Add(new StringContent(record.District), $"[{i}].District");

                if (!string.IsNullOrEmpty(record.Description))
                    formData.Add(new StringContent(record.Description), $"[{i}].Description");

                formData.Add(new StringContent(record.HasInternalTransport.ToString()), $"[{i}].HasInternalTransport");
            }
        }

        public async Task<ApiResponse<IEnumerable<MachineWorkRecordViewModel>>> BatchApproveMachineWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, CancellationToken cancellationToken = default)
        {
            return await _apiService.PutAsync<IEnumerable<MachineWorkRecordViewModel>>($"{BaseEndpoint}/batch-approve/user/{userId}/date/{date:yyyy-MM-dd}", null, cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<MachineWorkRecordViewModel>>> BatchRejectMachineWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, string? rejectReason, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/batch-reject/user/{userId}/date/{date:yyyy-MM-dd}";
            if (!string.IsNullOrEmpty(rejectReason))
            {
                endpoint += $"?rejectReason={Uri.EscapeDataString(rejectReason)}";
            }
            return await _apiService.PutAsync<IEnumerable<MachineWorkRecordViewModel>>(endpoint, null, cancellationToken);
        }

        public async Task<ApiResponse<MachineWorkRecordViewModel>> ApproveMachineWorkRecordByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _apiService.PutAsync<MachineWorkRecordViewModel>($"{BaseEndpoint}/{userId}/approve", null, cancellationToken);
        }

        public async Task<ApiResponse<MachineWorkRecordViewModel>> RejectMachineWorkRecordByIdAsync(string userId, string? rejectReason, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/{userId}/reject";
            if (!string.IsNullOrEmpty(rejectReason))
            {
                endpoint += $"?rejectReason={Uri.EscapeDataString(rejectReason)}";
            }
            return await _apiService.PutAsync<MachineWorkRecordViewModel>(endpoint, null, cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<MachineWorkRecordViewModel>>> GetMachineWorkRecordsByUserIdDateStatusAsync(string userId, DateTime date, int status, CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<MachineWorkRecordViewModel>>($"{BaseEndpoint}/user/{userId}/date/{date:yyyy-MM-dd}/status/{status}", cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<MachineWorkRecordViewModel>>> GetApprovedMachineWorkRecordsByUserAsync(string userId, DateTime date, CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<MachineWorkRecordViewModel>>($"{BaseEndpoint}/approved/user/{userId}/date/{date:yyyy-MM-dd}", cancellationToken);
        }
    }
}
