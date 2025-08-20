using System.Text;
using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.WorkRecordExpenseModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Newtonsoft.Json;

namespace IdeKusgozManagement.WebUI.Services
{
    public class WorkRecordExpenseApiService : IWorkRecordExpenseApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WorkRecordExpenseApiService> _logger;

        public WorkRecordExpenseApiService(HttpClient httpClient, ILogger<WorkRecordExpenseApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ApiResponse<WorkRecordExpenseViewModel>> AddExpenseToWorkRecordAsync(string workRecordId, CreateWorkRecordExpenseViewModel createExpenseViewModel, CancellationToken cancellationToken = default)
        {
            try
            {
                var json = JsonConvert.SerializeObject(createExpenseViewModel);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"api/workrecordexpenses/work-record/{workRecordId}", content, cancellationToken);
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<WorkRecordExpenseViewModel>>(responseContent);
                    return apiResponse ?? new ApiResponse<WorkRecordExpenseViewModel> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<WorkRecordExpenseViewModel>>(responseContent);
                return errorResponse ?? new ApiResponse<WorkRecordExpenseViewModel> { IsSuccess = false, Message = "Masraf eklenemedi" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddExpenseToWorkRecordAsync işleminde hata oluştu. WorkRecordId: {WorkRecordId}", workRecordId);
                return new ApiResponse<WorkRecordExpenseViewModel> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<IEnumerable<WorkRecordExpenseViewModel>>> GetWorkRecordExpensesAsync(string workRecordId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/workrecordexpenses/work-record/{workRecordId}", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<WorkRecordExpenseViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<IEnumerable<WorkRecordExpenseViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<IEnumerable<WorkRecordExpenseViewModel>> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetWorkRecordExpensesAsync işleminde hata oluştu. WorkRecordId: {WorkRecordId}", workRecordId);
                return new ApiResponse<IEnumerable<WorkRecordExpenseViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<WorkRecordExpenseViewModel>> GetExpenseByIdAsync(string expenseId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/workrecordexpenses/{expenseId}", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<WorkRecordExpenseViewModel>>(content);
                    return apiResponse ?? new ApiResponse<WorkRecordExpenseViewModel> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<WorkRecordExpenseViewModel> { IsSuccess = false, Message = "Masraf bulunamadı" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetExpenseByIdAsync işleminde hata oluştu. ExpenseId: {ExpenseId}", expenseId);
                return new ApiResponse<WorkRecordExpenseViewModel> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<WorkRecordExpenseViewModel>> UpdateWorkRecordExpenseAsync(string expenseId, UpdateWorkRecordExpenseViewModel updateExpenseViewModel, CancellationToken cancellationToken = default)
        {
            try
            {
                var json = JsonConvert.SerializeObject(updateExpenseViewModel);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"api/workrecordexpenses/{expenseId}", content, cancellationToken);
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<WorkRecordExpenseViewModel>>(responseContent);
                    return apiResponse ?? new ApiResponse<WorkRecordExpenseViewModel> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<WorkRecordExpenseViewModel>>(responseContent);
                return errorResponse ?? new ApiResponse<WorkRecordExpenseViewModel> { IsSuccess = false, Message = "Masraf güncellenemedi" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateWorkRecordExpenseAsync işleminde hata oluştu. ExpenseId: {ExpenseId}", expenseId);
                return new ApiResponse<WorkRecordExpenseViewModel> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<bool>> DeleteWorkRecordExpenseAsync(string expenseId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/workrecordexpenses/{expenseId}", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<bool>>(content);
                    return apiResponse ?? new ApiResponse<bool> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<bool> { IsSuccess = false, Message = "Masraf silinemedi" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteWorkRecordExpenseAsync işleminde hata oluştu. ExpenseId: {ExpenseId}", expenseId);
                return new ApiResponse<bool> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<IEnumerable<WorkRecordExpenseViewModel>>> GetAllExpensesByUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/workrecordexpenses/user/{userId}", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<WorkRecordExpenseViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<IEnumerable<WorkRecordExpenseViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<IEnumerable<WorkRecordExpenseViewModel>> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllExpensesByUserAsync işleminde hata oluştu. UserId: {UserId}", userId);
                return new ApiResponse<IEnumerable<WorkRecordExpenseViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<IEnumerable<WorkRecordExpenseViewModel>>> GetMyExpensesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync("api/workrecordexpenses/my-expenses", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<WorkRecordExpenseViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<IEnumerable<WorkRecordExpenseViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<IEnumerable<WorkRecordExpenseViewModel>> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetMyExpensesAsync işleminde hata oluştu");
                return new ApiResponse<IEnumerable<WorkRecordExpenseViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<decimal>> GetTotalExpenseAmountByWorkRecordAsync(string workRecordId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/workrecordexpenses/work-record/{workRecordId}/total", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<decimal>>(content);
                    return apiResponse ?? new ApiResponse<decimal> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<decimal> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetTotalExpenseAmountByWorkRecordAsync işleminde hata oluştu. WorkRecordId: {WorkRecordId}", workRecordId);
                return new ApiResponse<decimal> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<decimal>> GetTotalExpenseAmountByUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/workrecordexpenses/user/{userId}/total", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<decimal>>(content);
                    return apiResponse ?? new ApiResponse<decimal> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<decimal> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetTotalExpenseAmountByUserAsync işleminde hata oluştu. UserId: {UserId}", userId);
                return new ApiResponse<decimal> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<decimal>> GetMyTotalExpenseAmountAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync("api/workrecordexpenses/my-total", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<decimal>>(content);
                    return apiResponse ?? new ApiResponse<decimal> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<decimal> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetMyTotalExpenseAmountAsync işleminde hata oluştu");
                return new ApiResponse<decimal> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<decimal>> GetTotalExpenseAmountAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync("api/workrecordexpenses/total", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<decimal>>(content);
                    return apiResponse ?? new ApiResponse<decimal> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<decimal> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetTotalExpenseAmountAsync işleminde hata oluştu");
                return new ApiResponse<decimal> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<decimal>> GetAverageExpenseAmountAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync("api/workrecordexpenses/average", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<decimal>>(content);
                    return apiResponse ?? new ApiResponse<decimal> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<decimal> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAverageExpenseAmountAsync işleminde hata oluştu");
                return new ApiResponse<decimal> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }
    }
}