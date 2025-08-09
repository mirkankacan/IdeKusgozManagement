using System.Text;
using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.RoleModels;
using IdeKusgozManagement.WebUI.Models.UserModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Newtonsoft.Json;

namespace IdeKusgozManagement.WebUI.Services
{
    public class RoleApiService : IRoleApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RoleApiService> _logger;

        public RoleApiService(HttpClient httpClient, ILogger<RoleApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<RoleViewModel>>> GetAllRolesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/roles");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<RoleViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<IEnumerable<RoleViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<IEnumerable<RoleViewModel>> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllRolesAsync işleminde hata oluştu");
                return new ApiResponse<IEnumerable<RoleViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<RoleViewModel>> GetRoleByIdAsync(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/roles/{id}");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<RoleViewModel>>(content);
                    return apiResponse ?? new ApiResponse<RoleViewModel> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<RoleViewModel> { IsSuccess = false, Message = "Rol bulunamadı" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetRoleByIdAsync işleminde hata oluştu. RoleId: {RoleId}", id);
                return new ApiResponse<RoleViewModel> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<RoleViewModel>> GetRoleByNameAsync(string name)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/roles/by-name/{name}");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<RoleViewModel>>(content);
                    return apiResponse ?? new ApiResponse<RoleViewModel> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<RoleViewModel> { IsSuccess = false, Message = "Rol bulunamadı" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetRoleByNameAsync işleminde hata oluştu. RoleName: {RoleName}", name);
                return new ApiResponse<RoleViewModel> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<RoleViewModel>> CreateRoleAsync(CreateRoleViewModel model)
        {
            try
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/roles", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<RoleViewModel>>(responseContent);
                    return apiResponse ?? new ApiResponse<RoleViewModel> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<RoleViewModel>>(responseContent);
                return errorResponse ?? new ApiResponse<RoleViewModel> { IsSuccess = false, Message = "Rol oluşturulamadı" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateRoleAsync işleminde hata oluştu");
                return new ApiResponse<RoleViewModel> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<RoleViewModel>> UpdateRoleAsync(string id, UpdateRoleViewModel model)
        {
            try
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"api/roles/{id}", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<RoleViewModel>>(responseContent);
                    return apiResponse ?? new ApiResponse<RoleViewModel> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<RoleViewModel>>(responseContent);
                return errorResponse ?? new ApiResponse<RoleViewModel> { IsSuccess = false, Message = "Rol güncellenemedi" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateRoleAsync işleminde hata oluştu. RoleId: {RoleId}", id);
                return new ApiResponse<RoleViewModel> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<bool>> DeleteRoleAsync(string id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/roles/{id}");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<bool>>(content);
                    return apiResponse ?? new ApiResponse<bool> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<bool> { IsSuccess = false, Message = "Rol silinemedi" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteRoleAsync işleminde hata oluştu. RoleId: {RoleId}", id);
                return new ApiResponse<bool> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<bool>> ActivateRoleAsync(string id)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/roles/{id}/activate", null);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<bool>>(content);
                    return apiResponse ?? new ApiResponse<bool> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<bool> { IsSuccess = false, Message = "Rol aktifleştirilemedi" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ActivateRoleAsync işleminde hata oluştu. RoleId: {RoleId}", id);
                return new ApiResponse<bool> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<bool>> DeactivateRoleAsync(string id)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/roles/{id}/deactivate", null);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<bool>>(content);
                    return apiResponse ?? new ApiResponse<bool> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<bool> { IsSuccess = false, Message = "Rol pasifleştirilemedi" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeactivateRoleAsync işleminde hata oluştu. RoleId: {RoleId}", id);
                return new ApiResponse<bool> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<IEnumerable<UserViewModel>>> GetUsersInRoleAsync(string roleName)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/roles/{roleName}/users");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<UserViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<IEnumerable<UserViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<IEnumerable<UserViewModel>> { IsSuccess = false, Message = "Kullanıcılar getirilemedi" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetUsersInRoleAsync işleminde hata oluştu. RoleName: {RoleName}", roleName);
                return new ApiResponse<IEnumerable<UserViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }
    }
}