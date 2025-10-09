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
                var json = JsonConvert.SerializeObject(createWorkRecordViewModels);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/workrecords/batch-create", content, cancellationToken);
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
                return new ApiResponse<IEnumerable<WorkRecordViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
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

        public async Task<ApiResponse<IEnumerable<WorkRecordViewModel>>> BatchRejectWorkRecordsByUserIdAndDateAsync(string userId, DateTime date, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PutAsync($"api/workrecords/batch-reject/user/{userId}/date/{date:yyyy-MM-dd}", null, cancellationToken);
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

        public async Task<ApiResponse<WorkRecordViewModel>> RejectWorkRecordByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PutAsync($"api/workrecords/{userId}/reject", null, cancellationToken);
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