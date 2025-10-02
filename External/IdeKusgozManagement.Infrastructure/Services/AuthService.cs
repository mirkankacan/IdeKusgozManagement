using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.DTOs.AuthDTOs;
using IdeKusgozManagement.Application.Interfaces.Providers;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IJwtProvider jwtProvider, IIdentityService identityService, ILogger<AuthService> logger) : IAuthService
    {
        public async Task<ApiResponse<TokenDTO>> LoginAsync(LoginDTO loginDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                // Kullanıcıyı bul
                var user = await userManager.FindByNameAsync(loginDTO.TCNo);
                if (user == null)
                {
                    logger.LogWarning("Giriş denemesi başarısız. Kullanıcı bulunamadı. TCNo: {TCNo}", loginDTO.TCNo);
                    return ApiResponse<TokenDTO>.Error("TC No veya şifre hatalı");
                }

                // Kullanıcı aktif mi kontrol et
                if (!user.IsActive)
                {
                    logger.LogWarning("Giriş denemesi başarısız. Kullanıcı pasif durumda. TCNo: {TCNo}", loginDTO.TCNo);
                    return ApiResponse<TokenDTO>.Error("Hesabınız pasif durumda");
                }

                // Şifre kontrolü
                var result = await signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);
                if (!result.Succeeded)
                {
                    logger.LogWarning("Giriş denemesi başarısız. Geçersiz şifre. TCNo: {TCNo}", loginDTO.TCNo);
                    return ApiResponse<TokenDTO>.Error("Şifre hatalı");
                }

                // Token oluştur
                var token = await jwtProvider.CreateTokenAsync(user);

                logger.LogInformation("Kullanıcı başarıyla giriş yaptı. TCNo: {TCNo}", loginDTO.TCNo);

                return ApiResponse<TokenDTO>.Success(token, "Giriş başarılı");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "LoginAsync işleminde hata oluştu. TCNo: {TCNo}", loginDTO.TCNo);
                return ApiResponse<TokenDTO>.Error("Giriş işlemi sırasında bir hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> LogoutAsync(CancellationToken cancellationToken = default)
        {
            var userId = identityService.GetUserId();

            try
            {
                if (!string.IsNullOrEmpty(userId))
                {
                    var user = await userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        // Refresh token'ı temizle
                        user.RefreshToken = null;
                        user.RefreshTokenExpires = null;
                        await userManager.UpdateAsync(user);

                        logger.LogInformation("Kullanıcı başarıyla çıkış yaptı. UserId: {UserId}", userId);
                    }
                }

                await signInManager.SignOutAsync();
                return ApiResponse<bool>.Success(true, "Çıkış başarılı");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "LogoutAsync işleminde hata oluştu. UserId: {UserId}", userId);
                return ApiResponse<bool>.Error("Çıkış işlemi sırasında bir hata oluştu");
            }
        }

        public async Task<ApiResponse<TokenDTO>> RefreshTokenAsync(CreateTokenByRefreshTokenDTO createTokenByRefreshTokenDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await userManager.FindByIdAsync(createTokenByRefreshTokenDTO.UserId);
                if (user == null)
                {
                    logger.LogWarning("Refresh token denemesi başarısız. Kullanıcı bulunamadı. UserId: {UserId}", createTokenByRefreshTokenDTO.UserId);
                    return ApiResponse<TokenDTO>.Error("Kullanıcı bulunamadı");
                }

                // Kullanıcı aktif mi kontrol et
                if (!user.IsActive)
                {
                    logger.LogWarning("Refresh token denemesi başarısız. Kullanıcı pasif durumda. UserId: {UserId}", createTokenByRefreshTokenDTO.UserId);
                    return ApiResponse<TokenDTO>.Error("Hesabınız pasif durumda");
                }

                // Refresh token kontrolü
                if (user.RefreshToken != createTokenByRefreshTokenDTO.RefreshToken)
                {
                    logger.LogWarning("Refresh token denemesi başarısız. Geçersiz refresh token. UserId: {UserId} | DatabaseRefreshToken: {DbRefreshToken} | FromUIRefreshToken: {UiRefreshToken}", createTokenByRefreshTokenDTO.UserId, user.RefreshToken, createTokenByRefreshTokenDTO.RefreshToken);
                    return ApiResponse<TokenDTO>.Error("Geçersiz refresh token");
                }

                // Refresh token süresi dolmuş mu kontrol et
                if (user.RefreshTokenExpires == null || user.RefreshTokenExpires <= DateTime.Now)
                {
                    logger.LogWarning("Refresh token denemesi başarısız. Refresh token süresi dolmuş. UserId: {UserId}", createTokenByRefreshTokenDTO.UserId);
                    return ApiResponse<TokenDTO>.Error("Refresh token süresi dolmuş");
                }

                // Yeni token oluştur
                var newToken = await jwtProvider.CreateTokenAsync(user);

                logger.LogInformation("Token başarıyla yenilendi. UserId: {UserId}", createTokenByRefreshTokenDTO.UserId);

                return ApiResponse<TokenDTO>.Success(newToken, "Token başarıyla yenilendi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "RefreshTokenAsync işleminde hata oluştu. UserId: {UserId}", createTokenByRefreshTokenDTO.UserId);
                return ApiResponse<TokenDTO>.Error("Token yenileme işlemi sırasında bir hata oluştu");
            }
        }
    }
}