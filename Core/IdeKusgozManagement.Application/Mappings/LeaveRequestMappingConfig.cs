using IdeKusgozManagement.Application.DTOs.LeaveRequestDTOs;
using IdeKusgozManagement.Domain.Entities;
using Mapster;

namespace IdeKusgozManagement.Application.Mappings
{
    public class LeaveRequestMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<IdtLeaveRequest, LeaveRequestDTO>()
                 .Map(dest => dest.CreatedByFullName, src => $"{src.CreatedByUser.Name} {src.CreatedByUser.Surname}")
                 .Map(dest => dest.UpdatedByFullName, src => src.UpdatedByUser != null ? $"{src.UpdatedByUser.Name} {src.UpdatedByUser.Surname}" : null)
                 .Map(dest => dest.FilePath, src => src.File != null ? src.File.Path : null);
        }
    }
}