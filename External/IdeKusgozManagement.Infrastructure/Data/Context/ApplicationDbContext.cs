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
            modelBuilder.Entity<IdtWorkRecordExpense>()
               .HasOne(x => x.WorkRecord)
               .WithMany(x => x.WorkRecordExpenses)
               .HasForeignKey(x => x.WorkRecordId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<IdtWorkRecord>()
              .HasOne(x => x.Equipment)
              .WithMany(x => x.WorkRecords)
              .HasForeignKey(x => x.EquipmentId)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<IdtWorkRecordExpense>()
             .HasOne(x => x.Expense)
             .WithMany(x => x.WorkRecordExpenses)
             .HasForeignKey(x => x.ExpenseId)
             .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<IdtWorkRecordExpense>()
               .HasOne(x => x.File)
               .WithMany(x => x.WorkRecordExpenses)
               .HasForeignKey(x => x.FileId)
               .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<IdtLeaveRequest>()
                .HasOne(x => x.File)
                .WithMany(x => x.LeaveRequests)
                .HasForeignKey(x => x.FileId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<IdtLeaveRequest>()
                .HasOne(x => x.CreatedByUser)
                .WithMany()
                .HasForeignKey(x => x.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<IdtLeaveRequest>()
                .HasOne(x => x.UpdatedByUser)
                .WithMany()
                .HasForeignKey(x => x.UpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<IdtWorkRecord>()
             .HasOne(x => x.CreatedByUser)
             .WithMany()
             .HasForeignKey(x => x.CreatedBy)
             .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<IdtWorkRecord>()
                .HasOne(x => x.UpdatedByUser)
                .WithMany()
                .HasForeignKey(x => x.UpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<IdtMessage>()
               .HasOne(x => x.CreatedByUser)
               .WithMany()
               .HasForeignKey(x => x.CreatedBy)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<IdtNotification>()
               .HasOne(x => x.CreatedByUser)
               .WithMany()
               .HasForeignKey(x => x.CreatedBy)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<IdtNotificationRead>()
               .HasOne(x => x.Notification)
               .WithMany(x => x.NotificationReads)
               .HasForeignKey(x => x.NotificationId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<IdtNotificationRead>()
               .HasOne(x => x.CreatedByUser)
               .WithMany()
               .HasForeignKey(x => x.CreatedBy)
               .OnDelete(DeleteBehavior.Restrict);

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