namespace IdeKusgozManagement.Application.DTOs.OptionDTOs
{
    public class EmailOptionsDTO
    {
        public string FromEmail { get; set; }
        public string[] ToEmails { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string Password { get; set; }
        public string DeliveryMethod { get; set; }
    }
}