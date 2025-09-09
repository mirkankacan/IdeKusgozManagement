using IdeKusgozManagement.Application.DTOs.LeaveRequestDTOs;
using IdeKusgozManagement.Domain.Entities;
using Mapster;

namespace IdeKusgozManagement.Application.Mappings
{
    public static class LeaveRequestMappingConfig
    {
        public static void Configure()
        {
            TypeAdapterConfig<IdtLeaveRequest, LeaveRequestDTO>
                .NewConfig()
                .Map(dest => dest.CreatedByName, src => src.CreatedByUser != null
                    ? $"{src.CreatedByUser.Name} {src.CreatedByUser.Surname}"
                    : string.Empty)
                .Map(dest => dest.UpdatedByName, src => src.UpdatedByUser != null
                    ? $"{src.UpdatedByUser.Name} {src.UpdatedByUser.Surname}"
                    : null);

            TypeAdapterConfig<CreateLeaveRequestDTO, IdtLeaveRequest>
                .NewConfig()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.CreatedDate)
                .Ignore(dest => dest.UpdatedDate)
                .Ignore(dest => dest.CreatedBy)
                .Ignore(dest => dest.UpdatedBy);

            TypeAdapterConfig<UpdateLeaveRequestDTO, IdtLeaveRequest>
                .NewConfig()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.CreatedDate)
                .Ignore(dest => dest.UpdatedDate)
                .Ignore(dest => dest.CreatedBy)
                .Ignore(dest => dest.UpdatedBy);
        }
    }
}