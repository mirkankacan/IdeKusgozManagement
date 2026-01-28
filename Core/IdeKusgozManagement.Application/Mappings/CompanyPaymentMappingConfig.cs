using IdeKusgozManagement.Application.DTOs.CompanyPaymentDTOs;
using IdeKusgozManagement.Domain.Entities;
using Mapster;

namespace IdeKusgozManagement.Application.Mappings
{
    public class CompanyPaymentMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CreateCompanyPaymentDTO, IdtCompanyPayment>()
                .Ignore(x => x.Equipment)
                .Ignore(x => x.Expense)
                .Ignore(x => x.Project)
                .Ignore(x => x.Approver)
                .Ignore(x => x.FileIds);

            config.NewConfig<UpdateCompanyPaymentDTO, IdtCompanyPayment>()
                .Ignore(x => x.Equipment)
                .Ignore(x => x.Expense)
                .Ignore(x => x.Project)
                .Ignore(x => x.Approver)
                .Ignore(x => x.FileIds);

            config.NewConfig<IdtCompanyPayment, CompanyPaymentDTO>()
                .Map(dest => dest.Equipment, src => src.Equipment != null ? src.Equipment.Name : string.Empty)
                .Map(dest => dest.Expense, src => src.Expense != null ? src.Expense.Name : string.Empty)
                .Map(dest => dest.Project, src => src.Project != null ? src.Project.Name : string.Empty)
                .Map(dest => dest.CreatedByFullName, src => src.CreatedByUser.Name + " " + src.CreatedByUser.Surname)
                .Map(dest => dest.SelectedApproverFullName, src => src.Approver != null ? src.Approver.Name + " " + src.Approver.Surname : string.Empty)
                .Map(dest => dest.FileIds, src => string.IsNullOrEmpty(src.FileIds)
                    ? new List<string>()
                    : src.FileIds.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
        }
    }
}