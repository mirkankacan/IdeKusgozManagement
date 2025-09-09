using System.Security.Claims;
using IdeKusgozManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CurrentUserService> _logger;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor, ILogger<CurrentUserService> logger)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger;
        }

        public string? GetCurrentUserId()
        {
            try
            {
                return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCurrentUserId işleminde hata oluştu");

                throw;
            }
        }

        public string? GetCurrentUserName()
        {
            try
            {
                return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCurrentUserName işleminde hata oluştu");

                throw;
            }
        }

        public string? GetCurrentUserRole()
        {
            try
            {
                return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCurrentUserRole işleminde hata oluştu");

                throw;
            }
        }
    }
}