using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.WorkRecordModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Newtonsoft.Json;
using System.Text;

namespace IdeKusgozManagement.WebUI.Services
{
    public class WorkRecordApiService : IWorkRecordApiService
    {
        private readonly HttpClient _httpClient;

        public WorkRecordApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<IEnumerable<WorkRecordViewModel>>> GetMyWorkRecordsByDateAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/workrecords/my-records/date/{date:yyyy-MM-dd}", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<WorkRecordViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<IEnumerable<WorkRecordViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<IEnumerable<WorkRecordViewModel>> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<WorkRecordViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<IEnumerable<WorkRecordViewModel>>> GetWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/workrecords/user/{userId}/date/{date:yyyy-MM-dd}", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<WorkRecordViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<IEnumerable<WorkRecordViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<IEnumerable<WorkRecordViewModel>> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<WorkRecordViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<IEnumerable<WorkRecordViewModel>>> BatchCreateWorkRecordsAsync(IEnumerable<CreateWorkRecordViewModel> createWorkRecordViewModels, CancellationToken cancellationToken = default)
        {
            try
            {
                using var formData = new MultipartFormDataContent();

                var recordsList = createWorkRecordViewModels.ToList();

                for (int i = 0; i < recordsList.Count; i++)
                {
                    var record = recordsList[i];

                    // Ana work record alanları
                    formData.Add(new StringContent(record.Date.ToString("yyyy-MM-dd")), $"[{i}].Date");

                    if (!string.IsNullOrEmpty(record.ExcuseReason))
                        formData.Add(new StringContent(record.ExcuseReason), $"[{i}].ExcuseReason");

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

                    formData.Add(new StringContent(record.HasBreakfast.ToString()), $"[{i}].HasBreakfast");
                    formData.Add(new StringContent(record.HasLunch.ToString()), $"[{i}].HasLunch");
                    formData.Add(new StringContent(record.HasDinner.ToString()), $"[{i}].HasDinner");
                    formData.Add(new StringContent(record.HasNightMeal.ToString()), $"[{i}].HasNightMeal");
                    formData.Add(new StringContent(record.HasTravel.ToString()), $"[{i}].HasTravel");

                    // Expenses
                    if (record.WorkRecordExpenses != null && record.WorkRecordExpenses.Any())
                    {
                        for (int j = 0; j < record.WorkRecordExpenses.Count; j++)
                        {
                            var expense = record.WorkRecordExpenses[j];

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

                var response = await _httpClient.PostAsync("api/workrecords/batch-create-modify", formData, cancellationToken);
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<WorkRecordViewModel>>>(responseContent);
                    return apiResponse ?? new ApiResponse<IEnumerable<WorkRecordViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<WorkRecordViewModel>>>(responseContent);
                return errorResponse ?? new ApiResponse<IEnumerable<WorkRecordViewModel>> { IsSuccess = false, Message = "İş kayıtları oluşturulamadı" };
            }
            catch (Exception ex)
            {
                // Loglama ekleyin
                return new ApiResponse<IEnumerable<WorkRecordViewModel>> { IsSuccess = false, Message = $"Bir hata oluştu: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<IEnumerable<WorkRecordViewModel>>> BatchUpdateWorkRecordsByUserIdAsync(string userId, IEnumerable<UpdateWorkRecordViewModel> updateWorkRecordViewModel, CancellationToken cancellationToken = default)
        {
            try
            {
                var json = JsonConvert.SerializeObject(updateWorkRecordViewModel);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"api/workrecords/batch-update/user/{userId}", content, cancellationToken);
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<WorkRecordViewModel>>>(responseContent);
                    return apiResponse ?? new ApiResponse<IEnumerable<WorkRecordViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<WorkRecordViewModel>>>(responseContent);
                return errorResponse ?? new ApiResponse<IEnumerable<WorkRecordViewModel>> { IsSuccess = false, Message = "İş kayıtları güncellenemedi" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<WorkRecordViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<IEnumerable<WorkRecordViewModel>>> BatchApproveWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PutAsync($"api/workrecords/batch-approve/user/{userId}/date/{date:yyyy-MM-dd}", null, cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<WorkRecordViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<IEnumerable<WorkRecordViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<IEnumerable<WorkRecordViewModel>> { IsSuccess = false, Message = "Onaylama başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<WorkRecordViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<IEnumerable<WorkRecordViewModel>>> BatchRejectWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, string? rejectReason, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PutAsync($"api/workrecords/batch-reject/user/{userId}/date/{date:yyyy-MM-dd}?rejectReason={Uri.EscapeDataString(rejectReason)}", null, cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<WorkRecordViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<IEnumerable<WorkRecordViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<IEnumerable<WorkRecordViewModel>> { IsSuccess = false, Message = "Reddetme başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<WorkRecordViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<WorkRecordViewModel>> ApproveWorkRecordByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PutAsync($"api/workrecords/{userId}/approve", null, cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<WorkRecordViewModel>>(content);
                    return apiResponse ?? new ApiResponse<WorkRecordViewModel> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<WorkRecordViewModel> { IsSuccess = false, Message = "Onaylama başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<WorkRecordViewModel> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<WorkRecordViewModel>> RejectWorkRecordByIdAsync(string userId, string? rejectReason, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PutAsync($"api/workrecords/{userId}/reject?rejectReason={Uri.EscapeDataString(rejectReason)}", null, cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<WorkRecordViewModel>>(content);
                    return apiResponse ?? new ApiResponse<WorkRecordViewModel> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<WorkRecordViewModel> { IsSuccess = false, Message = "Onaylama başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<WorkRecordViewModel> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }
    }
}