using System.Text;
using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.UserModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Newtonsoft.Json;

namespace IdeKusgozManagement.WebUI.Services
{
    public class UserApiService : IUserApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserApiService> _logger;

        public UserApiService(HttpClient httpClient, ILogger<UserApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<UserViewModel>>> GetAllUsersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/users");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<UserViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<IEnumerable<UserViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<IEnumerable<UserViewModel>> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllUsersAsync işleminde hata oluştu");
                return new ApiResponse<IEnumerable<UserViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<UserViewModel>> GetUserByIdAsync(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/users/{id}");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<UserViewModel>>(content);
                    return apiResponse ?? new ApiResponse<UserViewModel> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<UserViewModel> { IsSuccess = false, Message = "Kullanıcı bulunamadı" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetUserByIdAsync işleminde hata oluştu. UserId: {UserId}", id);
                return new ApiResponse<UserViewModel> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<UserViewModel>> CreateUserAsync(CreateUserViewModel model)
        {
            try
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/users", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<UserViewModel>>(responseContent);
                    return apiResponse ?? new ApiResponse<UserViewModel> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<UserViewModel>>(responseContent);
                return errorResponse ?? new ApiResponse<UserViewModel> { IsSuccess = false, Message = "Kullanıcı oluşturulamadı" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateUserAsync işleminde hata oluştu");
                return new ApiResponse<UserViewModel> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<UserViewModel>> UpdateUserAsync(string id, UpdateUserViewModel model)
        {
            try
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"api/users/{id}", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<UserViewModel>>(responseContent);
                    return apiResponse ?? new ApiResponse<UserViewModel> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<UserViewModel>>(responseContent);
                return errorResponse ?? new ApiResponse<UserViewModel> { IsSuccess = false, Message = "Kullanıcı güncellenemedi" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateUserAsync işleminde hata oluştu. UserId: {UserId}", id);
                return new ApiResponse<UserViewModel> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<bool>> DeleteUserAsync(string id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/users/{id}");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<bool>>(content);
                    return apiResponse ?? new ApiResponse<bool> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<bool> { IsSuccess = false, Message = "Kullanıcı silinemedi" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteUserAsync işleminde hata oluştu. UserId: {UserId}", id);
                return new ApiResponse<bool> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<bool>> AssignRoleToUserAsync(AssignRoleViewModel model)
        {
            try
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/users/assign-role", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<bool>>(responseContent);
                    return apiResponse ?? new ApiResponse<bool> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<bool>>(responseContent);
                return errorResponse ?? new ApiResponse<bool> { IsSuccess = false, Message = "Rol atanamadı" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AssignRoleToUserAsync işleminde hata oluştu");
                return new ApiResponse<bool> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<bool>> ActivateUserAsync(string id)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/users/{id}/activate", null);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<bool>>(content);
                    return apiResponse ?? new ApiResponse<bool> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<bool> { IsSuccess = false, Message = "Kullanıcı aktifleştirilemedi" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ActivateUserAsync işleminde hata oluştu. UserId: {UserId}", id);
                return new ApiResponse<bool> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<bool>> DeactivateUserAsync(string id)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/users/{id}/deactivate", null);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<bool>>(content);
                    return apiResponse ?? new ApiResponse<bool> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<bool> { IsSuccess = false, Message = "Kullanıcı pasifleştirilemedi" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeactivateUserAsync işleminde hata oluştu. UserId: {UserId}", id);
                return new ApiResponse<bool> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<bool>> ChangePasswordAsync(string userId, ChangePasswordViewModel model)
        {
            try
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"api/users/{userId}/change-password", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<bool>>(responseContent);
                    return apiResponse ?? new ApiResponse<bool> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<bool>>(responseContent);
                return errorResponse ?? new ApiResponse<bool> { IsSuccess = false, Message = "Şifre değiştirilemedi" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChangePasswordAsync işleminde hata oluştu. UserId: {UserId}", userId);
                return new ApiResponse<bool> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }
    }
}