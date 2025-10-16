using IdeKusgozManagement.Application.DTOs.NotificationDTOs;
using IdeKusgozManagement.Domain.Entities;
using Mapster;

namespace IdeKusgozManagement.Application.Mappings
{
    public class NotificationMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CreateNotificationDTO, IdtNotification>()
                .Map(dest => dest.TargetUsers,
                     src => src.TargetUsers != null && src.TargetUsers.Count > 0
                            ? string.Join(",", src.TargetUsers)
                            : null)
                .Map(dest => dest.TargetRoles,
                     src => src.TargetRoles != null && src.TargetRoles.Count > 0
                            ? string.Join(",", src.TargetRoles)
                            : null);

            config.NewConfig<IdtNotification, NotificationDTO>()
                .Map(dest => dest.CreatedByFullName, src => $"{src.CreatedByUser.Name} {src.CreatedByUser.Surname}");
        }
    }
}