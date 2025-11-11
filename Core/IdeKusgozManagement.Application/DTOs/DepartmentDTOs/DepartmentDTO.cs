using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Application.DTOs.DepartmentDTOs
{
    public class DepartmentDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DepartmentScope Scope { get; set; }
    }
}