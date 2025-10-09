using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.EquipmentModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Newtonsoft.Json;
using System.Text;

namespace IdeKusgozManagement.WebUI.Services
{
    public class EquipmentApiService : IEquipmentApiService
    {
        private readonly HttpClient _httpClient;

        public EquipmentApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<IEnumerable<EquipmentViewModel>>> GetEquipmentsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync("api/equipments", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<EquipmentViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<IEnumerable<EquipmentViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<EquipmentViewModel>>>(content);
                return errorResponse ?? new ApiResponse<IEnumerable<EquipmentViewModel>> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<EquipmentViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<EquipmentViewModel>> GetEquipmentByIdAsync(string equipmentId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/equipments/{equipmentId}", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<EquipmentViewModel>>(content);
                    return apiResponse ?? new ApiResponse<EquipmentViewModel> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<EquipmentViewModel>>(content);
                return errorResponse ?? new ApiResponse<EquipmentViewModel> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<EquipmentViewModel> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<string>> CreateEquipmentAsync(CreateEquipmentViewModel model, CancellationToken cancellationToken = default)
        {
            try
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/equipments", content, cancellationToken);
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<string>>(responseContent);
                    return apiResponse ?? new ApiResponse<string> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<string>>(responseContent);
                return errorResponse ?? new ApiResponse<string> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<string> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<bool>> UpdateEquipmentAsync(string equipmentId, UpdateEquipmentViewModel model, CancellationToken cancellationToken = default)
        {
            try
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"api/equipments/{equipmentId}", content, cancellationToken);
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<bool>>(responseContent);
                    return apiResponse ?? new ApiResponse<bool> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<bool>>(responseContent);
                return errorResponse ?? new ApiResponse<bool> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<bool>> DeleteEquipmentAsync(string equipmentId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/equipments/{equipmentId}", cancellationToken);
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<bool>>(responseContent);
                    return apiResponse ?? new ApiResponse<bool> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<bool>>(responseContent);
                return errorResponse ?? new ApiResponse<bool> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<IEnumerable<EquipmentViewModel>>> GetActiveEquipmentsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync("api/equipments/active-equipments", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<EquipmentViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<IEnumerable<EquipmentViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<EquipmentViewModel>>>(content);
                return errorResponse ?? new ApiResponse<IEnumerable<EquipmentViewModel>> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<EquipmentViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<bool>> EnableEquipmentAsync(string equipmentId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PutAsync($"api/equipments/{equipmentId}/enable", null, cancellationToken);
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<bool>>(responseContent);
                    return apiResponse ?? new ApiResponse<bool> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<bool>>(responseContent);
                return errorResponse ?? new ApiResponse<bool> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<bool>> DisableEquipmentAsync(string equipmentId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PutAsync($"api/equipments/{equipmentId}/disable", null, cancellationToken);
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<bool>>(responseContent);
                    return apiResponse ?? new ApiResponse<bool> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<bool>>(responseContent);
                return errorResponse ?? new ApiResponse<bool> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }
    }
}