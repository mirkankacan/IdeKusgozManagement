using System.Text.Json.Serialization;

namespace IdeKusgozManagement.Application.DTOs.HolidayDTOs
{
    public class HolidayDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public HolidayCountryDTO Country { get; set; }
        public HolidayDateDTO Date { get; set; }
        public List<string> Type { get; set; }
        [JsonPropertyName("primary_type")]
        public string PrimaryType { get; set; }
        [JsonPropertyName("canonical_url")]
        public string CanonicalUrl { get; set; }
        public string Urlid { get; set; }
        public string Locations { get; set; }
        public string States { get; set; }
    }
}
