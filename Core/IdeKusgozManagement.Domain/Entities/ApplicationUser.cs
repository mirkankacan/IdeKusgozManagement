using Microsoft.AspNetCore.Identity;

namespace IdeKusgozManagement.Domain.Entities
{
    public class ApplicationUser : IdentityUser<string>
    {
        public ApplicationUser()
        {
            Id = Guid.NewGuid().ToString();
            IsActive = true;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public bool IsActive { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpires { get; set; }
        public string TCNo { get; set; }
        public bool IsExpatriate { get; set; }
        public string? PasswordResetCode { get; set; }
        public DateTime? PasswordResetCodeExpires { get; set; }
        public DateTime HireDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public string DepartmentId { get; set; }
        public virtual IdtDepartment Department { get; set; }
        public string DepartmentDutyId { get; set; }
        public virtual IdtDepartmentDuty DepartmentDuty { get; set; }
    }
}