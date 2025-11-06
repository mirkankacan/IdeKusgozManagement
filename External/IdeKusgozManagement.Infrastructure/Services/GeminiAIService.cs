using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.AIDTOs;
using IdeKusgozManagement.Application.DTOs.OptionDTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class GeminiAIService(HttpClient httpClient, IOptions<GeminiAiOptionsDTO> aiOptions, ILogger<GeminiAIService> logger) : IAIService
    {
        public async Task<ApiResponse<AIDateResponse>> AnalyzeDocumentDateAsync(byte[] documentBytes, string contentType, string documentTypeName)
        {
            // Too Many Requests hatasını bypass etmek için
            await Task.Delay(TimeSpan.FromSeconds(2));
            try
            {
                var prompt = $@"Bu dökümanı analiz et. Belge türü: {documentTypeName}
Dökümandaki tarihlerden hangisi bu belgenin başlangıç tarihi olmalı?
KURALLAR:
- Doğum tarihi ASLA seçme
- Belge düzenleme/veriliş tarihi öncelikli
- Eski tarihlerden kaçın
- Bu dökümanın belirli periyotlarda yenilenmesi gerekiyor olduğunu düşün. Bunun bir başlangıç tarihi var. Başlangıç tarihi evrağın verildiği, eğitim tarihi vs. olabilir
Sadece JSON ver: {{""selectedDate"":""dd.MM.yyyy"",""confidence"":0.9,""reasoning"":""açıklama""}}";

                // API call
                var base64Document = Convert.ToBase64String(documentBytes);
                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new object[]
                            {
                                new { text = prompt },
                                new { inline_data = new { mime_type = contentType, data = base64Document } }
                            }
                        }
                    }
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var opts = aiOptions.Value;
                var response = await httpClient.PostAsync($"{opts.ApiUrl}?key={opts.ApiKey}", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    logger.LogError("Gemini API hata döndürdü: {StatusCode} - {Content}", response.StatusCode, errorContent);
                    return ApiResponse<AIDateResponse>.Error($"Gemini API hatası: {response.StatusCode}");
                }

                var responseText = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(responseText))
                {
                    return ApiResponse<AIDateResponse>.Error("Gemini AI cevap okunamadı");
                }

                logger.LogInformation("Gemini API Yanıtı: {Response}", responseText);

                // Parse response with better error handling
                try
                {
                    var geminiResponse = JsonSerializer.Deserialize<JsonDocument>(responseText);

                    // Check if response has the expected structure
                    if (!geminiResponse.RootElement.TryGetProperty("candidates", out var candidatesElement) ||
                        candidatesElement.GetArrayLength() == 0)
                    {
                        logger.LogError("Gemini yanıtında candidates dizisi bulunamadı");
                        return ApiResponse<AIDateResponse>.Error("Gemini AI geçersiz yanıt yapısı");
                    }

                    var firstCandidate = candidatesElement[0];

                    if (!firstCandidate.TryGetProperty("content", out var contentElement))
                    {
                        logger.LogError("Gemini yanıtında candidate içinde content bulunamadı");
                        return ApiResponse<AIDateResponse>.Error("Gemini AI geçersiz yanıt yapısı - content bulunamadı");
                    }

                    if (!contentElement.TryGetProperty("parts", out var partsElement) ||
                        partsElement.GetArrayLength() == 0)
                    {
                        logger.LogError("Gemini yanıtında parts dizisi bulunamadı");
                        return ApiResponse<AIDateResponse>.Error("Gemini AI geçersiz yanıt yapısı - parts bulunamadı");
                    }

                    var firstPart = partsElement[0];

                    if (!firstPart.TryGetProperty("text", out var textElement))
                    {
                        logger.LogError("Gemini yanıtında part içinde text bulunamadı");
                        return ApiResponse<AIDateResponse>.Error("Gemini AI geçersiz yanıt yapısı - text bulunamadı");
                    }

                    var text = textElement.GetString();

                    if (string.IsNullOrEmpty(text))
                    {
                        logger.LogError("Gemini yanıtında text alanı boş");
                        return ApiResponse<AIDateResponse>.Error("Gemini AI boş yanıt döndürdü");
                    }

                    logger.LogInformation("Gemini'den çıkarılan metin: {Text}", text);

                    // JSON extract with better regex
                    var jsonMatch = System.Text.RegularExpressions.Regex.Match(text, @"\{[^{}]*(?:\{[^{}]*\}[^{}]*)*\}",
                        System.Text.RegularExpressions.RegexOptions.Singleline);

                    if (!jsonMatch.Success)
                    {
                        logger.LogError("Gemini yanıt metninde JSON bulunamadı: {Text}", text);
                        return ApiResponse<AIDateResponse>.Error("Gemini AI yanıtında JSON bulunamadı");
                    }

                    var jsonText = jsonMatch.Value;
                    logger.LogInformation("Çıkarılan JSON: {Json}", jsonText);

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var result = JsonSerializer.Deserialize<AIDateResponse>(jsonText, options);

                    if (result == null)
                    {
                        logger.LogError("JSON'ın AIDateResponse'a dönüştürülmesi başarısız");
                        return ApiResponse<AIDateResponse>.Error("Gemini AI yanıtı çözümlenemedi");
                    }

                    if (result.Confidence < 0.6)
                    {
                        logger.LogWarning("Gemini AI güven skoru çok düşük: {Confidence}", result.Confidence);
                        return ApiResponse<AIDateResponse>.Error("Gemini AI hangi tarih olduğundan yeterince emin değil");
                    }

                    var dateResponse = new AIDateResponse
                    {
                        SelectedDate = result.SelectedDate,
                        Confidence = result.Confidence,
                        Reasoning = result.Reasoning
                    };

                    return ApiResponse<AIDateResponse>.Success(dateResponse, "Gemini AI ile tarih bulma işlemi başarıyla gerçekleşti");
                }
                catch (JsonException jsonEx)
                {
                    logger.LogError(jsonEx, "Gemini yanıtında JSON ayrıştırma hatası: {Error}", jsonEx.Message);
                    return ApiResponse<AIDateResponse>.Error("Gemini AI yanıtı JSON formatında değil");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "AnalyzeDocumentDateAsync işleminde hata: {Error}", ex.Message);
                return ApiResponse<AIDateResponse>.Error("Gemini AI ile tarih bulma işleminde beklenmeyen hata");
            }
        }
    }
}