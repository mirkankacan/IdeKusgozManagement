namespace IdeKusgozManagement.Application.DTOs.MessageDTOs
{
    public class CreateMessageDTO
    {
        public string Content { get; set; }
        public string[]? TargetRoles { get; set; }
        public string[]? TargetUsers { get; set; }
    }
}