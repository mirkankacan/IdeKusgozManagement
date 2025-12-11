namespace IdeKusgozManagement.Application.DTOs.MachineWorkRecordDTOs
{
    public class CreateOrModifyMachineWorkRecordDTO
    {
        public DateTime Date { get; set; }
        public string DailyStatus { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string? ProjectId { get; set; }
        public string? EquipmentId { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public bool HasInternalTransport { get; set; }
        public string? RejectReason { get; set; }
        public string? Description { get; set; }
    }
}