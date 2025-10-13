using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.LeaveRequestModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace IdeKusgozManagement.WebUI.Services
{
    public class LeaveRequestApiService : ILeaveRequestApiService
    {
        private readonly HttpClient _httpClient;

        public LeaveRequestApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<IEnumerable<LeaveRequestViewModel>>> GetLeaveRequestsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync("api/leaveRequests", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<LeaveRequestViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<IEnumerable<LeaveRequestViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<LeaveRequestViewModel>>>(content);
                return errorResponse ?? new ApiResponse<IEnumerable<LeaveRequestViewModel>> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<LeaveRequestViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<IEnumerable<LeaveRequestViewModel>>> GetLeaveRequestsByStatusAsync(int status, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/leaveRequests/status/{status}", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<LeaveRequestViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<IEnumerable<LeaveRequestViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<LeaveRequestViewModel>>>(content);
                return errorResponse ?? new ApiResponse<IEnumerable<LeaveRequestViewModel>> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<LeaveRequestViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<IEnumerable<LeaveRequestViewModel>>> GetMyLeaveRequestsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync("api/leaveRequests/my-leave-requests", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<LeaveRequestViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<IEnumerable<LeaveRequestViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<LeaveRequestViewModel>>>(content);
                return errorResponse ?? new ApiResponse<IEnumerable<LeaveRequestViewModel>> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<LeaveRequestViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<IEnumerable<LeaveRequestViewModel>>> GetLeaveRequestsByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/leaveRequests/user/{userId}", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<LeaveRequestViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<IEnumerable<LeaveRequestViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<LeaveRequestViewModel>>>(content);
                return errorResponse ?? new ApiResponse<IEnumerable<LeaveRequestViewModel>> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<LeaveRequestViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<LeaveRequestViewModel>> GetLeaveRequestByIdAsync(string leaveRequestId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/leaveRequests/{leaveRequestId}", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<LeaveRequestViewModel>>(content);
                    return apiResponse ?? new ApiResponse<LeaveRequestViewModel> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<LeaveRequestViewModel>>(content);
                return errorResponse ?? new ApiResponse<LeaveRequestViewModel> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<LeaveRequestViewModel> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<LeaveRequestViewModel>> CreateLeaveRequestAsync(CreateLeaveRequestViewModel model, CancellationToken cancellationToken = default)
        {
            try
            {
                MultipartFormDataContent? formData = new MultipartFormDataContent();

                // Add form fields
                formData.Add(new StringContent(model.StartDate.ToString("yyyy-MM-dd")), "StartDate");
                formData.Add(new StringContent(model.EndDate.ToString("yyyy-MM-dd")), "EndDate");
                formData.Add(new StringContent(model.Reason), "Reason");

                if (!string.IsNullOrEmpty(model.Description))
                {
                    formData.Add(new StringContent(model.Description), "Description");
                }

                // Add file if exists
                if (model.File?.FormFile != null)
                {
                    var fileContent = new StreamContent(model.File.FormFile.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(model.File.FormFile.ContentType);
                    formData.Add(fileContent, "File.FormFile", model.File.FormFile.FileName);
                }

                var response = await _httpClient.PostAsync("api/leaveRequests", formData, cancellationToken);
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<LeaveRequestViewModel>>(responseContent);
                    return apiResponse ?? new ApiResponse<LeaveRequestViewModel>
                    {
                        IsSuccess = false,
                        Message = "Veri alınamadı"
                    };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<LeaveRequestViewModel>>(responseContent);
                return errorResponse ?? new ApiResponse<LeaveRequestViewModel>
                {
                    IsSuccess = false,
                    Message = $"API çağrısı başarısız: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<LeaveRequestViewModel>
                {
                    IsSuccess = false,
                    Message = "Bir hata oluştu"
                };
            }
        }

        public async Task<ApiResponse<bool>> DeleteLeaveRequestAsync(string leaveRequestId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/leaveRequests/{leaveRequestId}", cancellationToken);
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

        public async Task<ApiResponse<LeaveRequestViewModel>> ApproveLeaveRequestAsync(string leaveRequestId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PutAsync($"api/leaveRequests/{leaveRequestId}/approve", null, cancellationToken);
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<LeaveRequestViewModel>>(responseContent);
                    return apiResponse ?? new ApiResponse<LeaveRequestViewModel> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<LeaveRequestViewModel>>(responseContent);
                return errorResponse ?? new ApiResponse<LeaveRequestViewModel> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<LeaveRequestViewModel> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<LeaveRequestViewModel>> RejectLeaveRequestAsync(string leaveRequestId, string? rejectReason, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PutAsync($"api/leaveRequests/{leaveRequestId}/reject?rejectReason={rejectReason}", null, cancellationToken);
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<LeaveRequestViewModel>>(responseContent);
                    return apiResponse ?? new ApiResponse<LeaveRequestViewModel> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<LeaveRequestViewModel>>(responseContent);
                return errorResponse ?? new ApiResponse<LeaveRequestViewModel> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<LeaveRequestViewModel> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<IEnumerable<LeaveRequestViewModel>>> GetLeaveRequestsByUserIdAndStatusAsync(string userId, int status, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/leaveRequests/user/{userId}/status/{status}", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<LeaveRequestViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<IEnumerable<LeaveRequestViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<LeaveRequestViewModel>>>(content);
                return errorResponse ?? new ApiResponse<IEnumerable<LeaveRequestViewModel>> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<LeaveRequestViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }

        public async Task<ApiResponse<IEnumerable<LeaveRequestViewModel>>> GetMyLeaveRequestsByStatusAsync(int status, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/leaveRequests/my-leave-requests/status/{status}", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<LeaveRequestViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<IEnumerable<LeaveRequestViewModel>> { IsSuccess = false, Message = "Veri alınamadı" };
                }

                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<LeaveRequestViewModel>>>(content);
                return errorResponse ?? new ApiResponse<IEnumerable<LeaveRequestViewModel>> { IsSuccess = false, Message = "API çağrısı başarısız" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<LeaveRequestViewModel>> { IsSuccess = false, Message = "Bir hata oluştu" };
            }
        }
    }
}