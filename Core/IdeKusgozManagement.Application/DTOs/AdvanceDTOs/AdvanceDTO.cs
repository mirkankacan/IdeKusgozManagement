using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Application.DTOs.AdvanceDTOs
{
    public class AdvanceDTO
    {
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public string? Description { get; set; }
        public AdvanceStatus Status { get; set; }

        public string StatusText => Status switch
        {
            AdvanceStatus.Pending => "Beklemede",
            AdvanceStatus.ApprovedByUnitManager => "Birim Yöneticisi Onayladı",
            AdvanceStatus.ApprovedByChief => "Şef Onayladı",
            AdvanceStatus.RejectedByUnitManager => "Birim Yöneticisi Reddetti",
            AdvanceStatus.RejectedByChief => "Şef Reddetti",
            _ => "Bilinmiyor"
        };

        public DateTime? UnitManagerProcessedDate { get; set; }
        public string? ProcessedByUnitManagerId { get; set; }
        public string? UnitManagerFullName { get; set; }
        public DateTime? ChiefProcessedDate { get; set; }
        public string? ProcessedByChiefId { get; set; }
        public string? ChiefByFullName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByFullName { get; set; }
        public string? UpdatedBy { get; set; }
        public string? UpdatedByFullName { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}