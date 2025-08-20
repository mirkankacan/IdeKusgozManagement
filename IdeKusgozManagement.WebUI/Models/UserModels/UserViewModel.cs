namespace IdeKusgozManagement.WebUI.Models.UserModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string FullName => $"{Name} {Surname}";
        public bool IsActive { get; set; }
        public string RoleName { get; set; }
        public List<string> SuperiorIds { get; set; }
    }
}