using IdeKusgozManagement.Application.DTOs.RoleDTOs;
using IdeKusgozManagement.Domain.Entities;
using Mapster;

namespace IdeKusgozManagement.Application.Mappings
{
    public static class RoleMappingConfig
    {
        public static void Configure()
        {
            TypeAdapterConfig<ApplicationRole, RoleDTO>
                .NewConfig();

            TypeAdapterConfig<CreateRoleDTO, ApplicationRole>
                .NewConfig()
                .Map(dest => dest.IsActive, src => true)
                .Ignore(dest => dest.Id);

            TypeAdapterConfig<UpdateRoleDTO, ApplicationRole>
                .NewConfig()
                .Ignore(dest => dest.Id);
        }
    }
}