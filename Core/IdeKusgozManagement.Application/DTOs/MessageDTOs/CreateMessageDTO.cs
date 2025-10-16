namespace IdeKusgozManagement.Application.DTOs.MessageDTOs
{
    public class CreateMessageDTO
    {
        public string Content { get; set; }
        public List<string>? TargetRoles { get; set; }
        public List<string>? TargetUsers { get; set; }
    }
}