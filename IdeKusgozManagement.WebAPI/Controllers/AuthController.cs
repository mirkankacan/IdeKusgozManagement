using IdeKusgozManagement.Application.DTOs.AuthDTOs;
using IdeKusgozManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdeKusgozManagement.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        /// <summary>
        /// Kullanıcı girişi yapar ve JWT token döner
        /// </summary>
        /// <param name="loginDTO">Giriş bilgileri</param>
        /// <returns>JWT Token bilgileri</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await authService.LoginAsync(loginDTO);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Kullanıcı çıkışı yapar
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var result = await authService.LogoutAsync();
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Refresh token ile yeni access token alır
        /// </summary>
        /// <param name="refreshTokenDTO">Refresh token bilgileri</param>
        /// <returns>Yeni JWT Token bilgileri</returns>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] CreateTokenByRefreshTokenDTO refreshTokenDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await authService.RefreshTokenAsync(refreshTokenDTO);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}