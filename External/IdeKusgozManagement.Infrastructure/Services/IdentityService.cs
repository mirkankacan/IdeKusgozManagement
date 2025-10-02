using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Application.Interfaces.UnitOfWork;
using IdeKusgozManagement.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class IdentityService(IHttpContextAccessor accessor, ILogger<IdentityService> logger, IUnitOfWork unitOfWork) : IIdentityService
    {
        public string GetUserFullName()
        {
            if (!accessor.HttpContext!.User.Identity!.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Kullanıcı kimliği doğrulanmadı");
            }
            return accessor.HttpContext!.User!.Claims.First(c => c.Type == "FullName")!.Value;
        }

        public string GetUserId()
        {
            if (!accessor.HttpContext!.User.Identity!.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Kullanıcı kimliği doğrulanmadı");
            }
            return accessor.HttpContext!.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier)!.Value!;
        }

        public string GetUserRole()
        {
            if (!accessor.HttpContext!.User.Identity!.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Kullanıcı kimliği doğrulanmadı");
            }
            return accessor.HttpContext!.User!.Claims.First(c => c.Type == ClaimTypes.Role)!.Value;
        }

        public async Task<string[]?> GetUserSuperiorsAsync(CancellationToken cancellationToken = default)
        {
            if (!accessor.HttpContext!.User.Identity!.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Kullanıcı kimliği doğrulanmadı");
            }
            var users = await unitOfWork.GetRepository<IdtUserHierarchy>().WhereAsNoTracking(x => x.SubordinateId == GetUserId()).Select(x => x.SuperiorId).ToArrayAsync(cancellationToken);
            return users;
        }

        public async Task<string[]?> GetUserSubordinatesAsync(CancellationToken cancellationToken = default)
        {
            if (!accessor.HttpContext!.User.Identity!.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Kullanıcı kimliği doğrulanmadı");
            }
            var users = await unitOfWork.GetRepository<IdtUserHierarchy>().WhereAsNoTracking(x => x.SuperiorId == GetUserId()).Select(x => x.SubordinateId).ToArrayAsync(cancellationToken);
            return users;
        }

        public string GetUserTCNo()
        {
            if (!accessor.HttpContext!.User.Identity!.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Kullanıcı kimliği doğrulanmadı");
            }
            return accessor.HttpContext!.User.Claims.First(c => c.Type == "TCNo")!.Value!;
        }
    }
}