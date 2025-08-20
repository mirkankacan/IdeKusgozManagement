using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace IdeKusgozManagement.Infrastructure.Helpers
{
    public static class CurrentUserHelper
    {
        private static IHttpContextAccessor? _httpContextAccessor;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static string? GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public static string? GetCurrentUserName()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
        }

        public static bool IsAuthenticated()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        }

        public static string GetCurrentUserRoles()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role).ToString();
        }
    }
}