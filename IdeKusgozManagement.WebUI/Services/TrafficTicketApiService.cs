using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.TrafficTicketModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace IdeKusgozManagement.WebUI.Services
{
    public class TrafficTicketApiService : ITrafficTicketApiService
    {
        private readonly HttpClient _httpClient;

        public TrafficTicketApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<IEnumerable<TrafficTicketViewModel>>> GetTrafficTicketsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync("api/traffictickets", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<TrafficTicketViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<IEnumerable<TrafficTicketViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<TrafficTicketViewModel>>>(content);
                return errorResponse ?? new ApiResponse<IEnumerable<TrafficTicketViewModel>> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<TrafficTicketViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<TrafficTicketViewModel>> GetTrafficTicketByIdAsync(string trafficTicketId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/traffictickets/{trafficTicketId}", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<TrafficTicketViewModel>>(content);
                    return apiResponse ?? new ApiResponse<TrafficTicketViewModel> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<TrafficTicketViewModel>>(content);
                return errorResponse ?? new ApiResponse<TrafficTicketViewModel> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<TrafficTicketViewModel> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<string>> CreateTrafficTicketAsync(CreateTrafficTicketViewModel model, CancellationToken cancellationToken = default)
        {
            try
            {
                MultipartFormDataContent? formData = new MultipartFormDataContent();

                // Add form fields
                formData.Add(new StringContent(model.ProjectId), "ProjectId");
                formData.Add(new StringContent(model.EquipmentId), "EquipmentId");
                formData.Add(new StringContent(model.Type.ToString()), "Type");
                formData.Add(new StringContent(model.Amount.ToString()), "Amount");
                formData.Add(new StringContent(model.TicketDate.ToString("yyyy-MM-dd")), "TicketDate");

                if (!string.IsNullOrEmpty(model.TargetUserId) && model.Type == 1)
                {
                    formData.Add(new StringContent(model.TargetUserId), "TargetUserId");
                }

                // Add file if exists
                if (model.File?.FormFile != null)
                {
                    var fileContent = new StreamContent(model.File.FormFile.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(model.File.FormFile.ContentType);
                    formData.Add(fileContent, "File.FormFile", model.File.FormFile.FileName);
                }

                var response = await _httpClient.PostAsync("api/traffictickets", formData, cancellationToken);
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<string>>(responseContent);
                    return apiResponse ?? new ApiResponse<string>
                    {
                        IsSuccess = false,
                        Message = "Veri alınamadı"
                    };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<string>>(responseContent);
                return errorResponse ?? new ApiResponse<string>
                {
                    IsSuccess = false,
                    Message = $"API çağrısı başarısız: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>
                {
                    IsSuccess = false,
                    Message = "Bir hata oluştu"
                };
            }
        }

        public async Task<ApiResponse<bool>> UpdateTrafficTicketAsync(string trafficTicketId, UpdateTrafficTicketViewModel model, CancellationToken cancellationToken = default)
        {
            try
            {
                MultipartFormDataContent? formData = new MultipartFormDataContent();

                // Add form fields
                formData.Add(new StringContent(model.ProjectId), "ProjectId");
                formData.Add(new StringContent(model.EquipmentId), "EquipmentId");
                formData.Add(new StringContent(model.Type.ToString()), "Type");
                formData.Add(new StringContent(model.Amount.ToString()), "Amount");
                formData.Add(new StringContent(model.TicketDate.ToString("yyyy-MM-dd")), "TicketDate");

                if (!string.IsNullOrEmpty(model.TargetUserId) && model.Type == 1)
                {
                    formData.Add(new StringContent(model.TargetUserId), "TargetUserId");
                }

                // Add file if exists
                if (model.File?.FormFile != null)
                {
                    var fileContent = new StreamContent(model.File.FormFile.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(model.File.FormFile.ContentType);
                    formData.Add(fileContent, "File.FormFile", model.File.FormFile.FileName);
                }

                var response = await _httpClient.PutAsync($"api/traffictickets/{trafficTicketId}", formData, cancellationToken);
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<bool>>(responseContent);
                    return apiResponse ?? new ApiResponse<bool> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<bool>>(responseContent);
                return errorResponse ?? new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Message = $"API çağrısı başarısız: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Message = "Bir hata oluştu"
                };
            }
        }

        public async Task<ApiResponse<bool>> DeleteTrafficTicketAsync(string trafficTicketId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/traffictickets/{trafficTicketId}", cancellationToken);
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