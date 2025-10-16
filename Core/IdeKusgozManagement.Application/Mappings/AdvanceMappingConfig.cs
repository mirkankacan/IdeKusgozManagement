using IdeKusgozManagement.Application.DTOs.AdvanceDTOs;
using IdeKusgozManagement.Domain.Entities;
using Mapster;

namespace IdeKusgozManagement.Application.Mappings
{
    public class AdvanceMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<IdtAdvance, AdvanceDTO>()
                 .Map(dest => dest.CreatedByFullName, src => $"{src.CreatedByUser.Name} {src.CreatedByUser.Surname}")
                 .Map(dest => dest.ChiefByFullName, src => src.ChiefUser != null ? $"{src.ChiefUser.Name} {src.ChiefUser.Surname}" : null)
                 .Map(dest => dest.UnitManagerFullName, src => src.UnitManagerUser != null ? $"{src.UnitManagerUser.Name} {src.UnitManagerUser.Surname}" : null)
                 .Map(dest => dest.UpdatedByFullName, src => src.UpdatedByUser != null ? $"{src.UpdatedByUser.Name} {src.UpdatedByUser.Surname}" : null);
        }
    }
}