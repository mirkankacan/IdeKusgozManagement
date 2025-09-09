using IdeKusgozManagement.Application.DTOs.LeaveRequestDTOs;
using IdeKusgozManagement.Application.DTOs.MessageDTOs;
using IdeKusgozManagement.Domain.Entities;
using Mapster;

namespace IdeKusgozManagement.Application.Mappings
{
    public static class MessageMappingConfig
    {
        public static void Configure()
        {
            TypeAdapterConfig<IdtMessage, MessageDTO>
                .NewConfig()
                 .Map(dest => dest.CreatedByName, src => src.CreatedByUser != null
                    ? $"{src.CreatedByUser.Name} {src.CreatedByUser.Surname}"
                    : string.Empty);

            TypeAdapterConfig<CreateMessageDTO, IdtMessage>
                .NewConfig()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.CreatedDate)
                .Ignore(dest => dest.UpdatedDate)
                .Ignore(dest => dest.CreatedBy)
                .Ignore(dest => dest.UpdatedBy);
        }
    }
}