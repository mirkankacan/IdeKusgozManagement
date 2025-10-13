using IdeKusgozManagement.Domain.Entities.Base;
using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtFile : BaseEntity
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string OriginalName { get; set; }
        public string? TargetUserId { get; set; }
        public FileType Type { get; set; }
        public virtual ICollection<IdtWorkRecordExpense> WorkRecordExpenses { get; set; } = new List<IdtWorkRecordExpense>();
        public virtual ICollection<IdtLeaveRequest> LeaveRequests { get; set; } = new List<IdtLeaveRequest>();
    }
}