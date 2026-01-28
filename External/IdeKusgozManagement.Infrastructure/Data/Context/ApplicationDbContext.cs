using IdeKusgozManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdeKusgozManagement.Infrastructure.Data.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<IdtAdvance> IdtAdvances => Set<IdtAdvance>();
        public DbSet<IdtAdvancePart> IdtAdvanceParts => Set<IdtAdvancePart>();
        public DbSet<IdtEquipment> IdtEquipments => Set<IdtEquipment>();
        public DbSet<IdtExpense> IdtExpenses => Set<IdtExpense>();
        public DbSet<IdtFile> IdtFiles => Set<IdtFile>();
        public DbSet<IdtLeaveRequest> IdtLeaveRequests => Set<IdtLeaveRequest>();
        public DbSet<IdtMessage> IdtMessages => Set<IdtMessage>();
        public DbSet<IdtNotification> IdtNotifications => Set<IdtNotification>();
        public DbSet<IdtNotificationRead> IdtNotificationReads => Set<IdtNotificationRead>();
        public DbSet<IdtProject> IdtProjects => Set<IdtProject>();
        public DbSet<IdtUserHierarchy> IdtUserHierarchies => Set<IdtUserHierarchy>();
        public DbSet<IdtWorkRecord> IdtWorkRecords => Set<IdtWorkRecord>();
        public DbSet<IdtWorkRecordExpense> IdtWorkRecordExpenses => Set<IdtWorkRecordExpense>();
        public DbSet<IdtTrafficTicket> IdtTrafficTickets => Set<IdtTrafficTicket>();
        public DbSet<IdtAnnualLeaveEntitlement> IdtAnnualLeaveEntitlements => Set<IdtAnnualLeaveEntitlement>();
        public DbSet<IdtDepartment> IdtDepartments => Set<IdtDepartment>();
        public DbSet<IdtDepartmentDuty> IdtDepartmentDuties => Set<IdtDepartmentDuty>();
        public DbSet<IdtDocumentType> IdtDocumentTypes => Set<IdtDocumentType>();
        public DbSet<IdtCompany> IdtCompanies => Set<IdtCompany>();
        public DbSet<IdtDepartmentDocumentRequirment> IdtDepartmentDocumentRequirments => Set<IdtDepartmentDocumentRequirment>();
        public DbSet<IdtUserBalance> IdtUserBalances => Set<IdtUserBalance>();
        public DbSet<IdtMachineWorkRecord> IdtMachineWorkRecords => Set<IdtMachineWorkRecord>();
        public DbSet<IdtCompanyPayment> IdtCompanyPayments => Set<IdtCompanyPayment>();
        public DbSet<IdtAuditLog> IdtAuditLogs => Set<IdtAuditLog>();
        public DbSet<IdtParameter> IdtParameters => Set<IdtParameter>();
        public DbSet<IdtDevice> IdtDevices => Set<IdtDevice>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(InfrastructureAssembly).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}