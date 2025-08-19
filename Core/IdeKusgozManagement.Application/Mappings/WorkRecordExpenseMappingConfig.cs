using IdeKusgozManagement.Application.DTOs.WorkRecordExpenseDTOs;
using IdeKusgozManagement.Domain.Entities;
using Mapster;

namespace IdeKusgozManagement.Application.Mappings
{
    public static class WorkRecordExpenseMappingConfig
    {
        public static void Configure()
        {
            // IdtWorkRecordExpense -> WorkRecordExpenseDTO
            TypeAdapterConfig<IdtWorkRecordExpense, WorkRecordExpenseDTO>
                .NewConfig();

            // CreateWorkRecordExpenseDTO -> IdtWorkRecordExpense
            TypeAdapterConfig<CreateWorkRecordExpenseDTO, IdtWorkRecordExpense>
                .NewConfig()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.WorkRecordId)
                .Ignore(dest => dest.CreatedDate)
                .Ignore(dest => dest.UpdatedDate)
                .Ignore(dest => dest.CreatedBy)
                .Ignore(dest => dest.UpdatedBy);

            // UpdateWorkRecordExpenseDTO -> IdtWorkRecordExpense
            TypeAdapterConfig<UpdateWorkRecordExpenseDTO, IdtWorkRecordExpense>
                .NewConfig()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.WorkRecordId)
                .Ignore(dest => dest.CreatedDate)
                .Ignore(dest => dest.UpdatedDate)
                .Ignore(dest => dest.CreatedBy)
                .Ignore(dest => dest.UpdatedBy);
        }
    }
}