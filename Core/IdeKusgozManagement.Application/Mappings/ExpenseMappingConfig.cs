using IdeKusgozManagement.Application.DTOs.ExpenseDTOs;
using IdeKusgozManagement.Domain.Entities;
using Mapster;

namespace IdeKusgozManagement.Application.Mappings
{
    public static class ExpenseMappingConfig
    {
        public static void Configure()
        {
            // IdtExpense -> ExpenseListDTO
            TypeAdapterConfig<IdtExpense, ExpenseDTO>
                .NewConfig();

            // CreateExpenseDTO -> IdtExpense
            TypeAdapterConfig<CreateExpenseDTO, IdtExpense>
                .NewConfig()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.CreatedDate)
                .Ignore(dest => dest.UpdatedDate)
                .Ignore(dest => dest.CreatedBy)
                .Ignore(dest => dest.UpdatedBy);

            // UpdateExpenseDTO -> IdtExpense
            TypeAdapterConfig<UpdateExpenseDTO, IdtExpense>
                .NewConfig()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.CreatedDate)
                .Ignore(dest => dest.UpdatedDate)
                .Ignore(dest => dest.CreatedBy)
                .Ignore(dest => dest.UpdatedBy);
        }
    }
}