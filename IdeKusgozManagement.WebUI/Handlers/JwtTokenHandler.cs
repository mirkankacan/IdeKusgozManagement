using System.Net.Http.Headers;
using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.AuthModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json;

namespace IdeKusgozManagement.WebUI.Handlers
{
    public class JwtTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<JwtTokenHandler> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public JwtTokenHandler(IHttpContextAccessor httpContextAccessor, ILogger<JwtTokenHandler> logger, IHttpClientFactory httpClientFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JwtToken");

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await base.SendAsync(request, cancellationToken);

            // Token süresi dolmuşsa refresh token deneyin
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized && !string.IsNullOrEmpty(token))
            {
                var refreshToken = _httpContextAccessor.HttpContext?.Session.GetString("RefreshToken");
                var userId = _httpContextAccessor.HttpContext?.Session.GetString("UserId");

                if (!string.IsNullOrEmpty(refreshToken) && !string.IsNullOrEmpty(userId))
                {
                    var newToken = await TryRefreshTokenAsync(userId, refreshToken, cancellationToken);

                    if (!string.IsNullOrEmpty(newToken))
                    {
                        // Yeni token ile tekrar deneyin
                        var newRequest = await CloneRequestAsync(request);
                        newRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newToken);
                        response = await base.SendAsync(newRequest, cancellationToken);

                        _logger.LogInformation("Token başarıyla yenilendi ve istek tekrarlandı");
                    }
                    else
                    {
                        // Refresh token da başarısız - kullanıcıyı logout et
                        _logger.LogWarning("Refresh token başarısız, kullanıcı logout ediliyor");
                        await ForceLogoutAndRedirect();
                        return CreateRedirectResponse();
                    }
                }
                else
                {
                    // Refresh token yok - kullanıcıyı logout et
                    _logger.LogWarning("Refresh token bulunamadı, kullanıcı logout ediliyor");
                    await ForceLogoutAndRedirect();
                    return CreateRedirectResponse();
                }
            }

            return response;
        }

        private async Task<string?> TryRefreshTokenAsync(string userId, string refreshToken, CancellationToken cancellationToken)
        {
            try
            {
                // AuthApi HttpClient'ı kullan (JwtTokenHandler olmadan)
                using var httpClient = _httpClientFactory.CreateClient("AuthApi");

                var refreshRequest = new
                {
                    UserId = userId,
                    RefreshToken = refreshToken
                };

                var json = JsonConvert.SerializeObject(refreshRequest);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                // Doğru endpoint kullan
                var response = await httpClient.PostAsync("api/Auth/refresh-token", content, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<TokenViewModel>>(responseContent);

                    if (apiResponse?.IsSuccess == true && !string.IsNullOrEmpty(apiResponse.Data.Token))
                    {
                        // Session'ı güncelle
                        _httpContextAccessor.HttpContext?.Session.SetString("JwtToken", apiResponse.Data.Token);
                        _httpContextAccessor.HttpContext?.Session.SetString("UserId", apiResponse.Data.UserId);
                        _httpContextAccessor.HttpContext?.Session.SetString("UserName", apiResponse.Data.UserName);
                        _httpContextAccessor.HttpContext?.Session.SetString("FullName", apiResponse.Data.FullName);
                        _httpContextAccessor.HttpContext?.Session.SetString("RoleName", apiResponse.Data.RoleName);

                        if (!string.IsNullOrEmpty(apiResponse.Data.RefreshToken))
                        {
                            _httpContextAccessor.HttpContext?.Session.SetString("RefreshToken", apiResponse.Data.RefreshToken);
                        }

                        _logger.LogInformation("Token başarıyla yenilendi - UserId: {UserId}", userId);
                        return apiResponse.Data.Token;
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogWarning("Refresh token başarısız - StatusCode: {StatusCode}, Content: {Content}",
                        response.StatusCode, errorContent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Refresh token işlemi sırasında hata oluştu");
            }

            return null;
        }

        private async Task ForceLogoutAndRedirect()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                try
                {
                    // Session'ı tamamen temizle
                    httpContext.Session.Clear();

                    // Cookie authentication'ı temizle
                    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                    _logger.LogInformation("Kullanıcı zorla logout edildi");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Logout işlemi sırasında hata oluştu");
                }
            }
        }

        private HttpResponseMessage CreateRedirectResponse()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext != null && IsAjaxRequest(httpContext.Request))
            {
                // AJAX request için JSON response
                return new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new
                    {
                        message = "Oturum süresi doldu. Lütfen tekrar giriş yapın.",
                        redirectToLogin = true,
                        loginUrl = "/giris-yap"
                    }), System.Text.Encoding.UTF8, "application/json")
                };
            }
            else
            {
                // Normal HTTP request için redirect response
                var response = new HttpResponseMessage(System.Net.HttpStatusCode.Redirect);
                response.Headers.Location = new Uri("/giris-yap", UriKind.Relative);
                return response;
            }
        }

        private bool IsAjaxRequest(HttpRequest request)
        {
            return request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                   request.Headers["Content-Type"].ToString().Contains("application/json") ||
                   request.Path.StartsWithSegments("/api") ||
                   request.Headers["Accept"].ToString().Contains("application/json");
        }

        private async Task<HttpRequestMessage> CloneRequestAsync(HttpRequestMessage originalRequest)
        {
            var clonedRequest = new HttpRequestMessage(originalRequest.Method, originalRequest.RequestUri);

            // Headers'ları kopyala (Authorization hariç)
            foreach (var header in originalRequest.Headers.Where(h => h.Key != "Authorization"))
            {
                clonedRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            // Content'i kopyala
            if (originalRequest.Content != null)
            {
                var contentBytes = await originalRequest.Content.ReadAsByteArrayAsync();
                clonedRequest.Content = new ByteArrayContent(contentBytes);

                // Content headers'ını kopyala
                foreach (var header in originalRequest.Content.Headers)
                {
                    clonedRequest.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            return clonedRequest;
        }
    }
}