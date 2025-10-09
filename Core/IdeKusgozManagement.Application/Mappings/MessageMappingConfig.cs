using IdeKusgozManagement.Application.DTOs.MessageDTOs;
using IdeKusgozManagement.Domain.Entities;
using Mapster;

namespace IdeKusgozManagement.Application.Mappings
{
    public class MessageMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CreateMessageDTO, IdtMessage>()
               .Map(dest => dest.TargetUsers, src => src.TargetUsers != null && src.TargetUsers.Length > 0
                           ? string.Join(",", src.TargetUsers)
                           : null)
               .Map(dest => dest.TargetRoles, src => src.TargetRoles != null && src.TargetRoles.Length > 0
                           ? string.Join(",", src.TargetRoles)
                           : null);

            config.NewConfig<IdtMessage, MessageDTO>()
                .Map(dest => dest.TargetUsers,
                     src => !string.IsNullOrWhiteSpace(src.TargetUsers)
                            ? src.TargetUsers.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                            : null)
                .Map(dest => dest.TargetRoles,
                     src => !string.IsNullOrWhiteSpace(src.TargetRoles)
                            ? src.TargetRoles.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                            : null)
                .Map(dest => dest.CreatedByFullName, src => $"{src.CreatedByUser.Name} {src.CreatedByUser.Surname}");

        }
    }
}