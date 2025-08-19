using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdeKusgozManagement.Application.DTOs.WorkRecordDTOs;
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
                .NewConfig();

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