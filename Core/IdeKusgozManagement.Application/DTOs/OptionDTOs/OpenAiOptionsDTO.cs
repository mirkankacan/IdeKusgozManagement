namespace IdeKusgozManagement.Application.DTOs.OptionDTOs
{
    public class OpenAiOptionsDTO
    {
        public string ApiKey { get; set; } = default!;
        public string OrganizationId { get; set; } = default!;
        public string ProjectId { get; set; } = default!;
        public string Model { get; set; } = default!;
        public string Name { get; set; } = default!;
    }
}