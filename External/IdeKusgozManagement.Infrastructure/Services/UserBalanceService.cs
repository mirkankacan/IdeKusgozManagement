using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.UserBalanceDTOs;
using IdeKusgozManagement.Application.Interfaces.UnitOfWork;
using IdeKusgozManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class UserBalanceService(IUnitOfWork unitOfWork, ILogger<DepartmentService> logger) : IUserBalanceService
    {
        public async Task<ServiceResponse<UserBalanceDTO>> GetUserBalancesByUserAsync(string userId, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    return ServiceResponse<UserBalanceDTO>.Error("Kullanıcı ID'si boş geçilemez");

                var parameters = new object[] { userId };

                var funcResults = await unitOfWork.ExecuteTableValuedFunctionAsync<UserBalanceDTO>(
                        "dbo.IDF_UserAdvanceBalance",
                        parameters,
                        cancellationToken);

                var result = funcResults.FirstOrDefault();

                return result != null
                    ? ServiceResponse<UserBalanceDTO>.Success(result)
                    : ServiceResponse<UserBalanceDTO>.Error("Veri alınamadı");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetUserBalancesByUserAsync işleminde hata oluştu. UserId: {UserId}", userId);
                throw;
            }
        }

        public async Task<ServiceResponse<bool>> IncreaseUserBalanceAsync(string userId, UpdateUserBalanceDTO updateUserBalanceDTO, CancellationToken cancellationToken)
        {
            try
            {
                var userBalance = await unitOfWork.GetRepository<IdtUserBalance>().Where(x => x.UserId == userId && x.Type == updateUserBalanceDTO.Type).FirstOrDefaultAsync(cancellationToken);
                if (userBalance == null)
                    return ServiceResponse<bool>.Error("Kullanıcı bakiyesi bulunamadı");

                userBalance.Balance = userBalance.Balance + updateUserBalanceDTO.Amount;

                unitOfWork.GetRepository<IdtUserBalance>().Update(userBalance);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                return ServiceResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "IncreaseUserBalanceAsync işleminde hata oluştu. UserId: {UserId}, Type: {Type}", userId, updateUserBalanceDTO.Type);
                throw;
            }
        }

        public async Task<ServiceResponse<bool>> DecreaseUserBalanceAsync(string userId, UpdateUserBalanceDTO updateUserBalanceDTO, CancellationToken cancellationToken)
        {
            try
            {
                var userBalance = await unitOfWork.GetRepository<IdtUserBalance>().Where(x => x.UserId == userId && x.Type == updateUserBalanceDTO.Type).FirstOrDefaultAsync(cancellationToken);
                if (userBalance == null)
                    return ServiceResponse<bool>.Error("Kullanıcı bakiyesi bulunamadı");

                userBalance.Balance = userBalance.Balance - updateUserBalanceDTO.Amount;

                unitOfWork.GetRepository<IdtUserBalance>().Update(userBalance);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                return ServiceResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DecreaseUserBalanceAsync işleminde hata oluştu. UserId: {UserId}, Type: {Type}", userId, updateUserBalanceDTO.Type);
                throw;
            }
        }
    }
}