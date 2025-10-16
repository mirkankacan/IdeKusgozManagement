using IdeKusgozManagement.Application.DTOs.WorkRecordDTOs;
using IdeKusgozManagement.Domain.Entities;
using Mapster;

namespace IdeKusgozManagement.Application.Mappings
{
    public class WorkRecordMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<IdtWorkRecord, WorkRecordDTO>()
                 .Map(dest => dest.CreatedByFullName, src => $"{src.CreatedByUser.Name} {src.CreatedByUser.Surname}")
                 .Map(dest => dest.UpdatedByFullName, src => src.UpdatedByUser != null ? $"{src.UpdatedByUser.Name} {src.UpdatedByUser.Surname}" : null)
                 .Map(dest => dest.EquipmentName, src => src.Equipment.Name)
                 .Map(dest => dest.ProjectName, src => src.Project.Name);
        }
    }
}