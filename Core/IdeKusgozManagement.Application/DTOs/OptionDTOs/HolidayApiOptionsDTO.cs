namespace IdeKusgozManagement.Application.DTOs.OptionDTOs
{
    public class HolidayApiOptionsDTO
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
        public string Country { get; set; }
        public int CacheExpirationDays { get; set; }
    }
}