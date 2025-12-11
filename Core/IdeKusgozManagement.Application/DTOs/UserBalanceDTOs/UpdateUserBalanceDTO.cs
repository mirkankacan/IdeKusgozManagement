using IdeKusgozManagement.Domain.Enums;

namespace IdeKusgozManagement.Application.DTOs.UserBalanceDTOs
{
    public class UpdateUserBalanceDTO
    {
        public BalanceType Type { get; set; }
        public decimal Amount { get; set; }
    }
}