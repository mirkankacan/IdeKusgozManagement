using IdeKusgozManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace IdeKusgozManagement.Infrastructure.Authorization
{
    public sealed class RoleAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string[] _roles;

        public RoleAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim?.Value == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var userId = userIdClaim.Value;
            var roleService = context.HttpContext.RequestServices.GetRequiredService<IRoleService>();

            var hasRequiredRole = false;

            foreach (var role in _roles)
            {
                var isInRole = await roleService.IsUserInRoleAsync(userId, role);
                if (isInRole.Data)
                {
                    hasRequiredRole = true;
                    break;
                }
            }

            if (!hasRequiredRole)
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }

    public sealed class AsyncRoleAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string[] _roles;

        public AsyncRoleAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim?.Value == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var userId = userIdClaim.Value;
            var roleService = context.HttpContext.RequestServices.GetRequiredService<IRoleService>();

            var hasRequiredRole = false;

            foreach (var role in _roles)
            {
                var isInRole = await roleService.IsUserInRoleAsync(userId, role);
                if (isInRole.Data)
                {
                    hasRequiredRole = true;
                    break;
                }
            }

            if (!hasRequiredRole)
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}