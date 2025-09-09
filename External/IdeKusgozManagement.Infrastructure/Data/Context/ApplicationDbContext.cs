using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Domain.Entities;
using IdeKusgozManagement.Domain.Entities.Base;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdeKusgozManagement.Infrastructure.Data.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        private readonly ILogger<ApplicationDbContext> _logger;
        private readonly ICurrentUserService _currentUserService;

        public ApplicationDbContext(DbContextOptions options, ILogger<ApplicationDbContext> logger, ICurrentUserService currentUserService)
            : base(options)
        {
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public DbSet<IdtWorkRecord> IdtWorkRecords => Set<IdtWorkRecord>();
        public DbSet<IdtLeaveRequest> IdtLeaveRequests => Set<IdtLeaveRequest>();
        public DbSet<IdtWorkRecordExpense> IdtWorkRecordExpenses => Set<IdtWorkRecordExpense>();
        public DbSet<IdtUserHierarchy> IdtUserHierarchies => Set<IdtUserHierarchy>();
        public DbSet<IdtExpense> IdtExpenses => Set<IdtExpense>();
        public DbSet<IdtEquipment> IdtEquipments => Set<IdtEquipment>();
        public DbSet<IdtMessage> IdtMessages => Set<IdtMessage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdtWorkRecordExpense>()
               .HasOne(wre => wre.WorkRecord)
               .WithMany(wr => wr.WorkRecordExpenses)
               .HasForeignKey(wre => wre.WorkRecordId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<IdtWorkRecord>()
              .HasOne(wr => wr.Equipment)
              .WithMany(e => e.WorkRecords)
              .HasForeignKey(wr => wr.EquipmentId)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<IdtWorkRecordExpense>()
             .HasOne(wre => wre.Expense)
             .WithMany(e => e.WorkRecordExpenses)
             .HasForeignKey(wre => wre.ExpenseId)
             .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<IdtLeaveRequest>()
                .HasOne(lr => lr.CreatedByUser)
                .WithMany(u => u.CreatedLeaveRequests)
                .HasForeignKey(lr => lr.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<IdtLeaveRequest>()
                .HasOne(lr => lr.UpdatedByUser)
                .WithMany(u => u.UpdatedLeaveRequests)
                .HasForeignKey(lr => lr.UpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<IdtMessage>()
               .HasOne(m => m.CreatedByUser)
               .WithMany()
               .HasForeignKey(m => m.CreatedBy)
               .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<BaseEntity>();
            var currentUserId = _currentUserService.GetCurrentUserId();
            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = DateTime.Now;
                        entry.Entity.CreatedBy = currentUserId ?? "Unknown";
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedDate = DateTime.Now;
                        entry.Entity.UpdatedBy = currentUserId ?? "Unknown";
                        entry.Property(e => e.CreatedDate).IsModified = false;
                        entry.Property(e => e.CreatedBy).IsModified = false;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}