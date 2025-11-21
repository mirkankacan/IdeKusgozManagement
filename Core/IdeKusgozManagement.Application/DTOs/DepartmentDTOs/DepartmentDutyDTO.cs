using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Application.DTOs.DepartmentDTOs
{
    public class DepartmentDutyDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string DepartmentId { get; set; }
        public DutyScope Scope { get; set; }
    }
}