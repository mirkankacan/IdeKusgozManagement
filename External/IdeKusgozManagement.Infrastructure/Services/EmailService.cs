using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.OptionDTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class EmailService(ILogger<EmailService> logger, IOptions<EmailOptionsDTO> options) : IEmailService
    {
        public async Task<ServiceResult<bool>> SendVerificationCodeEmailAsync(string email, string verificationCode, string fullName, CancellationToken cancellationToken = default)
        {
            try
            {
                var smtpSettings = options.Value;
                if (!ValidateSmtpSettings(smtpSettings))
                {
                    logger.LogError("SMTP ayarları eksik veya hatalı");
                    return ServiceResult<bool>.Error("SMTP Yapılandırma Hatası", "SMTP ayarları eksik veya hatalı. Lütfen sistem yöneticisiyle iletişime geçin.", HttpStatusCode.InternalServerError);
                }

                using var smtpClient = CreateSmtpClient(smtpSettings);

                var mailMessage = CreateVerificationCodeMessage(email, verificationCode, fullName, smtpSettings.FromEmail);

                await smtpClient.SendMailAsync(mailMessage, cancellationToken);

                logger.LogInformation("Doğrulama kodu emaili başarıyla gönderildi. Email: {Email}", email);
                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (SmtpException ex)
            {
                logger.LogError(ex, "SMTP hatası oluştu. Email: {Email}, SMTP Kodu: {StatusCode}", email, ex.StatusCode);
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Email gönderilirken beklenmeyen hata oluştu. Email: {Email}", email);
                throw;
            }
        }

        private static bool ValidateSmtpSettings(EmailOptionsDTO settings)
        {
            return !string.IsNullOrEmpty(settings.Host) &&
                   !string.IsNullOrEmpty(settings.FromEmail) &&
                   !string.IsNullOrEmpty(settings.Password) &&
                   settings.Port > 0;
        }

        private static SmtpClient CreateSmtpClient(EmailOptionsDTO settings)
        {
            return new SmtpClient(settings.Host)
            {
                Port = settings.Port,
                Credentials = new NetworkCredential(settings.FromEmail, settings.Password),
                EnableSsl = settings.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };
        }

        private static MailMessage CreateVerificationCodeMessage(string toEmail, string verificationCode, string fullName, string fromAddress)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromAddress, "Kuşgöz Yönetim Sistemi"),
                Subject = "🔑 Şifre Sıfırlama Doğrulama Kodu",
                Body = GetVerificationCodeEmailTemplate(verificationCode, fullName),
                IsBodyHtml = true,
                Priority = MailPriority.High
            };

            mailMessage.To.Add(new MailAddress(toEmail));

            // Email headers - tracking ve güvenlik için
            mailMessage.Headers.Add("X-Mailer", "IDE-Kusgoz-Management-System");
            mailMessage.Headers.Add("X-Priority", "1");
            mailMessage.Headers.Add("X-MSMail-Priority", "High");

            return mailMessage;
        }

        private static string GetVerificationCodeEmailTemplate(string verificationCode, string fullName)
        {
            return $@"
<!DOCTYPE html>
<html lang='tr'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Şifre Sıfırlama Doğrulama Kodu</title>
</head>
<body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f4f4;'>
    <table role='presentation' style='width: 100%; border-collapse: collapse;'>
        <tr>
            <td style='padding: 20px 0;'>
                <table role='presentation' style='max-width: 600px; margin: 0 auto; background-color: white; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);'>
                    <tr>
                        <td style='padding: 40px 30px; text-align: center;'>
                            <h1 style='color: #2c3e50; margin: 0 0 30px 0; font-size: 24px;'>Şifre Sıfırlama</h1>

                            <p style='color: #555; font-size: 16px; line-height: 1.6; margin: 0 0 20px 0;'>
                                Merhaba <strong>{fullName}</strong>,
                            </p>

                            <p style='color: #555; font-size: 16px; line-height: 1.6; margin: 0 0 30px 0;'>
                                Şifrenizi sıfırlama talebinde bulundunuz. Aşağıdaki 6 haneli doğrulama kodunu şifre sıfırlama penceresinde kullanın:
                            </p>

                            <table role='presentation' style='margin: 30px auto;'>
                                <tr>
                                    <td style='background-color: #f8f9fa; border: 2px dashed #3498db; padding: 20px; border-radius: 10px; text-align: center;'>
                                        <span style='font-size: 32px; font-weight: bold; color: #2c3e50; letter-spacing: 5px; font-family: monospace;'>{verificationCode}</span>
                                    </td>
                                </tr>
                            </table>

                            <table role='presentation' style='background-color: #fff3cd; border: 1px solid #ffeaa7; border-radius: 5px; margin: 20px 0; width: 100%;'>
                                <tr>
                                    <td style='padding: 15px;'>
                                        <h3 style='color: #856404; margin: 0 0 10px 0; font-size: 16px;'>⚠️ Önemli Bilgi</h3>
                                        <p style='color: #856404; margin: 0; font-size: 14px;'>
                                            Şifre sıfırlama penceresinde bu <strong>doğrulama kodunu</strong> girmeniz gerekmektedir.
                                        </p>
                                    </td>
                                </tr>
                            </table>

                            <p style='color: #777; font-size: 14px; line-height: 1.6; margin: 20px 0;'>
                                Bu kod <strong>5 dakika</strong> geçerlidir. Eğer bu talebi siz yapmadıysanız, bu emaili görmezden gelebilirsiniz.
                            </p>

                            <p style='color: #e74c3c; font-size: 14px; line-height: 1.6; font-weight: bold; margin: 20px 0;'>
                                🔒 Güvenliğiniz için bu kodu kimseyle paylaşmayın.
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td style='background-color: #f8f9fa; padding: 20px; text-align: center; border-top: 1px solid #eee; border-radius: 0 0 10px 10px;'>
                            <p style='color: #999; font-size: 12px; margin: 0;'>
                                Kuşgöz Yönetim Sistemi<br>
                                Bu e-posta otomatik olarak gönderilmiştir.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
        }
    }
}