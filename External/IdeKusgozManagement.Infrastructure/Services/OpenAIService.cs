using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.AIDTOs;
using IdeKusgozManagement.Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace IdeKusgozManagement.Infrastructure.Services
{
#pragma warning disable OPENAI001

    public class OpenAIService(ChatClient chatClient, ILogger<OpenAIService> logger) : IAIService
    {
        public async Task<ServiceResult<AIDateResponse>> AnalyzeDocumentDateAsync(IFormFile file, string documentTypeName, CancellationToken cancellationToken = default)
        {
            try
            {
                // Input validation
                if (file == null || file.Length == 0)
                    return ServiceResult<AIDateResponse>.Error("Dosya Bulunamadı", "Geçerli dosya bulunamadı.", HttpStatusCode.BadRequest);

                if (string.IsNullOrWhiteSpace(documentTypeName))
                    return ServiceResult<AIDateResponse>.Error("Validasyon Hatası", "Doküman türü belirtilmeli.", HttpStatusCode.BadRequest);

                var binaryData = await FileHelper.ConvertToBinaryDataAsync(file);
                var contentType = FileHelper.GetContentType(Path.GetExtension(file.FileName).ToLowerInvariant());

                var prompt = $@"This is a {documentTypeName} document that requires periodic renewal/monitoring.
Find the START/REFERENCE date that should be used to calculate the next renewal period.
PRIORITY RULES:
1. Report/examination date
2. Document issue date
3. Certification/approval date
4. Test/inspection completion date
5. NEVER use: birth dates, manufacturing dates, irrelevant historical dates
Return ONLY this JSON:
{{""selectedDate"":""dd.MM.yyyy"",""confidence"":0.95,""reasoning"":""why this date is the renewal baseline""}}";

                var messages = new List<ChatMessage>
            {
                new SystemChatMessage("You are a document analysis system. Always respond in valid JSON only."),
                new UserChatMessage(
                    ChatMessageContentPart.CreateTextPart(prompt),
                    ChatMessageContentPart.CreateFilePart(binaryData, contentType, file.FileName)
                )
            };

                var response = await chatClient.CompleteChatAsync(messages, new ChatCompletionOptions
                {
                    Temperature = 0,
                    ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat()
                }, cancellationToken);

                // Null check ekleyin
                var text = response.Value.Content?[0]?.Text;
                if (string.IsNullOrWhiteSpace(text))
                {
                    logger.LogError("OpenAI'dan boş response alındı");
                    return ServiceResult<AIDateResponse>.Error("Yanıt Alınamadı", "OpenAI'dan geçerli yanıt alınamadı. Lütfen daha sonra tekrar deneyin.", HttpStatusCode.BadGateway);
                }

                // JSON'u bul ve çıkart
                var jsonMatch = Regex.Match(text, @"\{.*\}", RegexOptions.Singleline);
                if (!jsonMatch.Success)
                {
                    logger.LogError("OpenAI response'unda JSON bulunamadı. Response: {Response}", text);
                    return ServiceResult<AIDateResponse>.Error("Geçersiz Yanıt", "Geçerli JSON response alınamadı.", HttpStatusCode.BadGateway);
                }

                var result = JsonSerializer.Deserialize<AIDateResponse>(jsonMatch.Value,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (result == null)
                {
                    logger.LogError("JSON deserialize edilemedi: {Json}", jsonMatch.Value);
                    return ServiceResult<AIDateResponse>.Error("Parse Hatası", "JSON parse edilemedi.", HttpStatusCode.BadGateway);
                }

                if (result.Confidence < 0.6)
                {
                    logger.LogWarning("OpenAI güven skoru düşük: {Confidence}", result.Confidence);
                    return ServiceResult<AIDateResponse>.Error("Düşük Güven Skoru", $"Güven skoru çok düşük: {result.Confidence:P0}. Lütfen daha net bir belge yükleyin.", HttpStatusCode.BadRequest);
                }

                logger.LogInformation("Başarılı analiz - Dosya: {FileName}, Tarih: {Date}, Güven: {Confidence}",
                    file.FileName, result.SelectedDate, result.Confidence);

                return ServiceResult<AIDateResponse>.SuccessAsOk(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "AnalyzeDocumentDateAsync işleminde hata - Dosya: {FileName}", file?.FileName);
                throw;
            }
        }
    }
}