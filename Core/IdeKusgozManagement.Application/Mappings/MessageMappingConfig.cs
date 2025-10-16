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
       .Map(dest => dest.TargetUsers, src => src.TargetUsers != null && src.TargetUsers.Count > 0
                   ? string.Join(",", src.TargetUsers)
                   : null)
       .Map(dest => dest.TargetRoles, src => src.TargetRoles != null && src.TargetRoles.Count > 0
                   ? string.Join(",", src.TargetRoles)
                   : null);


        }
    }
}