namespace IdeKusgozManagement.WebUI.Models.MachineWorkRecordModels
{
    public class MachineWorkRecordViewModel
    {
        public string Id { get; set; }
        public string DailyStatus { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string? ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public string? EquipmentId { get; set; }
        public string? EquipmentName { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Description { get; set; }
        public bool HasInternalTransport { get; set; }
        public int Status { get; set; }
        public string? RejectReason { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string CreatedByFullName { get; set; }
        public string? UpdatedByFullName { get; set; }

        public string StatusText { get; set; }
    }
}