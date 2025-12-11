using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.MachineWorkRecordModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Newtonsoft.Json;

namespace IdeKusgozManagement.WebUI.Services
{
    public class MachineWorkRecordApiService : IMachineWorkRecordApiService
    {
        private readonly HttpClient _httpClient;

        public MachineWorkRecordApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<IEnumerable<MachineWorkRecordViewModel>>> GetMyMachineWorkRecordsByDateAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/MachineWorkRecords/my-records/date/{date:yyyy-MM-dd}", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<MachineWorkRecordViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<IEnumerable<MachineWorkRecordViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<IEnumerable<MachineWorkRecordViewModel>> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<MachineWorkRecordViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<IEnumerable<MachineWorkRecordViewModel>>> GetMachineWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/MachineWorkRecords/user/{userId}/date/{date:yyyy-MM-dd}", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<MachineWorkRecordViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<IEnumerable<MachineWorkRecordViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<IEnumerable<MachineWorkRecordViewModel>> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<MachineWorkRecordViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<IEnumerable<MachineWorkRecordViewModel>>> BatchCreateOrModifyMachineWorkRecordsAsync(IEnumerable<CreateOrModifyMachineWorkRecordViewModel> createMachineWorkRecordViewModels, CancellationToken cancellationToken = default)
        {
            try
            {
                using var formData = new MultipartFormDataContent();
                var recordsList = createMachineWorkRecordViewModels.ToList();

                AddMachineWorkRecordsToFormData(formData, recordsList);

                var response = await _httpClient.PostAsync("api/MachineWorkRecords/batch-create-modify", formData, cancellationToken);
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<MachineWorkRecordViewModel>>>(responseContent);
                    return apiResponse ?? new ApiResponse<IEnumerable<MachineWorkRecordViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<MachineWorkRecordViewModel>>>(responseContent);
                return errorResponse ?? new ApiResponse<IEnumerable<MachineWorkRecordViewModel>> { IsSuccess = false, Message = "İş kayıtları oluşturulamadı" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<MachineWorkRecordViewModel>> { IsSuccess = false, Message = $"Bir hata oluştu: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<IEnumerable<MachineWorkRecordViewModel>>> BatchUpdateMachineWorkRecordsByUserIdAsync(string userId, IEnumerable<CreateOrModifyMachineWorkRecordViewModel> updateMachineWorkRecordViewModel, CancellationToken cancellationToken = default)
        {
            try
            {
                using var formData = new MultipartFormDataContent();
                var recordsList = updateMachineWorkRecordViewModel.ToList();

                AddMachineWorkRecordsToFormData(formData, recordsList);

                var response = await _httpClient.PutAsync($"api/MachineWorkRecords/batch-update/user/{userId}", formData, cancellationToken);
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<MachineWorkRecordViewModel>>>(responseContent);
                    return apiResponse ?? new ApiResponse<IEnumerable<MachineWorkRecordViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<MachineWorkRecordViewModel>>>(responseContent);
                return errorResponse ?? new ApiResponse<IEnumerable<MachineWorkRecordViewModel>> { IsSuccess = false, Message = "İş kayıtları güncellenemedi" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<MachineWorkRecordViewModel>> { IsSuccess = false, Message = $"Bir hata oluştu: {ex.Message}" };
            }
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
            try
            {
                var response = await _httpClient.PutAsync($"api/MachineWorkRecords/batch-approve/user/{userId}/date/{date:yyyy-MM-dd}", null, cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<MachineWorkRecordViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<IEnumerable<MachineWorkRecordViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<MachineWorkRecordViewModel>>>(content);
                return errorResponse ?? new ApiResponse<IEnumerable<MachineWorkRecordViewModel>> { IsSuccess = false, Message = "Toplu onaylama başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<MachineWorkRecordViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<IEnumerable<MachineWorkRecordViewModel>>> BatchRejectMachineWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, string? rejectReason, CancellationToken cancellationToken = default)
        {
            try
            {
                var encodedRejectReason = string.IsNullOrEmpty(rejectReason)
                    ? string.Empty
                    : Uri.EscapeDataString(rejectReason);
                var response = await _httpClient.PutAsync($"api/MachineWorkRecords/batch-reject/user/{userId}/date/{date:yyyy-MM-dd}?rejectReason={encodedRejectReason}", null, cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<MachineWorkRecordViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<IEnumerable<MachineWorkRecordViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }
                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<MachineWorkRecordViewModel>>>(content);
                return errorResponse ?? new ApiResponse<IEnumerable<MachineWorkRecordViewModel>> { IsSuccess = false, Message = "Toplu reddetme başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<MachineWorkRecordViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<MachineWorkRecordViewModel>> ApproveMachineWorkRecordByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PutAsync($"api/MachineWorkRecords/{userId}/approve", null, cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<MachineWorkRecordViewModel>>(content);
                    return apiResponse ?? new ApiResponse<MachineWorkRecordViewModel> { IsSuccess = false, Message = "Veri alınamadı" };
                }
                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<MachineWorkRecordViewModel>>(content);
                return errorResponse ?? new ApiResponse<MachineWorkRecordViewModel> { IsSuccess = false, Message = "Onaylama başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<MachineWorkRecordViewModel> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<MachineWorkRecordViewModel>> RejectMachineWorkRecordByIdAsync(string userId, string? rejectReason, CancellationToken cancellationToken = default)
        {
            try
            {
                var encodedRejectReason = string.IsNullOrEmpty(rejectReason)
                 ? string.Empty
                 : Uri.EscapeDataString(rejectReason);

                var response = await _httpClient.PutAsync($"api/MachineWorkRecords/{userId}/reject?rejectReason={encodedRejectReason}", null, cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<MachineWorkRecordViewModel>>(content);
                    return apiResponse ?? new ApiResponse<MachineWorkRecordViewModel> { IsSuccess = false, Message = "Veri alınamadı" };
                }
                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<MachineWorkRecordViewModel>>(content);
                return errorResponse ?? new ApiResponse<MachineWorkRecordViewModel> { IsSuccess = false, Message = "Reddetme başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<MachineWorkRecordViewModel> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<IEnumerable<MachineWorkRecordViewModel>>> GetMachineWorkRecordsByUserIdDateStatusAsync(string userId, DateTime date, int status, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/MachineWorkRecords/user/{userId}/date/{date:yyyy-MM-dd}/status/{status}", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<MachineWorkRecordViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<IEnumerable<MachineWorkRecordViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<IEnumerable<MachineWorkRecordViewModel>> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<MachineWorkRecordViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<IEnumerable<MachineWorkRecordViewModel>>> GetApprovedMachineWorkRecordsByUserAsync(string userId, DateTime date, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/MachineWorkRecords/approved/user/{userId}/date/{date:yyyy-MM-dd}", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<MachineWorkRecordViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<IEnumerable<MachineWorkRecordViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<IEnumerable<MachineWorkRecordViewModel>> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<MachineWorkRecordViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }
    }
}