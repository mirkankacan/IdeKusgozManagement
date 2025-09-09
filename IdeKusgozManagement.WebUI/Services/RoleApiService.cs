using System.Text;
using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.RoleModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Newtonsoft.Json;

namespace IdeKusgozManagement.WebUI.Services
{
    public class RoleApiService : IRoleApiService
    {
        private readonly HttpClient _httpClient;

        public RoleApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<IEnumerable<RoleViewModel>>> GetRolesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync("api/roles", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<RoleViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<IEnumerable<RoleViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<IEnumerable<RoleViewModel>> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<RoleViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<IEnumerable<RoleViewModel>>> GetActiveRolesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync("api/roles/active-roles", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<RoleViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<IEnumerable<RoleViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<IEnumerable<RoleViewModel>> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<RoleViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<RoleViewModel>> GetRoleByIdAsync(string roleId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/roles/{roleId}", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<RoleViewModel>>(content);
                    return apiResponse ?? new ApiResponse<RoleViewModel> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<RoleViewModel> { IsSuccess = false, Message = "Rol bulunamadı" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<RoleViewModel> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<RoleViewModel>> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/roles/name/{roleName}", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<RoleViewModel>>(content);
                    return apiResponse ?? new ApiResponse<RoleViewModel> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<RoleViewModel> { IsSuccess = false, Message = "Rol bulunamadı" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<RoleViewModel> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<RoleViewModel>> CreateRoleAsync(CreateRoleViewModel model, CancellationToken cancellationToken = default)
        {
            try
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/roles", content, cancellationToken);
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

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
                return new ApiResponse<RoleViewModel> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<RoleViewModel>> UpdateRoleAsync(string roleId, UpdateRoleViewModel model, CancellationToken cancellationToken = default)
        {
            try
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"api/roles/{roleId}", content, cancellationToken);
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

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
                return new ApiResponse<RoleViewModel> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<bool>> DeleteRoleAsync(string roleId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/roles/{roleId}", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<bool>>(content);
                    return apiResponse ?? new ApiResponse<bool> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<bool> { IsSuccess = false, Message = "Rol silinemedi" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<bool>> EnableRoleAsync(string roleId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/roles/{roleId}/enable", null, cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<bool>>(content);
                    return apiResponse ?? new ApiResponse<bool> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<bool> { IsSuccess = false, Message = "Rol aktifleştirilemedi" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<bool>> DisableRoleAsync(string roleId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/roles/{roleId}/disable", null, cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<bool>>(content);
                    return apiResponse ?? new ApiResponse<bool> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                return new ApiResponse<bool> { IsSuccess = false, Message = "Rol pasifleştirilemedi" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }
    }
}