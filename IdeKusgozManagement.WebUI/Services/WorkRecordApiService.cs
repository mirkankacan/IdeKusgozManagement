using System.Text;
using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.WorkRecordModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Newtonsoft.Json;

namespace IdeKusgozManagement.WebUI.Services
{
    public class WorkRecordApiService : IWorkRecordApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WorkRecordApiService> _logger;

        public WorkRecordApiService(HttpClient httpClient, ILogger<WorkRecordApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<WorkRecordViewModel>>> GetAllWorkRecordsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync("api/workrecords", cancellationToken);
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
                _logger.LogError(ex, "GetAllWorkRecordsAsync işleminde hata oluştu");
                return new ApiResponse<IEnumerable<WorkRecordViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<WorkRecordViewModel>> GetWorkRecordByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/workrecords/{id}", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<WorkRecordViewModel>>(content);
                    return apiResponse ?? new ApiResponse<WorkRecordViewModel> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<WorkRecordViewModel> { IsSuccess = false, Message = "İş kaydı bulunamadı" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetWorkRecordByIdAsync işleminde hata oluştu. WorkRecordId: {WorkRecordId}", id);
                return new ApiResponse<WorkRecordViewModel> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<IEnumerable<WorkRecordViewModel>>> GetMyWorkRecordsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync("api/workrecords/my-records", cancellationToken);
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
                _logger.LogError(ex, "GetMyWorkRecordsAsync işleminde hata oluştu");
                return new ApiResponse<IEnumerable<WorkRecordViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<IEnumerable<WorkRecordViewModel>>> GetWorkRecordsByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/workrecords/user/{userId}", cancellationToken);
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
                _logger.LogError(ex, "GetWorkRecordsByUserIdAsync işleminde hata oluştu. UserId: {UserId}", userId);
                return new ApiResponse<IEnumerable<WorkRecordViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<IEnumerable<WorkRecordViewModel>>> GetWorkRecordsByDateAndUserAsync(DateTime date, string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/workrecords/by-date-and-user?date={date:yyyy-MM-dd}&userId={userId}", cancellationToken);
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
                _logger.LogError(ex, "GetWorkRecordsByDateAndUserAsync işleminde hata oluştu. Date: {Date}, UserId: {UserId}", date, userId);
                return new ApiResponse<IEnumerable<WorkRecordViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<IEnumerable<WorkRecordViewModel>>> BatchCreateWorkRecordsAsync(List<CreateWorkRecordViewModel> createWorkRecordViewModels, CancellationToken cancellationToken = default)
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
                _logger.LogError(ex, "BatchCreateWorkRecordsAsync işleminde hata oluştu");
                return new ApiResponse<IEnumerable<WorkRecordViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<WorkRecordViewModel>> UpdateWorkRecordAsync(string id, UpdateWorkRecordViewModel updateWorkRecordViewModel, CancellationToken cancellationToken = default)
        {
            try
            {
                var json = JsonConvert.SerializeObject(updateWorkRecordViewModel);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"api/workrecords/{id}", content, cancellationToken);
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<WorkRecordViewModel>>(responseContent);
                    return apiResponse ?? new ApiResponse<WorkRecordViewModel> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<WorkRecordViewModel>>(responseContent);
                return errorResponse ?? new ApiResponse<WorkRecordViewModel> { IsSuccess = false, Message = "İş kaydı güncellenemedi" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateWorkRecordAsync işleminde hata oluştu. WorkRecordId: {WorkRecordId}", id);
                return new ApiResponse<WorkRecordViewModel> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<bool>> BatchApproveWorkRecordsByUserAndDateAsync(string userId, DateTime date, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/workrecords/approve?userId={userId}&date={date:yyyy-MM-dd}", null, cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<bool>>(content);
                    return apiResponse ?? new ApiResponse<bool> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<bool> { IsSuccess = false, Message = "Onaylama başarısız" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BatchApproveWorkRecordsByUserAndDateAsync işleminde hata oluştu. UserId: {UserId}, Date: {Date}", userId, date);
                return new ApiResponse<bool> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<bool>> BatchRejectWorkRecordsByUserAndDateAsync(string userId, DateTime date, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/workrecords/reject?userId={userId}&date={date:yyyy-MM-dd}", null, cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<bool>>(content);
                    return apiResponse ?? new ApiResponse<bool> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<bool> { IsSuccess = false, Message = "Reddetme başarısız" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BatchRejectWorkRecordsByUserAndDateAsync işleminde hata oluştu. UserId: {UserId}, Date: {Date}", userId, date);
                return new ApiResponse<bool> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<int>> GetWorkRecordCountAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync("api/workrecords/count", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<int>>(content);
                    return apiResponse ?? new ApiResponse<int> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<int> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetWorkRecordCountAsync işleminde hata oluştu");
                return new ApiResponse<int> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<int>> GetWorkRecordCountByStatusAsync(int status, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/workrecords/count/by-status/{status}", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<int>>(content);
                    return apiResponse ?? new ApiResponse<int> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<int> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetWorkRecordCountByStatusAsync işleminde hata oluştu. Status: {Status}", status);
                return new ApiResponse<int> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }
    }
}