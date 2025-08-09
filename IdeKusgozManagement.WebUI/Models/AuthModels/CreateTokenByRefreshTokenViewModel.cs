namespace IdeKusgozManagement.WebUI.Models.AuthModels
{
    public class CreateTokenByRefreshTokenViewModel
    {
        public string UserId { get; set; }
        public string RefreshToken { get; set; }
    }
}