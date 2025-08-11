using System.Security.Claims;
using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.AuthDTOs;
using IdeKusgozManagement.Application.Interfaces;
using IdeKusgozManagement.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtProvider _jwtProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtProvider jwtProvider,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtProvider = jwtProvider;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<ApiResponse<TokenDTO>> LoginAsync(LoginDTO loginDTO)
        {
            try
            {
                // Kullanıcıyı bul
                var user = await _userManager.FindByNameAsync(loginDTO.UserName);
                if (user == null)
                {
                    _logger.LogWarning("Giriş denemesi başarısız. Kullanıcı bulunamadı: {UserName}", loginDTO.UserName);
                    return ApiResponse<TokenDTO>.Error("Kullanıcı adı veya şifre hatalı");
                }

                // Kullanıcı aktif mi kontrol et
                if (!user.IsActive)
                {
                    _logger.LogWarning("Giriş denemesi başarısız. Kullanıcı pasif durumda: {UserName}", loginDTO.UserName);
                    return ApiResponse<TokenDTO>.Error("Hesabınız pasif durumda");
                }

                // Şifre kontrolü
                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("Giriş denemesi başarısız. Geçersiz şifre: {UserName}", loginDTO.UserName);
                    return ApiResponse<TokenDTO>.Error("Kullanıcı adı veya şifre hatalı");
                }

                // Token oluştur
                var token = await _jwtProvider.CreateTokenAsync(user);

                _logger.LogInformation("Kullanıcı başarıyla giriş yaptı: {UserName}", loginDTO.UserName);

                return ApiResponse<TokenDTO>.Success(token, "Giriş başarılı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LoginAsync işleminde hata oluştu. UserName: {UserName}", loginDTO.UserName);
                return ApiResponse<TokenDTO>.Error("Giriş işlemi sırasında bir hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> LogoutAsync()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!string.IsNullOrEmpty(userId))
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        // Refresh token'ı temizle
                        user.RefreshToken = null;
                        user.RefreshTokenExpires = null;
                        await _userManager.UpdateAsync(user);

                        _logger.LogInformation("Kullanıcı başarıyla çıkış yaptı: {UserId}", userId);
                    }
                }

                await _signInManager.SignOutAsync();
                return ApiResponse<bool>.Success(true, "Çıkış başarılı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LogoutAsync işleminde hata oluştu");
                return ApiResponse<bool>.Error("Çıkış işlemi sırasında bir hata oluştu");
            }
        }

        public async Task<ApiResponse<TokenDTO>> RefreshTokenAsync(CreateTokenByRefreshTokenDTO createTokenByRefreshTokenDTO)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(createTokenByRefreshTokenDTO.UserId);
                if (user == null)
                {
                    _logger.LogWarning("Refresh token denemesi başarısız. Kullanıcı bulunamadı. User Id: {UserId}", createTokenByRefreshTokenDTO.UserId);
                    return ApiResponse<TokenDTO>.Error("Kullanıcı bulunamadı");
                }

                // Kullanıcı aktif mi kontrol et
                if (!user.IsActive)
                {
                    _logger.LogWarning("Refresh token denemesi başarısız. Kullanıcı pasif durumda. User Id: {UserId}", createTokenByRefreshTokenDTO.UserId);
                    return ApiResponse<TokenDTO>.Error("Hesabınız pasif durumda");
                }

                // Refresh token kontrolü
                if (user.RefreshToken != createTokenByRefreshTokenDTO.RefreshToken)
                {
                    _logger.LogWarning("Refresh token denemesi başarısız. Geçersiz refresh token User Id: {UserId} | Database Refresh Token: {DbRefreshToken} | From UI Refresh Token: {UiRefreshToken}", createTokenByRefreshTokenDTO.UserId, user.RefreshToken,createTokenByRefreshTokenDTO.RefreshToken);
                    return ApiResponse<TokenDTO>.Error("Geçersiz refresh token");
                }

                // Refresh token süresi dolmuş mu kontrol et
                if (user.RefreshTokenExpires == null || user.RefreshTokenExpires <= DateTime.Now)
                {
                    _logger.LogWarning("Refresh token denemesi başarısız. Refresh token süresi dolmuş. User Id: {UserId}", createTokenByRefreshTokenDTO.UserId);
                    return ApiResponse<TokenDTO>.Error("Refresh token süresi dolmuş");
                }

                // Yeni token oluştur
                var newToken = await _jwtProvider.CreateTokenAsync(user);

                _logger.LogInformation("Token başarıyla yenilendi. User Id: {UserId}", createTokenByRefreshTokenDTO.UserId);

                return ApiResponse<TokenDTO>.Success(newToken, "Token başarıyla yenilendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RefreshTokenAsync işleminde hata oluştu. UserId: {UserId}", createTokenByRefreshTokenDTO.UserId);
                return ApiResponse<TokenDTO>.Error("Token yenileme işlemi sırasında bir hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> IsAuthenticatedAsync()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext?.User?.Identity?.IsAuthenticated != true)
                {
                    return ApiResponse<bool>.Success(false, "Kullanıcı oturumu açık değil");
                }

                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return ApiResponse<bool>.Success(false, "Kullanıcı ID'si bulunamadı");
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("Kimlik doğrulama kontrolünde kullanıcı bulunamadı: {UserId}", userId);
                    return ApiResponse<bool>.Success(false, "Kullanıcı bulunamadı");
                }

                if (!user.IsActive)
                {
                    _logger.LogWarning("Kimlik doğrulama kontrolünde kullanıcı pasif: {UserId}", userId);
                    return ApiResponse<bool>.Success(false, "Kullanıcı hesabı pasif");
                }

                return ApiResponse<bool>.Success(true, "Kullanıcı kimliği doğrulandı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IsAuthenticatedAsync işleminde hata oluştu");
                return ApiResponse<bool>.Error("Kimlik doğrulama kontrolü sırasında bir hata oluştu");
            }
        }

        private string? GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}