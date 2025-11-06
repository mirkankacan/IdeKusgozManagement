using IdeKusgozManagement.Domain.Entities.Base;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtFile : BaseEntity
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string OriginalName { get; set; }
        public string DocumentTypeId { get; set; }
        public string? TargetUserId { get; set; }
        public string? DepartmentId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public virtual IdtDocumentType DocumentType { get; set; }
        public virtual IdtDepartment? Department { get; set; }
        public virtual ApplicationUser? TargetUser { get; set; }
        public virtual ICollection<IdtWorkRecordExpense> WorkRecordExpenses { get; set; } = new List<IdtWorkRecordExpense>();
        public virtual ICollection<IdtLeaveRequest> LeaveRequests { get; set; } = new List<IdtLeaveRequest>();
        public virtual ICollection<IdtTrafficTicket> TrafficTickets { get; set; } = new List<IdtTrafficTicket>();
    }
}