namespace IdeKusgozManagement.Application.DTOs.AIDTOs
{
    public class AIDateResponse
    {
        public string SelectedDate { get; set; } = "";
        public double Confidence { get; set; }
        public string Reasoning { get; set; } = "";
    }
}