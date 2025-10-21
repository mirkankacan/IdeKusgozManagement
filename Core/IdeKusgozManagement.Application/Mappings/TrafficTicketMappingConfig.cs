using IdeKusgozManagement.Application.DTOs.TrafficTicketDTOs;
using IdeKusgozManagement.Domain.Entities;
using Mapster;

namespace IdeKusgozManagement.Application.Mappings
{
    public class TrafficTicketMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CreateTrafficTicketDTO, IdtTrafficTicket>()
              .Ignore(x => x.File);

            config.NewConfig<UpdateTrafficTicketDTO, IdtTrafficTicket>()
             .Ignore(x => x.File);

            config.NewConfig<IdtTrafficTicket, TrafficTicketDTO>()
                .Map(dest => dest.ProjectName, src => src.Project.Name)
                .Map(dest => dest.EquipmentName, src => src.Equipment.Name)
                .Map(dest => dest.FileId, src => src.File != null ? src.File.Id : null)
                .Map(dest => dest.FilePath, src => src.File != null ? src.File.Path : null)
                .Map(dest => dest.OriginalFileName, src => src.File != null ? src.File.OriginalName : null)
                .Map(dest => dest.CreatedByFullName, src => $"{src.CreatedByUser.Name} {src.CreatedByUser.Surname}")
                .Map(dest => dest.TargetUserFullName, src => src.TargetUser != null ? $"{src.TargetUser.Name} {src.TargetUser.Surname}" : null);
        }
    }
}