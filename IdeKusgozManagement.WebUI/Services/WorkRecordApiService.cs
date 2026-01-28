using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.WorkRecordModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;

namespace IdeKusgozManagement.WebUI.Services
{
    public class WorkRecordApiService : IWorkRecordApiService
    {
        private readonly IApiService _apiService;
        private readonly ILogger<WorkRecordApiService> _logger;
        private const string BaseEndpoint = "api/workrecords";

        public WorkRecordApiService(
            IApiService apiService,
            ILogger<WorkRecordApiService> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<WorkRecordViewModel>>> GetMyWorkRecordsByDateAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<WorkRecordViewModel>>($"{BaseEndpoint}/my-records/date/{date:yyyy-MM-dd}", cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<WorkRecordViewModel>>> GetWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<WorkRecordViewModel>>($"{BaseEndpoint}/user/{userId}/date/{date:yyyy-MM-dd}", cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<WorkRecordViewModel>>> BatchCreateOrModifyWorkRecordsAsync(IEnumerable<CreateOrModifyWorkRecordViewModel> createWorkRecordViewModels, CancellationToken cancellationToken = default)
        {
            using var formData = new MultipartFormDataContent();
            var recordsList = createWorkRecordViewModels.ToList();

            AddWorkRecordsToFormData(formData, recordsList);

            return await _apiService.PostMultipartAsync<IEnumerable<WorkRecordViewModel>>($"{BaseEndpoint}/batch-create-modify", formData, cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<WorkRecordViewModel>>> BatchUpdateWorkRecordsByUserIdAsync(string userId, IEnumerable<CreateOrModifyWorkRecordViewModel> updateWorkRecordViewModel, CancellationToken cancellationToken = default)
        {
            using var formData = new MultipartFormDataContent();
            var recordsList = updateWorkRecordViewModel.ToList();

            AddWorkRecordsToFormData(formData, recordsList);

            return await _apiService.PutMultipartAsync<IEnumerable<WorkRecordViewModel>>($"{BaseEndpoint}/batch-update/user/{userId}", formData, cancellationToken);
        }

        private void AddWorkRecordsToFormData(MultipartFormDataContent formData, List<CreateOrModifyWorkRecordViewModel> recordsList)
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

                if (record.AdditionalStartTime.HasValue)
                    formData.Add(new StringContent(record.AdditionalStartTime.Value.ToString(@"hh\:mm")), $"[{i}].AdditionalStartTime");

                if (record.AdditionalEndTime.HasValue)
                    formData.Add(new StringContent(record.AdditionalEndTime.Value.ToString(@"hh\:mm")), $"[{i}].AdditionalEndTime");

                if (!string.IsNullOrEmpty(record.ProjectId))
                    formData.Add(new StringContent(record.ProjectId), $"[{i}].ProjectId");

                if (!string.IsNullOrEmpty(record.EquipmentId))
                    formData.Add(new StringContent(record.EquipmentId), $"[{i}].EquipmentId");

                if (!string.IsNullOrEmpty(record.Province))
                    formData.Add(new StringContent(record.Province), $"[{i}].Province");

                if (!string.IsNullOrEmpty(record.District))
                    formData.Add(new StringContent(record.District), $"[{i}].District");

                if (record.TravelExpenseAmount.HasValue)
                    formData.Add(new StringContent(record.TravelExpenseAmount.Value.ToString()), $"[{i}].TravelExpenseAmount");

                formData.Add(new StringContent(record.HasBreakfast.ToString()), $"[{i}].HasBreakfast");
                formData.Add(new StringContent(record.HasLunch.ToString()), $"[{i}].HasLunch");
                formData.Add(new StringContent(record.HasDinner.ToString()), $"[{i}].HasDinner");
                formData.Add(new StringContent(record.HasNightMeal.ToString()), $"[{i}].HasNightMeal");

                // Expenses
                if (record.WorkRecordExpenses != null && record.WorkRecordExpenses.Any())
                {
                    for (int j = 0; j < record.WorkRecordExpenses.Count; j++)
                    {
                        var expense = record.WorkRecordExpenses[j];

                        if (!string.IsNullOrEmpty(expense.Id))
                            formData.Add(new StringContent(expense.Id), $"[{i}].WorkRecordExpenses[{j}].Id");

                        formData.Add(new StringContent(expense.ExpenseId), $"[{i}].WorkRecordExpenses[{j}].ExpenseId");
                        formData.Add(new StringContent(expense.Amount.ToString()), $"[{i}].WorkRecordExpenses[{j}].Amount");

                        if (!string.IsNullOrEmpty(expense.Description))
                            formData.Add(new StringContent(expense.Description), $"[{i}].WorkRecordExpenses[{j}].Description");

                        // Dosya varsa ekle
                        if (expense.File != null && expense.File.FormFile.Length > 0)
                        {
                            var fileContent = new StreamContent(expense.File.FormFile.OpenReadStream());
                            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(expense.File.FormFile.ContentType);
                            formData.Add(fileContent, $"[{i}].WorkRecordExpenses[{j}].File.FormFile", expense.File.FormFile.FileName);
                        }
                    }
                }
            }
        }

        public async Task<ApiResponse<IEnumerable<WorkRecordViewModel>>> BatchApproveWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, CancellationToken cancellationToken = default)
        {
            return await _apiService.PutAsync<IEnumerable<WorkRecordViewModel>>($"{BaseEndpoint}/batch-approve/user/{userId}/date/{date:yyyy-MM-dd}", null, cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<WorkRecordViewModel>>> BatchRejectWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, string? rejectReason, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/batch-reject/user/{userId}/date/{date:yyyy-MM-dd}";
            if (!string.IsNullOrEmpty(rejectReason))
            {
                endpoint += $"?rejectReason={Uri.EscapeDataString(rejectReason)}";
            }
            return await _apiService.PutAsync<IEnumerable<WorkRecordViewModel>>(endpoint, null, cancellationToken);
        }

        public async Task<ApiResponse<WorkRecordViewModel>> ApproveWorkRecordByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _apiService.PutAsync<WorkRecordViewModel>($"{BaseEndpoint}/{userId}/approve", null, cancellationToken);
        }

        public async Task<ApiResponse<WorkRecordViewModel>> RejectWorkRecordByIdAsync(string userId, string? rejectReason, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/{userId}/reject";
            if (!string.IsNullOrEmpty(rejectReason))
            {
                endpoint += $"?rejectReason={Uri.EscapeDataString(rejectReason)}";
            }
            return await _apiService.PutAsync<WorkRecordViewModel>(endpoint, null, cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<WorkRecordViewModel>>> GetWorkRecordsByUserIdDateStatusAsync(string userId, DateTime date, int status, CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<WorkRecordViewModel>>($"{BaseEndpoint}/user/{userId}/date/{date:yyyy-MM-dd}/status/{status}", cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<WorkRecordViewModel>>> GetApprovedWorkRecordsByUserAsync(string userId, DateTime date, CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<WorkRecordViewModel>>($"{BaseEndpoint}/approved/user/{userId}/date/{date:yyyy-MM-dd}", cancellationToken);
        }
    }
}
