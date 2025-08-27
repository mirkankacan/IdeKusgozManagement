using IdeKusgozManagement.Application.DTOs.EquipmentDTOs;
using IdeKusgozManagement.Domain.Entities;
using Mapster;

namespace IdeKusgozManagement.Application.Mappings
{
    public static class EquipmentMappingConfig
    {
        public static void Configure()
        {
            // IdtEquipment -> EquipmentListDTO
            TypeAdapterConfig<IdtEquipment, EquipmentDTO>
                .NewConfig();

            // CreateEquipmentDTO -> IdtEquipment
            TypeAdapterConfig<CreateEquipmentDTO, IdtEquipment>
                .NewConfig()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.CreatedDate)
                .Ignore(dest => dest.UpdatedDate)
                .Ignore(dest => dest.CreatedBy)
                .Ignore(dest => dest.UpdatedBy);

            // UpdateEquipmentDTO -> IdtEquipment
            TypeAdapterConfig<UpdateEquipmentDTO, IdtEquipment>
                .NewConfig()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.CreatedDate)
                .Ignore(dest => dest.UpdatedDate)
                .Ignore(dest => dest.CreatedBy)
                .Ignore(dest => dest.UpdatedBy);
        }
    }
}