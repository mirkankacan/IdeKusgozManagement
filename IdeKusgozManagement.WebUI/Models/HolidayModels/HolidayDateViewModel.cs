namespace IdeKusgozManagement.WebUI.Models.HolidayModels
{
    public class HolidayDateViewModel
    {
        public string Iso { get; set; }
        public HolidayDateTimeViewModel DateTime { get; set; }
        public HolidayTimezoneViewModel Timezone { get; set; }
    }
}