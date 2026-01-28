using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.OptionDTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class OneSignalService(IHttpClientFactory httpClientFactory, IOptions<OneSignalOptionsDTO> options, ILogger<OneSignalService> logger) : IOneSignalService
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("OneSignal");

        public async Task SendNotificationAsync(string message, string heading,
            List<string>? userIds = null, List<string>? roles = null,
            Dictionary<string, object>? additionalData = null)
        {
            try
            {
                object payload;

                // Kullanıcı bazlı (external_id ile — YENİ YÖNTEM)
                if (userIds?.Any() == true)
                {
                    payload = new
                    {
                        app_id = options.Value.AppId,

                        include_aliases = new
                        {
                            external_id = userIds
                        },
                        target_channel = "push",

                        contents = new { en = message },
                        headings = new { en = heading },
                        data = additionalData
                    };
                }
                // Rol bazlı (TAG FILTER kullanarak)
                else if (roles?.Any() == true)
                {
                    var filters = BuildRoleFilters(roles);

                    payload = new
                    {
                        app_id = options.Value.AppId,
                        contents = new { en = message },
                        headings = new { en = heading },
                        filters = filters,
                        data = additionalData
                    };
                }
                // Herkese
                else
                {
                    payload = new
                    {
                        app_id = options.Value.AppId,
                        contents = new { en = message },
                        headings = new { en = heading },
                        included_segments = new[] { "All" },
                        data = additionalData
                    };
                }

                var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                });

                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(requestUri: "/notifications?c=push", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = JsonDocument.Parse(responseContent);

                    if (jsonResponse.RootElement.TryGetProperty("errors", out var errors))
                    {
                        var errorList = errors.EnumerateArray().Select(e => e.GetString()).ToList();

                        if (errorList.Any(e => e?.Contains("not subscribed") == true))
                        {
                            logger.LogWarning("OneSignal bildirimi gönderildi ancak kayıtlı cihaz bulunamadı. Message: {Message}", message);
                        }
                        else
                        {
                            logger.LogError("OneSignal bildirimi gönderilemedi. Errors: {Errors}",
                                string.Join(", ", errorList));
                        }
                    }
                    else
                    {
                        logger.LogInformation("OneSignal bildirimi başarıyla gönderildi. Response: {Response}", responseContent);
                    }
                }
                else
                {
                    logger.LogError("OneSignal bildirimi gönderilemedi. StatusCode: {StatusCode}, Response: {Response}",
                        response.StatusCode, responseContent);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "OneSignal bildirimi gönderilemedi. Message: {Message}", message);
                throw;
            }
        }

        private List<object> BuildRoleFilters(List<string> roles)
        {
            var filters = new List<object>();

            for (int i = 0; i < roles.Count; i++)
            {
                filters.Add(new
                {
                    field = "tag",
                    key = "role",
                    relation = "=",
                    value = roles[i]
                });

                // Son eleman değilse OR ekle
                if (i < roles.Count - 1)
                {
                    filters.Add(new { @operator = "OR" });
                }
            }

            return filters;
        }
    }
}