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
        public virtual ICollection<IdtLeaveRequest> CreatedLeaveRequests { get; set; } = new List<IdtLeaveRequest>();
        public virtual ICollection<IdtLeaveRequest> UpdatedLeaveRequests { get; set; } = new List<IdtLeaveRequest>();
    }
}