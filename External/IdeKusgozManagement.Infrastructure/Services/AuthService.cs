using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.AuthDTOs;
using IdeKusgozManagement.Application.Interfaces.Providers;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IJwtProvider jwtProvider, IIdentityService identityService, ILogger<AuthService> logger, IEmailService emailService) : IAuthService
    {
        public async Task<ServiceResponse<TokenDTO>> LoginAsync(LoginDTO loginDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                // Kullanıcıyı bul
                var user = await userManager.FindByNameAsync(loginDTO.TCNo);
                if (user == null)
                {
                    logger.LogWarning("Giriş denemesi başarısız. Kullanıcı bulunamadı. TCNo: {TCNo}", loginDTO.TCNo);
                    return ServiceResponse<TokenDTO>.Error("TC No veya şifre hatalı");
                }

                // Kullanıcı aktif mi kontrol et
                if (!user.IsActive)
                {
                    logger.LogWarning("Giriş denemesi başarısız. Kullanıcı pasif durumda. TCNo: {TCNo}", loginDTO.TCNo);
                    return ServiceResponse<TokenDTO>.Error("Hesabınız pasif durumda");
                }

                // Şifre kontrolü
                var result = await signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);
                if (!result.Succeeded)
                {
                    logger.LogWarning("Giriş denemesi başarısız. Geçersiz şifre. TCNo: {TCNo}", loginDTO.TCNo);
                    return ServiceResponse<TokenDTO>.Error("Şifre hatalı");
                }

                // Token oluştur
                var token = await jwtProvider.CreateTokenAsync(user);

                logger.LogInformation("Kullanıcı başarıyla giriş yaptı. TCNo: {TCNo}", loginDTO.TCNo);

                return ServiceResponse<TokenDTO>.Success(token, "Giriş başarılı");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "LoginAsync işleminde hata oluştu. TCNo: {TCNo}", loginDTO.TCNo);
                return ServiceResponse<TokenDTO>.Error("Giriş işlemi sırasında bir hata oluştu");
            }
        }

        public async Task<ServiceResponse<bool>> LogoutAsync(CancellationToken cancellationToken = default)
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
                return ServiceResponse<bool>.Success(true, "Çıkış başarılı");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "LogoutAsync işleminde hata oluştu. UserId: {UserId}", userId);
                return ServiceResponse<bool>.Error("Çıkış işlemi sırasında bir hata oluştu");
            }
        }

        public async Task<ServiceResponse<TokenDTO>> RefreshTokenAsync(CreateTokenByRefreshTokenDTO createTokenByRefreshTokenDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await userManager.FindByIdAsync(createTokenByRefreshTokenDTO.UserId);
                if (user == null)
                {
                    logger.LogWarning("Refresh token denemesi başarısız. Kullanıcı bulunamadı. UserId: {UserId}", createTokenByRefreshTokenDTO.UserId);
                    return ServiceResponse<TokenDTO>.Error("Kullanıcı bulunamadı");
                }

                // Kullanıcı aktif mi kontrol et
                if (!user.IsActive)
                {
                    logger.LogWarning("Refresh token denemesi başarısız. Kullanıcı pasif durumda. UserId: {UserId}", createTokenByRefreshTokenDTO.UserId);
                    return ServiceResponse<TokenDTO>.Error("Hesabınız pasif durumda");
                }

                // Refresh token kontrolü
                if (user.RefreshToken != createTokenByRefreshTokenDTO.RefreshToken)
                {
                    logger.LogWarning("Refresh token denemesi başarısız. Geçersiz refresh token. UserId: {UserId} | DatabaseRefreshToken: {DbRefreshToken} | FromUIRefreshToken: {UiRefreshToken}", createTokenByRefreshTokenDTO.UserId, user.RefreshToken, createTokenByRefreshTokenDTO.RefreshToken);
                    return ServiceResponse<TokenDTO>.Error("Geçersiz refresh token");
                }

                // Refresh token süresi dolmuş mu kontrol et
                if (user.RefreshTokenExpires == null || user.RefreshTokenExpires <= DateTime.UtcNow)
                {
                    logger.LogWarning("Refresh token denemesi başarısız. Refresh token süresi dolmuş. UserId: {UserId}", createTokenByRefreshTokenDTO.UserId);
                    return ServiceResponse<TokenDTO>.Error("Refresh token süresi dolmuş");
                }

                // Yeni token oluştur
                var newToken = await jwtProvider.CreateTokenAsync(user);

                logger.LogInformation("Token başarıyla yenilendi. UserId: {UserId}", createTokenByRefreshTokenDTO.UserId);

                return ServiceResponse<TokenDTO>.Success(newToken, "Token başarıyla yenilendi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "RefreshTokenAsync işleminde hata oluştu. UserId: {UserId}", createTokenByRefreshTokenDTO.UserId);
                return ServiceResponse<TokenDTO>.Error("Token yenileme işlemi sırasında bir hata oluştu");
            }
        }

        public async Task<ServiceResponse<bool>> ResetPasswordAsync(ResetPasswordDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await userManager.FindByNameAsync(dto.TCNo);
                if (user == null)
                {
                    logger.LogWarning("Şifre sıfırlama denemesi. Kullanıcı bulunamadı. TCNo: {TCNo}", dto.TCNo);
                    return ServiceResponse<bool>.Error("Bu TC numarasına kayıtlı kullanıcı bulunamadı");
                }

                // Kullanıcı aktif mi kontrol et
                if (!user.IsActive)
                {
                    return ServiceResponse<bool>.Error("Hesabınız pasif durumda");
                }
                if (string.IsNullOrEmpty(user.PasswordResetCode) || user.PasswordResetCode != dto.VerificationCode)
                {
                    logger.LogWarning("Şifre sıfırlama denemesi. Geçersiz doğrulama kodu. TCNo: {TCNo}, Girilen Kod: {Code}", dto.TCNo, dto.VerificationCode);
                    return ServiceResponse<bool>.Error("Geçersiz doğrulama kodu");
                }
                if (user.PasswordResetCodeExpires == null || user.PasswordResetCodeExpires <= DateTime.UtcNow)
                {
                    logger.LogWarning("Şifre sıfırlama denemesi. Doğrulama kodu süresi dolmuş. TCNo: {TCNo}", dto.TCNo);
                    return ServiceResponse<bool>.Error("Doğrulama kodunun süresi dolmuş. Lütfen yeni kod talep edin.");
                }
                if (dto.NewPassword != dto.ConfirmPassword)
                {
                    logger.LogWarning("Şifre sıfırlama denemesi. Şifre ve onay şifresi uyuşmuyor. TCNo: {TCNo}", dto.TCNo);
                    return ServiceResponse<bool>.Error("Şifre ve onay şifresi uyuşmuyor");
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
                return ServiceResponse<bool>.Success(true, "Şifreniz başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ResetPasswordAsync işleminde hata oluştu. TCNo: {TCNo}", dto.TCNo);
                return ServiceResponse<bool>.Error("Şifre sıfırlama işlemi sırasında bir hata oluştu");
            }
        }

        public async Task<ServiceResponse<bool>> SendResetPasswordEmailAsync(ForgotPasswordDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await userManager.FindByNameAsync(dto.TCNo);
                if (user == null)
                {
                    logger.LogWarning("Şifre sıfırlama talebi. Kullanıcı bulunamadı. TCNo: {TCNo}", dto.TCNo);
                    return ServiceResponse<bool>.Error("Gönderilen TC numarasına kayıtlı kullanıcı bulunamadı");
                }

                if (string.IsNullOrEmpty(user.Email))
                {
                    logger.LogWarning("Şifre sıfırlama talebi. Kullanıcının email adresi yok. Email: {Email}", user.Email);
                    return ServiceResponse<bool>.Error("Bu TC numarasına kayıtlı e-posta adresi bulunamadı");
                }
                // Kullanıcı aktif mi kontrol et
                if (!user.IsActive)
                {
                    logger.LogWarning("Şifre sıfırlama talebi. Kullanıcı pasif durumda. Email: {Email}", user.Email);
                    return ServiceResponse<bool>.Error("Bu TC numarasına ait kayıtlı hesap pasif durumda");
                }

                // 6 haneli doğrulama kodu oluştur
                var verificationCode = GenerateVerificationCode();

                // Kodu ve süresini veritabanına kaydet
                user.PasswordResetCode = verificationCode;
                user.PasswordResetCodeExpires = DateTime.UtcNow.AddMinutes(5); // 5 dakika geçerli
                await userManager.UpdateAsync(user);

                // Email gönder
                var emailServiceResponse = await emailService.SendVerificationCodeEmailAsync(user.Email, verificationCode, user.Name + " " + user.Surname, cancellationToken);

                if (!emailServiceResponse.IsSuccess)
                {
                    logger.LogError("Doğrulama kodu emaili gönderilemedi. Email: {Email}", user.Email);
                    return ServiceResponse<bool>.Error("Email gönderilirken bir hata oluştu");
                }

                logger.LogInformation("Doğrulama kodu emaili başarıyla gönderildi. Email: {Email}", user.Email);
                return ServiceResponse<bool>.Success(true, "Doğrulama kodu email adresinize gönderilmiştir. Kod 5 dakika geçerlidir.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "SendResetPasswordEmailAsync işleminde hata oluştu. TCNo: {TCNo}", dto.TCNo);
                return ServiceResponse<bool>.Error("Doğrulama kodu gönderilirken bir hata oluştu");
            }
        }

        private static string GenerateVerificationCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString(); // 6 haneli kod
        }
    }
}