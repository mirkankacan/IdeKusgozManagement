namespace IdeKusgozManagement.Application.DTOs.AIDTOs
{
    public class AIDateRequest
    {
        public byte[] DocumentBytes { get; set; }
        public string ContentType { get; set; }
        public string DocumentTypeName { get; set; }
    }
}