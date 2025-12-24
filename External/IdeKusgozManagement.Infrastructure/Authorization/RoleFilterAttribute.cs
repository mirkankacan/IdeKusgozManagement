using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.Infrastructure.Authorization
{
    public class RoleFilterAttribute : TypeFilterAttribute
    {
        public RoleFilterAttribute(params string[] roles) : base(typeof(AsyncRoleAttribute))
        {
            Arguments = new object[] { roles };
        }
    }
}