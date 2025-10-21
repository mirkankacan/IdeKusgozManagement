using IdeKusgozManagement.Application.DTOs.WorkRecordExpenseDTOs;
using IdeKusgozManagement.Domain.Entities;
using Mapster;

namespace IdeKusgozManagement.Application.Mappings
{
    public class WorkRecordExpenseMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<IdtWorkRecordExpense, WorkRecordExpenseDTO>()
                            .Map(dest => dest.ExpenseName, src => src.Expense.Name)
                            .Map(dest => dest.FileId, src => src.File != null ? src.File.Id : null)
                            .Map(dest => dest.FilePath, src => src.File != null ? src.File.Path : null)
                            .Map(dest => dest.OriginalFileName, src => src.File != null ? src.File.OriginalName : null);

            config.NewConfig<CreateOrModifyWorkRecordExpenseDTO, IdtWorkRecordExpense>()
              .Ignore(x => x.File);

            config.NewConfig<UpdateWorkRecordExpenseDTO, IdtWorkRecordExpense>()
              .Ignore(x => x.File);
        }
    }
}