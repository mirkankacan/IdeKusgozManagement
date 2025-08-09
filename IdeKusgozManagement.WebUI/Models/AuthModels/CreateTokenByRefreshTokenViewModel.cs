using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebUI.Models.AuthModels
{
    public class CreateTokenByRefreshTokenViewModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}