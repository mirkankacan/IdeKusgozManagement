namespace IdeKusgozManagement.WebUI.Models.HolidayModels
{
    public class HolidayViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public HolidayCountryViewModel Country { get; set; }
        public HolidayDateViewModel Date { get; set; }
        public List<string> Type { get; set; }

        public string primary_type { get; set; }
        public string canonical_url { get; set; }

        public string Urlid { get; set; }
        public string Locations { get; set; }
        public string States { get; set; }
    }
}