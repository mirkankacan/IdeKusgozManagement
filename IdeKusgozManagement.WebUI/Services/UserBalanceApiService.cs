using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.UserBalanceModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Newtonsoft.Json;
using System.Text;

namespace IdeKusgozManagement.WebUI.Services
{
    public class UserBalanceApiService : IUserBalanceApiService
    {
        private readonly HttpClient _httpClient;

        public UserBalanceApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<bool>> DecreaseUserBalanceAsync(string userId, UpdateUserBalanceViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"api/userbalances/{userId}/decrease", content, cancellationToken);
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<bool>>(responseContent);
                    return apiResponse ?? new ApiResponse<bool> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<bool>>(responseContent);
                return errorResponse ?? new ApiResponse<bool> { IsSuccess = false, Message = "Bakiye azaltılamadı" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<UserBalanceViewModel>> GetMyBalancesAsync(CancellationToken cancellationToken)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/userbalances/my-balance", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<UserBalanceViewModel>>(content);
                    return apiResponse ?? new ApiResponse<UserBalanceViewModel> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<UserBalanceViewModel> { IsSuccess = false, Message = "Kullanıcı bulunamadı" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<UserBalanceViewModel> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<UserBalanceViewModel>> GetUserBalancesByUserAsync(string userId, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/userbalances/{userId}", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<UserBalanceViewModel>>(content);
                    return apiResponse ?? new ApiResponse<UserBalanceViewModel> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<UserBalanceViewModel> { IsSuccess = false, Message = "Kullanıcı bulunamadı" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<UserBalanceViewModel> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<bool>> IncreaseUserBalanceAsync(string userId, UpdateUserBalanceViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"api/userbalances/{userId}/increase", content, cancellationToken);
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<bool>>(responseContent);
                    return apiResponse ?? new ApiResponse<bool> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<bool>>(responseContent);
                return errorResponse ?? new ApiResponse<bool> { IsSuccess = false, Message = "Bakiye arttırılamadı" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }
    }
}