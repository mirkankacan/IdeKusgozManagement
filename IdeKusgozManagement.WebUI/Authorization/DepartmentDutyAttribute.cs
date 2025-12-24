using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IdeKusgozManagement.WebUI.Authorization
{
    /// <summary>
    /// Custom authorization attribute that checks department duties.
    /// This attribute works with [Authorize(Roles = "...")] to provide OR logic:
    /// User must have one of the specified roles OR one of the specified department duties.
    /// If the user already passed role authorization, this attribute allows access.
    /// Otherwise, it checks department duties.
    /// </summary>
    public sealed class DepartmentDutyAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string[] _departmentDuties;

        /// <summary>
        /// Initializes a new instance with department duties.
        /// </summary>
        /// <param name="departmentDuties">Array of allowed department duties</param>
        public DepartmentDutyAttribute(params string[] departmentDuties)
        {
            _departmentDuties = departmentDuties ?? Array.Empty<string>();
        }

        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            // Check if user is authenticated
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new UnauthorizedResult();
                return Task.CompletedTask;
            }

            // Check department duties
            var hasRequiredDuty = false;
            if (_departmentDuties.Length > 0)
            {
                var departmentDutyName = user.FindFirstValue("DepartmentDutyName");
                if (!string.IsNullOrWhiteSpace(departmentDutyName))
                {
                    hasRequiredDuty = _departmentDuties.Contains(departmentDutyName);
                }
            }

            // If user has required duty, allow access
            // This provides OR logic: if role check fails but duty check passes, allow access
            if (hasRequiredDuty)
            {
                // Clear any previous authorization failure (from [Authorize(Roles = "...")])
                // to allow access based on department duty
                context.Result = null;
                return Task.CompletedTask;
            }

            // User doesn't have required duty
            // If role authorization also failed, the combined result will be Forbid
            // If role authorization passed, this won't matter
            return Task.CompletedTask;
        }
    }
}

