using Microsoft.AspNetCore.Identity;

namespace IdeKusgozManagement.Domain.Entities
{
    public class ApplicationRole : IdentityRole<string>
    {
        public ApplicationRole()
        {
            Id = Guid.NewGuid().ToString();
            IsActive = true;
        }

        public bool IsActive { get; set; }
    }
}