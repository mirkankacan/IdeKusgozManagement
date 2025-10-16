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
    }
}