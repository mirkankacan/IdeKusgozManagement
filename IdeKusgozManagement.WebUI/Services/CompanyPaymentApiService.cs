using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.CompanyPaymentModels;
using IdeKusgozManagement.WebUI.Services.Interfaces;
using System.Net.Http;

namespace IdeKusgozManagement.WebUI.Services
{
    public class CompanyPaymentApiService : ICompanyPaymentApiService
    {
        private readonly IApiService _apiService;
        private readonly ILogger<CompanyPaymentApiService> _logger;
        private const string BaseEndpoint = "api/companyPayments";

        public CompanyPaymentApiService(
            IApiService apiService,
            ILogger<CompanyPaymentApiService> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<CompanyPaymentViewModel>>> GetCompanyPaymentsAsync(CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<CompanyPaymentViewModel>>(BaseEndpoint, cancellationToken);
        }

        public async Task<ApiResponse<CompanyPaymentViewModel>> GetCompanyPaymentByIdAsync(string companyPaymentId, CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<CompanyPaymentViewModel>($"{BaseEndpoint}/{companyPaymentId}", cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<CompanyPaymentViewModel>>> GetCompanyPaymentsByStatusAsync(int status, CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<CompanyPaymentViewModel>>($"{BaseEndpoint}/by-status/{status}", cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<CompanyPaymentViewModel>>> GetMyCompanyPaymentsAsync(CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<CompanyPaymentViewModel>>($"{BaseEndpoint}/my-company-payments", cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<CompanyPaymentViewModel>>> GetCompanyPaymentsByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _apiService.GetAsync<IEnumerable<CompanyPaymentViewModel>>($"{BaseEndpoint}/user/{userId}", cancellationToken);
        }

        public async Task<ApiResponse<string>> CreateCompanyPaymentAsync(CreateCompanyPaymentViewModel model, CancellationToken cancellationToken = default)
        {
            var formData = new MultipartFormDataContent();

            if (!string.IsNullOrEmpty(model.EquipmentId))
            {
                formData.Add(new StringContent(model.EquipmentId), "EquipmentId");
            }

            formData.Add(new StringContent(model.Amount.ToString()), "Amount");
            formData.Add(new StringContent(model.ExpenseId), "ExpenseId");
            formData.Add(new StringContent(model.ProjectId), "ProjectId");

            if (!string.IsNullOrEmpty(model.PersonnelNote))
            {
                formData.Add(new StringContent(model.PersonnelNote), "PersonnelNote");
            }

            if (!string.IsNullOrEmpty(model.SelectedApproverId))
            {
                formData.Add(new StringContent(model.SelectedApproverId), "SelectedApproverId");
            }

            // Dosyaları ekle
            if (model.Files != null && model.Files.Any())
            {
                foreach (var file in model.Files)
                {
                    if (file != null && file.Length > 0)
                    {
                        var fileContent = new StreamContent(file.OpenReadStream());
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                        formData.Add(fileContent, "Files", file.FileName);
                    }
                }
            }

            return await _apiService.PostMultipartAsync<string>(BaseEndpoint, formData, cancellationToken);
        }

        public async Task<ApiResponse<bool>> UpdateCompanyPaymentAsync(string companyPaymentId, UpdateCompanyPaymentViewModel model, CancellationToken cancellationToken = default)
        {
            var formData = new MultipartFormDataContent();

            if (!string.IsNullOrEmpty(model.EquipmentId))
            {
                formData.Add(new StringContent(model.EquipmentId), "EquipmentId");
            }

            formData.Add(new StringContent(model.Amount.ToString()), "Amount");
            formData.Add(new StringContent(model.ExpenseId), "ExpenseId");
            formData.Add(new StringContent(model.ProjectId), "ProjectId");

            if (!string.IsNullOrEmpty(model.ChiefNote))
            {
                formData.Add(new StringContent(model.ChiefNote), "ChiefNote");
            }

            if (!string.IsNullOrEmpty(model.PersonnelNote))
            {
                formData.Add(new StringContent(model.PersonnelNote), "PersonnelNote");
            }

            if (!string.IsNullOrEmpty(model.SelectedApproverId))
            {
                formData.Add(new StringContent(model.SelectedApproverId), "SelectedApproverId");
            }

            formData.Add(new StringContent(model.Status.ToString()), "Status");

            // Dosyayı ekle
            if (model.File != null && model.File.Length > 0)
            {
                var fileContent = new StreamContent(model.File.OpenReadStream());
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(model.File.ContentType);
                formData.Add(fileContent, "File.FormFile", model.File.FileName);
            }

            return await _apiService.PutMultipartAsync<bool>($"{BaseEndpoint}/{companyPaymentId}", formData, cancellationToken);
        }

        public async Task<ApiResponse<bool>> DeleteCompanyPaymentAsync(string companyPaymentId, CancellationToken cancellationToken = default)
        {
            return await _apiService.DeleteAsync<bool>($"{BaseEndpoint}/{companyPaymentId}", cancellationToken);
        }

        public async Task<ApiResponse<bool>> ApproveCompanyPaymentAsync(string companyPaymentId, string? chiefNote = null, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/{companyPaymentId}/approve";
            if (!string.IsNullOrEmpty(chiefNote))
            {
                endpoint += $"?chiefNote={Uri.EscapeDataString(chiefNote)}";
            }
            return await _apiService.PutAsync<bool>(endpoint, null, cancellationToken);
        }

        public async Task<ApiResponse<bool>> RejectCompanyPaymentAsync(string companyPaymentId, string? rejectReason = null, CancellationToken cancellationToken = default)
        {
            var endpoint = $"{BaseEndpoint}/{companyPaymentId}/reject";
            if (!string.IsNullOrEmpty(rejectReason))
            {
                endpoint += $"?rejectReason={Uri.EscapeDataString(rejectReason)}";
            }
            return await _apiService.PutAsync<bool>(endpoint, null, cancellationToken);
        }
    }
}
