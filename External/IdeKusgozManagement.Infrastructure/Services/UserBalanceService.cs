using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.UserBalanceDTOs;
using IdeKusgozManagement.Application.Interfaces.UnitOfWork;
using IdeKusgozManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class UserBalanceService(IUnitOfWork unitOfWork, ILogger<DepartmentService> logger) : IUserBalanceService
    {
        public async Task<ServiceResult<UserBalanceDTO>> GetUserBalancesByUserAsync(string userId, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    return ServiceResult<UserBalanceDTO>.Error("Validasyon Hatası", "Kullanıcı ID'si boş geçilemez.", HttpStatusCode.BadRequest);

                var parameters = new object[] { userId };

                var funcResults = await unitOfWork.ExecuteTableValuedFunctionAsync<UserBalanceDTO>(
                        "dbo.IDF_UserAdvanceBalance",
                        parameters,
                        cancellationToken);

                var result = funcResults.FirstOrDefault();

                return result != null
                    ? ServiceResult<UserBalanceDTO>.SuccessAsOk(result)
                    : ServiceResult<UserBalanceDTO>.Error("Veri Bulunamadı", "Kullanıcı bakiyesi verisi alınamadı.", HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetUserBalancesByUserAsync işleminde hata oluştu. UserId: {UserId}", userId);
                throw;
            }
        }

        public async Task<ServiceResult<bool>> IncreaseUserBalanceAsync(string userId, UpdateUserBalanceDTO updateUserBalanceDTO, CancellationToken cancellationToken)
        {
            try
            {
                var userBalance = await unitOfWork.GetRepository<IdtUserBalance>().Where(x => x.UserId == userId && x.Type == updateUserBalanceDTO.Type).FirstOrDefaultAsync(cancellationToken);
                if (userBalance == null)
                    return ServiceResult<bool>.Error("Bakiye Bulunamadı", "Kullanıcı bakiyesi bulunamadı.", HttpStatusCode.NotFound);

                userBalance.Balance = userBalance.Balance + updateUserBalanceDTO.Amount;

                unitOfWork.GetRepository<IdtUserBalance>().Update(userBalance);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "IncreaseUserBalanceAsync işleminde hata oluştu. UserId: {UserId}, Type: {Type}", userId, updateUserBalanceDTO.Type);
                throw;
            }
        }

        public async Task<ServiceResult<bool>> DecreaseUserBalanceAsync(string userId, UpdateUserBalanceDTO updateUserBalanceDTO, CancellationToken cancellationToken)
        {
            try
            {
                var userBalance = await unitOfWork.GetRepository<IdtUserBalance>().Where(x => x.UserId == userId && x.Type == updateUserBalanceDTO.Type).FirstOrDefaultAsync(cancellationToken);
                if (userBalance == null)
                    return ServiceResult<bool>.Error("Bakiye Bulunamadı", "Kullanıcı bakiyesi bulunamadı.", HttpStatusCode.NotFound);

                userBalance.Balance = userBalance.Balance - updateUserBalanceDTO.Amount;

                unitOfWork.GetRepository<IdtUserBalance>().Update(userBalance);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DecreaseUserBalanceAsync işleminde hata oluştu. UserId: {UserId}, Type: {Type}", userId, updateUserBalanceDTO.Type);
                throw;
            }
        }
    }
}