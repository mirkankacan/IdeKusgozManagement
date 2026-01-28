using IdeKusgozManagement.Application.Common;

namespace IdeKusgozManagement.Application.Contracts.Services
{
    public interface IEmailService
    {
        Task<ServiceResult<bool>> SendVerificationCodeEmailAsync(string toEmail, string verificationCode, string toFullName, CancellationToken cancellationToken = default);
    }
}