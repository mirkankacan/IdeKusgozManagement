using IdeKusgozManagement.Application.DTOs.NotificationDTOs;
using IdeKusgozManagement.Domain.Entities;
using Mapster;

namespace IdeKusgozManagement.Application.Mappings
{
    public static class NotificationMappingConfig
    {
        public static void Configure()
        {
            TypeAdapterConfig<IdtNotification, NotificationDTO>
              .NewConfig()
              .Map(dest => dest.CreatedByName, src => src.CreatedByUser != null
                  ? $"{src.CreatedByUser.Name} {src.CreatedByUser.Surname}"
                  : string.Empty)
              .Map(dest => dest.IsRead, src => src.NotificationReads != null && src.NotificationReads.Any())
              .Map(dest => dest.ReadDate, src => src.NotificationReads != null && src.NotificationReads.Any()
                  ? src.NotificationReads.First().CreatedDate
                  : (DateTime?)null);

            TypeAdapterConfig<CreateNotificationDTO, IdtNotification>
                .NewConfig()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.CreatedDate)
                .Ignore(dest => dest.UpdatedDate)
                .Ignore(dest => dest.CreatedBy)
                .Ignore(dest => dest.UpdatedBy)
                .Ignore(dest => dest.CreatedByUser);
        }
    }
}