using IdeKusgozManagement.WebUI.Models;
using IdeKusgozManagement.WebUI.Models.AuthModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace IdeKusgozManagement.WebUI.Handlers
{
    public class JwtTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<JwtTokenHandler> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public JwtTokenHandler(IHttpContextAccessor httpContextAccessor,
            ILogger<JwtTokenHandler> logger,
            IHttpClientFactory httpClientFactory)
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
                var httpClient = _httpClientFactory.CreateClient("AuthApiWithoutToken");

                var refreshRequest = new CreateTokenByRefreshTokenViewModel
                {
                    UserId = userId,
                    RefreshToken = refreshToken
                };

                var json = JsonConvert.SerializeObject(refreshRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("api/auth/refresh-token", content, cancellationToken);
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<TokenViewModel>>(responseContent);

                    if (apiResponse?.IsSuccess == true && !string.IsNullOrEmpty(apiResponse.Data?.Token))
                    {
                        // Session'ı thread-safe şekilde güncelle
                        var httpContext = _httpContextAccessor.HttpContext;
                        if (httpContext != null)
                        {
                            lock (httpContext.Session)
                            {
                                httpContext.Session.SetString("JwtToken", apiResponse.Data.Token);
                                httpContext.Session.SetString("UserId", apiResponse.Data.UserId);
                                httpContext.Session.SetString("TCNo", apiResponse.Data.TCNo);
                                httpContext.Session.SetString("FullName", apiResponse.Data.FullName);
                                httpContext.Session.SetString("RoleName", apiResponse.Data.RoleName);
                                httpContext.Session.SetString("FullNameWithExp", apiResponse.Data.FullNameWithExp);

                                if (!string.IsNullOrEmpty(apiResponse.Data.RefreshToken))
                                {
                                    httpContext.Session.SetString("RefreshToken", apiResponse.Data.RefreshToken);
                                }
                            }
                        }

                        _logger.LogInformation("Token başarıyla yenilendi - UserId: {UserId}", userId);
                        return apiResponse.Data.Token;
                    }
                    else
                    {
                        _logger.LogWarning("Refresh token başarısız - Message: {Message}, Errors: {Errors}",
                            apiResponse?.Message, string.Join(", ", apiResponse?.Errors ?? new List<string>()));
                    }
                }
                else
                {
                    _logger.LogWarning("Refresh token API çağrısı başarısız - StatusCode: {StatusCode}, Content: {Content}",
                        response.StatusCode, responseContent);
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
                    // Session'ı önce temizle
                    httpContext.Session.Clear();

                    // Cookie authentication'ı temizle
                    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                    // API logout'u en son yap (başarısız olsa da önemli değil)
                    try
                    {
                        var httpClient = _httpClientFactory.CreateClient("AuthApiWithoutToken");
                        await httpClient.PostAsync("api/auth/logout", null);
                    }
                    catch (Exception logoutEx)
                    {
                        _logger.LogWarning(logoutEx, "API logout başarısız oldu, ancak local logout devam etti");
                    }

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
                    }), Encoding.UTF8, "application/json")
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

        private static bool IsAjaxRequest(HttpRequest request)
        {
            return request.Headers.ContainsKey("X-Requested-With") &&
                   request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                   request.Headers["Content-Type"].ToString().Contains("application/json") ||
                   request.Path.StartsWithSegments("/api") ||
                   request.Headers["Accept"].ToString().Contains("application/json");
        }

        private static async Task<HttpRequestMessage> CloneRequestAsync(HttpRequestMessage originalRequest)
        {
            var clonedRequest = new HttpRequestMessage(originalRequest.Method, originalRequest.RequestUri);

            // Headers'ları kopyala (Authorization hariç)
            foreach (var header in originalRequest.Headers.Where(h => h.Key != "Authorization"))
            {
                clonedRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            // Content'i kopyala - sadece gerekirse
            if (originalRequest.Content != null)
            {
                try
                {
                    var contentBytes = await originalRequest.Content.ReadAsByteArrayAsync();
                    clonedRequest.Content = new ByteArrayContent(contentBytes);

                    // Content headers'ını kopyala
                    foreach (var header in originalRequest.Content.Headers)
                    {
                        clonedRequest.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }
                catch (Exception)
                {
                    // Content okunamadıysa null bırak
                    clonedRequest.Content = null;
                }
            }

            return clonedRequest;
        }
    }
}