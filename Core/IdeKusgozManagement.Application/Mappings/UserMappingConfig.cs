using IdeKusgozManagement.Application.DTOs.UserDTOs;
using IdeKusgozManagement.Domain.Entities;
using Mapster;

namespace IdeKusgozManagement.Application.Mappings
{
    public class UserMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<ApplicationUser, UserDTO>()
                .Map(dest => dest.DepartmentName, src => src.Department.Name)
                .Map(dest => dest.DepartmentDutyName, src => src.DepartmentDuty.Name);
        }
    }
}
