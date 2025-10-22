namespace IdeKusgozManagement.Application.DTOs.HolidayDTOs
{
    public class HolidayDateDTO
    {
        public string Iso { get; set; }
        public HolidayDateTimeDTO DateTime { get; set; }
        public HolidayTimezoneDTO Timezone { get; set; }
    }
}
