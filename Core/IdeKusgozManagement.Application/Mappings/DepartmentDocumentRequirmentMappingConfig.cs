using IdeKusgozManagement.Application.DTOs.DocumentDTOs;
using IdeKusgozManagement.Domain.Entities;
using Mapster;

namespace IdeKusgozManagement.Application.Mappings
{
    public class DepartmentDocumentRequirmentMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<IdtDepartmentDocumentRequirment, DepartmentDocumentRequirmentDTO>()
                 .Map(dest => dest.DepartmentName, src => src.Department.Name)
                 .Map(dest => dest.DepartmentDutyName, src => src.DepartmentDuty.Name)
                 .Map(dest => dest.DocumentTypeName, src => src.DocumentType.Name)
                 .Map(dest => dest.CompanyName, src => src.Company != null ? src.Company.Name : null);
        }
    }
}