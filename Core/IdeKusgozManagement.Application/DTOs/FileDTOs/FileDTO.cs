namespace IdeKusgozManagement.Application.DTOs.FileDTOs
{
    public class FileDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string OriginalName { get; set; }
        public Stream? FileStream { get; set; }
        public string? ContentType { get; set; }
    }
}