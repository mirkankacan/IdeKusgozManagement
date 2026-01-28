using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.AuthDTOs;
using IdeKusgozManagement.Application.Interfaces.Providers;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Net;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IJwtProvider jwtProvider, IIdentityService identityService, ILogger<AuthService> logger, IEmailService emailService) : IAuthService
    {
        public async Task<ServiceResult<TokenDTO>> LoginAsync(LoginDTO loginDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                // Kullanıcıyı bul
                var user = await userManager.FindByNameAsync(loginDTO.TCNo);
                if (user == null)
                {
                    logger.LogWarning("Giriş denemesi başarısız. Kullanıcı bulunamadı. TCNo: {TCNo}", loginDTO.TCNo);
                    return ServiceResult<TokenDTO>.Error("Giriş Başarısız", "TC No veya şifre hatalı. Lütfen bilgilerinizi kontrol edin.", HttpStatusCode.Unauthorized);
                }

                // Kullanıcı aktif mi kontrol et
                if (!user.IsActive)
                {
                    logger.LogWarning("Giriş denemesi başarısız. Kullanıcı pasif durumda. TCNo: {TCNo}", loginDTO.TCNo);
                    return ServiceResult<TokenDTO>.Error("Hesap Pasif", "Hesabınız pasif durumda. Lütfen yöneticinizle iletişime geçin.", HttpStatusCode.Forbidden);
                }

                // Şifre kontrolü
                var result = await signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);
                if (!result.Succeeded)
                {
                    logger.LogWarning("Giriş denemesi başarısız. Geçersiz şifre. TCNo: {TCNo}", loginDTO.TCNo);
                    return ServiceResult<TokenDTO>.Error("Giriş Başarısız", "TC No veya şifre hatalı. Lütfen bilgilerinizi kontrol edin.", HttpStatusCode.Unauthorized);
                }

                // Token oluştur
                var token = await jwtProvider.CreateTokenAsync(user);

                logger.LogInformation("Kullanıcı başarıyla giriş yaptı. TCNo: {TCNo}", loginDTO.TCNo);

                return ServiceResult<TokenDTO>.SuccessAsOk(token);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "LoginAsync işleminde hata oluştu. TCNo: {TCNo}", loginDTO.TCNo);
                throw;
            }
        }

        public async Task<ServiceResult<bool>> LogoutAsync(CancellationToken cancellationToken = default)
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
                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "LogoutAsync işleminde hata oluştu. UserId: {UserId}", userId);
                throw;
            }
        }

        public async Task<ServiceResult<TokenDTO>> RefreshTokenAsync(CreateTokenByRefreshTokenDTO createTokenByRefreshTokenDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await userManager.FindByIdAsync(createTokenByRefreshTokenDTO.UserId);
                if (user == null)
                {
                    logger.LogWarning("Refresh token denemesi başarısız. Kullanıcı bulunamadı. UserId: {UserId}", createTokenByRefreshTokenDTO.UserId);
                    return ServiceResult<TokenDTO>.Error("Kullanıcı Bulunamadı", "Belirtilen kullanıcı bulunamadı.", HttpStatusCode.NotFound);
                }

                // Kullanıcı aktif mi kontrol et
                if (!user.IsActive)
                {
                    logger.LogWarning("Refresh token denemesi başarısız. Kullanıcı pasif durumda. UserId: {UserId}", createTokenByRefreshTokenDTO.UserId);
                    return ServiceResult<TokenDTO>.Error("Hesap Pasif", "Hesabınız pasif durumda. Lütfen yöneticinizle iletişime geçin.", HttpStatusCode.Forbidden);
                }

                // Refresh token kontrolü
                if (user.RefreshToken != createTokenByRefreshTokenDTO.RefreshToken)
                {
                    logger.LogWarning("Refresh token denemesi başarısız. Geçersiz refresh token. UserId: {UserId} | DatabaseRefreshToken: {DbRefreshToken} | FromUIRefreshToken: {UiRefreshToken}", createTokenByRefreshTokenDTO.UserId, user.RefreshToken, createTokenByRefreshTokenDTO.RefreshToken);
                    return ServiceResult<TokenDTO>.Error("Geçersiz Token", "Geçersiz refresh token. Lütfen tekrar giriş yapın.", HttpStatusCode.Unauthorized);
                }

                // Refresh token süresi dolmuş mu kontrol et
                if (user.RefreshTokenExpires == null || user.RefreshTokenExpires <= DateTime.UtcNow)
                {
                    logger.LogWarning("Refresh token denemesi başarısız. Refresh token süresi dolmuş. UserId: {UserId}", createTokenByRefreshTokenDTO.UserId);
                    return ServiceResult<TokenDTO>.Error("Token Süresi Dolmuş", "Refresh token süresi dolmuş. Lütfen tekrar giriş yapın.", HttpStatusCode.Unauthorized);
                }
                // ESKİ REFRESH TOKEN'I GEÇERSİZ KIL (Rotating Refresh Token Pattern)
                user.RefreshToken = null; // Geçici olarak null yap
                user.RefreshTokenExpires = null; // Geçici olarak null yap
                await userManager.UpdateAsync(user);
                // Yeni token oluştur
                var newToken = await jwtProvider.CreateTokenAsync(user);

                logger.LogInformation("Token başarıyla yenilendi. UserId: {UserId}", createTokenByRefreshTokenDTO.UserId);

                return ServiceResult<TokenDTO>.SuccessAsOk(newToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "RefreshTokenAsync işleminde hata oluştu. UserId: {UserId}", createTokenByRefreshTokenDTO.UserId);
                throw;
            }
        }

        public async Task<ServiceResult<bool>> ResetPasswordAsync(ResetPasswordDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await userManager.FindByNameAsync(dto.TCNo);
                if (user == null)
                {
                    logger.LogWarning("Şifre sıfırlama denemesi. Kullanıcı bulunamadı. TCNo: {TCNo}", dto.TCNo);
                    return ServiceResult<bool>.Error("Kullanıcı Bulunamadı", "Bu TC numarasına kayıtlı kullanıcı bulunamadı.", HttpStatusCode.NotFound);
                }

                // Kullanıcı aktif mi kontrol et
                if (!user.IsActive)
                {
                    return ServiceResult<bool>.Error("Hesap Pasif", "Hesabınız pasif durumda. Lütfen yöneticinizle iletişime geçin.", HttpStatusCode.Forbidden);
                }
                if (string.IsNullOrEmpty(user.PasswordResetCode) || user.PasswordResetCode != dto.VerificationCode)
                {
                    logger.LogWarning("Şifre sıfırlama denemesi. Geçersiz doğrulama kodu. TCNo: {TCNo}, Girilen Kod: {Code}", dto.TCNo, dto.VerificationCode);
                    return ServiceResult<bool>.Error("Geçersiz Doğrulama Kodu", "Girdiğiniz doğrulama kodu geçersiz. Lütfen kontrol edin.", HttpStatusCode.BadRequest);
                }
                if (user.PasswordResetCodeExpires == null || user.PasswordResetCodeExpires <= DateTime.UtcNow)
                {
                    logger.LogWarning("Şifre sıfırlama denemesi. Doğrulama kodu süresi dolmuş. TCNo: {TCNo}", dto.TCNo);
                    return ServiceResult<bool>.Error("Kod Süresi Dolmuş", "Doğrulama kodunun süresi dolmuş. Lütfen yeni kod talep edin.", HttpStatusCode.BadRequest);
                }
                if (dto.NewPassword != dto.ConfirmPassword)
                {
                    logger.LogWarning("Şifre sıfırlama denemesi. Şifre ve onay şifresi uyuşmuyor. TCNo: {TCNo}", dto.TCNo);
                    return ServiceResult<bool>.Error("Şifre Uyumsuzluğu", "Şifre ve onay şifresi uyuşmuyor. Lütfen kontrol edin.", HttpStatusCode.BadRequest);
                }
                // Şifre güncelleme kontrolü
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var result = await userManager.ResetPasswordAsync(user, token, dto.NewPassword);

                // Doğrulama kodunu temizle
                user.PasswordResetCode = null;
                user.PasswordResetCodeExpires = null;

                user.RefreshToken = null;
                user.RefreshTokenExpires = null;

                await userManager.UpdateAsync(user);

                logger.LogInformation("Şifre başarıyla sıfırlandı. TCNo: {TCNo}", dto.TCNo);
                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ResetPasswordAsync işleminde hata oluştu. TCNo: {TCNo}", dto.TCNo);
                throw;
            }
        }

        public async Task<ServiceResult<bool>> SendResetPasswordEmailAsync(ForgotPasswordDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await userManager.FindByNameAsync(dto.TCNo);
                if (user == null)
                {
                    logger.LogWarning("Şifre sıfırlama talebi. Kullanıcı bulunamadı. TCNo: {TCNo}", dto.TCNo);
                    return ServiceResult<bool>.Error("Kullanıcı Bulunamadı", "Gönderilen TC numarasına kayıtlı kullanıcı bulunamadı.", HttpStatusCode.NotFound);
                }

                if (string.IsNullOrEmpty(user.Email))
                {
                    logger.LogWarning("Şifre sıfırlama talebi. Kullanıcının email adresi yok. Email: {Email}", user.Email);
                    return ServiceResult<bool>.Error("E-posta Adresi Bulunamadı", "Bu TC numarasına kayıtlı e-posta adresi bulunamadı.", HttpStatusCode.BadRequest);
                }
                // Kullanıcı aktif mi kontrol et
                if (!user.IsActive)
                {
                    logger.LogWarning("Şifre sıfırlama talebi. Kullanıcı pasif durumda. Email: {Email}", user.Email);
                    return ServiceResult<bool>.Error("Hesap Pasif", "Bu TC numarasına ait kayıtlı hesap pasif durumda.", HttpStatusCode.Forbidden);
                }

                // 6 haneli doğrulama kodu oluştur
                var verificationCode = GenerateVerificationCode();

                // Kodu ve süresini veritabanına kaydet
                user.PasswordResetCode = verificationCode;
                user.PasswordResetCodeExpires = DateTime.UtcNow.AddMinutes(5); // 5 dakika geçerli
                await userManager.UpdateAsync(user);

                // Email gönder
                var emailServiceResult = await emailService.SendVerificationCodeEmailAsync(user.Email, verificationCode, user.Name + " " + user.Surname, cancellationToken);

                if (!emailServiceResult.IsSuccess)
                {
                    logger.LogError("Doğrulama kodu emaili gönderilemedi. Email: {Email}", user.Email);
                    return ServiceResult<bool>.Error("E-posta Gönderim Hatası", "E-posta gönderilirken bir hata oluştu. Lütfen daha sonra tekrar deneyin.", HttpStatusCode.InternalServerError);
                }

                logger.LogInformation("Doğrulama kodu emaili başarıyla gönderildi. Email: {Email}", user.Email);
                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "SendResetPasswordEmailAsync işleminde hata oluştu. TCNo: {TCNo}", dto.TCNo);
                throw;
            }
        }

        private static string GenerateVerificationCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString(); // 6 haneli kod
        }
    }
}