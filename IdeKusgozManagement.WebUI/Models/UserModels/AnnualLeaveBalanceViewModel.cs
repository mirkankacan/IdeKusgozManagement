namespace IdeKusgozManagement.WebUI.Models.UserModels
{
    public class AnnualLeaveBalanceViewModel
    {
        public decimal TotalEntitlement { get; set; }
        public decimal TotalUsedDays { get; set; }
        public decimal RemainingDays { get; set; }
        public decimal CurrentYearEntitlement { get; set; }
        public decimal CurrentYearUsed { get; set; }
        public int ServiceYears { get; set; }
        public string EmployeeName { get; set; }
        public DateTime? HireDate { get; set; }
    }
}