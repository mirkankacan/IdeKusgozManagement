using IdeKusgozManagement.Application.DTOs.ProjectDTOs;
using IdeKusgozManagement.Domain.Entities;
using Mapster;

namespace IdeKusgozManagement.Application.Mappings
{
    public class ProjectMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CreateProjectDTO, IdtProject>()
                .Map(dest => dest.TargetUserIds,
                     src => src.TargetUserIds != null && src.TargetUserIds.Count > 0
                            ? string.Join(",", src.TargetUserIds)
                            : null)
                .Map(dest => dest.TargetEquipmentIds,
                     src => src.TargetEquipmentIds != null && src.TargetEquipmentIds.Count > 0
                            ? string.Join(",", src.TargetEquipmentIds)
                            : null);

            config.NewConfig<UpdateProjectDTO, IdtProject>()
              .Map(dest => dest.TargetUserIds,
                   src => src.TargetUserIds != null && src.TargetUserIds.Count > 0
                          ? string.Join(",", src.TargetUserIds)
                          : null)
              .Map(dest => dest.TargetEquipmentIds,
                   src => src.TargetEquipmentIds != null && src.TargetEquipmentIds.Count > 0
                          ? string.Join(",", src.TargetEquipmentIds)
                          : null);

            config.NewConfig<IdtProject, ProjectDTO>()
                .Map(dest => dest.TargetUserIds, src => src.TargetUserIds != null
                    ? src.TargetUserIds.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                    : null)
                .Map(dest => dest.TargetEquipmentIds, src => src.TargetEquipmentIds != null
                    ? src.TargetEquipmentIds.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                    : null);
        }
    }
}