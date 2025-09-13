using IdeKusgozManagement.Application.DTOs.WorkRecordDTOs;
using IdeKusgozManagement.Application.DTOs.WorkRecordExpenseDTOs;
using IdeKusgozManagement.Domain.Entities;
using Mapster;

namespace IdeKusgozManagement.Application.Mappings
{
    public static class WorkRecordMappingConfig
    {
        public static void Configure()
        {
            // IdtWorkRecord -> WorkRecordDTO
            TypeAdapterConfig<IdtWorkRecord, WorkRecordDTO>
                .NewConfig()
                   .Map(dest => dest.CreatedByName, src => src.CreatedByUser != null
                    ? $"{src.CreatedByUser.Name} {src.CreatedByUser.Surname}"
                    : string.Empty)
                .Map(dest => dest.UpdatedByName, src => src.UpdatedByUser != null
                  ? $"{src.UpdatedByUser.Name} {src.UpdatedByUser.Surname}"
                    : string.Empty)
                .Map(dest => dest.Expenses, src => (List<WorkRecordExpenseDTO>?)null); // Varsayılan olarak null, servis tarafında doldurulacak

            // CreateWorkRecordDTO -> IdtWorkRecord
            TypeAdapterConfig<CreateWorkRecordDTO, IdtWorkRecord>
                .NewConfig()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.CreatedDate)
                .Ignore(dest => dest.UpdatedDate)
                .Ignore(dest => dest.CreatedBy)
                .Ignore(dest => dest.UpdatedBy);

            // UpdateWorkRecordDTO -> IdtWorkRecord
            TypeAdapterConfig<UpdateWorkRecordDTO, IdtWorkRecord>
                .NewConfig()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.Date)
                .Ignore(dest => dest.CreatedDate)
                .Ignore(dest => dest.UpdatedDate)
                .Ignore(dest => dest.CreatedBy)
                .Ignore(dest => dest.UpdatedBy)
                .Ignore(dest => dest.Status);
        }
    }
}