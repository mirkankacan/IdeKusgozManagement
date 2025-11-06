namespace IdeKusgozManagement.Application.DTOs.OptionDTOs
{
    public class GeminiAiOptionsDTO
    {
        public string ApiUrl { get; set; } = default!;
        public string ApiKey { get; set; } = default!;
        public string Name { get; set; }
        public string ProjectNumber { get; set; }
    }
}