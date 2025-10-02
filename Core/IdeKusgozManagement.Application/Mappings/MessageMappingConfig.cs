using IdeKusgozManagement.Application.DTOs.MessageDTOs;
using IdeKusgozManagement.Domain.Entities;
using Mapster;

namespace IdeKusgozManagement.Application.Mappings
{
    public class MessageMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<IdtMessage, MessageDTO>()
                 .Map(dest => dest.CreatedByFullName, src => $"{src.CreatedByUser.Name} {src.CreatedByUser.Surname}");

            config.NewConfig<CreateMessageDTO, IdtMessage>()
                .Map(dest => dest.TargetUsers, src => src.TargetUsers != null ? string.Join(";", src.TargetUsers) : null)
                .Map(dest => dest.TargetRoles, src => src.TargetRoles != null ? string.Join(";", src.TargetRoles) : null);
        }
    }
}