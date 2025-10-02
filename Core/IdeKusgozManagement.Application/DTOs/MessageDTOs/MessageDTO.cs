namespace IdeKusgozManagement.Application.DTOs.MessageDTOs
{
    public class MessageDTO
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByFullName { get; set; }
    }
}