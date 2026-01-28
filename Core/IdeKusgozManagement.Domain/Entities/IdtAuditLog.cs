namespace IdeKusgozManagement.Domain.Entities
{
    public class IdtAuditLog
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public string Operation { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
    }
}