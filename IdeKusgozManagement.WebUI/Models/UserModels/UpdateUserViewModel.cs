﻿using System.ComponentModel.DataAnnotations;

namespace IdeKusgozManagement.WebUI.Models.UserModels
{
    public class UpdateUserViewModel
    {
        [Required]
        public string TCNo { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        public string? Password { get; set; }

        public bool? IsActive { get; set; }
        public string? RoleName { get; set; }

        [Required]
        public bool IsExpatriate { get; set; }

        public List<string>? SuperiorIds { get; set; } = new();
    }
}