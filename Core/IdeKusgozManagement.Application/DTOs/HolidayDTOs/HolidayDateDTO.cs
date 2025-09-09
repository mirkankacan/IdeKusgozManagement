namespace IdeKusgozManagement.Application.DTOs.HolidayDTOs
{
    public class HolidayDateDTO
    {
        public string Iso { get; set; }
        public HolidayDateTimeDTO Datetime { get; set; }
        public HolidayTimezoneDTO Timezone { get; set; }
    }
}
