using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IdeKusgozManagement.Infrastructure.Authorization
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

            // If authorization already failed (e.g., from RoleFilter), check department duties as alternative
            // This provides OR logic: if role check fails but duty check passes, allow access
            if (context.Result != null)
            {
                // Previous filter (like RoleFilter) failed, check if user has required duty
                var hasRequiredDuty = false;
                if (_departmentDuties.Length > 0)
                {
                    var departmentDutyName = user.FindFirstValue("DepartmentDutyName");
                    if (!string.IsNullOrWhiteSpace(departmentDutyName))
                    {
                        hasRequiredDuty = _departmentDuties.Contains(departmentDutyName);
                    }
                }

                // If user has required duty, clear the failure and allow access
                if (hasRequiredDuty)
                {
                    context.Result = null;
                    return Task.CompletedTask;
                }
                // If user doesn't have required duty, keep the previous failure (Forbid)
                return Task.CompletedTask;
            }

            // If no previous authorization failure, check department duties anyway
            // This ensures the attribute works correctly regardless of filter order
            var hasDuty = false;
            if (_departmentDuties.Length > 0)
            {
                var departmentDutyName = user.FindFirstValue("DepartmentDutyName");
                if (!string.IsNullOrWhiteSpace(departmentDutyName))
                {
                    hasDuty = _departmentDuties.Contains(departmentDutyName);
                }
            }

            // If user has required duty, allow access
            if (hasDuty)
            {
                return Task.CompletedTask;
            }

            // User doesn't have required duty
            // Don't set ForbidResult here - let other filters (like RoleFilter) handle it
            // If all filters fail, the combined result will be Forbid
            return Task.CompletedTask;
        }
    }
}

