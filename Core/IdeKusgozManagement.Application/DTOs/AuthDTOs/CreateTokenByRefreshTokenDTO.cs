using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeKusgozManagement.Application.DTOs.AuthDTOs
{
    public class CreateTokenByRefreshTokenDTO
    {
        public string UserId { get; set; }
        public string RefreshToken { get; set; }
    }
}