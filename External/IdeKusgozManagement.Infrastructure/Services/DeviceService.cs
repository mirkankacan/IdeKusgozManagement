using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.DeviceDTOs;
using IdeKusgozManagement.Application.DTOs.OptionDTOs;
using IdeKusgozManagement.Application.Interfaces.UnitOfWork;
using IdeKusgozManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class DeviceService(
        IHttpClientFactory httpClientFactory,
        IOptions<OneSignalOptionsDTO> options,
        ILogger<DeviceService> logger,
        IUnitOfWork unitOfWork) : IDeviceService
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("OneSignal");

        public async Task<ServiceResult<bool>> RegisterDeviceAsync(RegisterDeviceDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var subscriptionType = dto.Platform.ToLower() == "android" ? "AndroidPush" : "iOSPush";

                var userPayload = new
                {
                    identity = new
                    {
                        external_id = dto.UserId
                    },
                    properties = new
                    {
                        tags = new
                        {
                            role = dto.UserRole
                        }
                    },
                    subscriptions = new[]
                    {
                new
                {
                    type = subscriptionType,
                    token = dto.DeviceToken,
                    enabled = true
                }
            }
                };

                var json = JsonSerializer.Serialize(userPayload);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"/apps/{options.Value.AppId}/users", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonDocument.Parse(responseContent);
                    var oneSignalId = result.RootElement.GetProperty("identity").GetProperty("onesignal_id").GetString();

                    var existingDevice = await unitOfWork.GetRepository<IdtDevice>()
                        .Where(d => d.UserId == dto.UserId && d.Platform == dto.Platform)
                        .FirstOrDefaultAsync(cancellationToken);

                    if (existingDevice != null)
                    {
                        existingDevice.DeviceToken = dto.DeviceToken;
                        existingDevice.OneSignalPlayerId = oneSignalId;
                        existingDevice.IsActive = true;
                        existingDevice.UpdatedDate = DateTime.Now;

                        unitOfWork.GetRepository<IdtDevice>().Update(existingDevice);
                    }
                    else
                    {
                        var device = new IdtDevice
                        {
                            UserId = dto.UserId,
                            DeviceToken = dto.DeviceToken,
                            Platform = dto.Platform,
                            OneSignalPlayerId = oneSignalId,
                            IsActive = true
                        };

                        await unitOfWork.GetRepository<IdtDevice>().AddAsync(device, cancellationToken);
                    }

                    await unitOfWork.SaveChangesAsync(cancellationToken);

                    logger.LogInformation("Cihaz başarıyla kaydedildi. UserId: {UserId}, Platform: {Platform}, OneSignalId: {OneSignalId}",
                        dto.UserId, dto.Platform, oneSignalId);

                    return ServiceResult<bool>.SuccessAsOk(true);
                }
                else
                {
                    logger.LogError("OneSignal kullanıcı kaydı başarısız. StatusCode: {StatusCode}, Response: {Response}",
                        response.StatusCode, responseContent);

                    return ServiceResult<bool>.Error(
                        "Cihaz Kaydı Başarısız",
                        $"OneSignal hatası: {responseContent}",
                        HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "RegisterDeviceAsync işleminde hata. UserId: {UserId}", dto.UserId);
                throw;
            }
        }
    }
}