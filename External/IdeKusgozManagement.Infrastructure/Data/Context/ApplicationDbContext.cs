using IdeKusgozManagement.Domain.Entities;
using IdeKusgozManagement.Domain.Entities.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IdeKusgozManagement.Infrastructure.Data.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        private readonly IHttpContextAccessor? _accessor;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor? accessor = null) : base(options)
        {
            _accessor = accessor;
        }

        public DbSet<IdtWorkRecord> IdtWorkRecords => Set<IdtWorkRecord>();
        public DbSet<IdtLeaveRequest> IdtLeaveRequests => Set<IdtLeaveRequest>();
        public DbSet<IdtWorkRecordExpense> IdtWorkRecordExpenses => Set<IdtWorkRecordExpense>();
        public DbSet<IdtUserHierarchy> IdtUserHierarchies => Set<IdtUserHierarchy>();
        public DbSet<IdtExpense> IdtExpenses => Set<IdtExpense>();
        public DbSet<IdtEquipment> IdtEquipments => Set<IdtEquipment>();
        public DbSet<IdtMessage> IdtMessages => Set<IdtMessage>();
        public DbSet<IdtNotification> IdtNotifications => Set<IdtNotification>();
        public DbSet<IdtNotificationRead> IdtNotificationReads => Set<IdtNotificationRead>();
        public DbSet<IdtFile> IdtFiles => Set<IdtFile>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(InfrastructureAssembly).Assembly);

            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<BaseEntity>();
            string? userId = _accessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = DateTime.Now;
                        entry.Entity.CreatedBy = userId ?? "Unknown";
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedDate = DateTime.Now;
                        entry.Entity.UpdatedBy = userId ?? "Unknown";
                        entry.Property(e => e.CreatedDate).IsModified = false;
                        entry.Property(e => e.CreatedBy).IsModified = false;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}