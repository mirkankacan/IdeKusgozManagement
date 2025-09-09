namespace IdeKusgozManagement.Application.DTOs.HolidayDTOs
{
    public class HolidayTimezoneDTO
    {
        public string Offset { get; set; }
        public string Zoneabb { get; set; }
        public int Zoneoffset { get; set; }
        public int Zonedst { get; set; }
        public int Zonetotaloffset { get; set; }
    }
}
