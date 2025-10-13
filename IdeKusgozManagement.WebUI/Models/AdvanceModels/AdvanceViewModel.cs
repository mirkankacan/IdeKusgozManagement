﻿namespace IdeKusgozManagement.WebUI.Models.AdvanceModels
{
    public class AdvanceViewModel
    {
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public string? Description { get; set; }
        public int Status { get; set; }

        public string StatusText { get; set; }
        public DateTime? UnitManagerProcessedDate { get; set; }
        public string? ProcessedByUnitManagerId { get; set; }
        public string? UnitManagerFullName { get; set; }
        public DateTime? ChiefProcessedDate { get; set; }
        public string? ProcessedByChiefId { get; set; }
        public string? ChiefByFullName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByFullName { get; set; }
    }
}