using IdeKusgozManagement.Application.Common;

namespace IdeKusgozManagement.Application.Contracts.Services
{
    public interface IEmailService
    {
        Task<ServiceResponse<bool>> SendVerificationCodeEmailAsync(string toEmail, string verificationCode, string toFullName, CancellationToken cancellationToken = default);
    }
}