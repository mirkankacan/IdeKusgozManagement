using IdeKusgozManagement.Domain.Entities;
using IdeKusgozManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdeKusgozManagement.Infrastructure.Data.Configurations
{
    public class IdtCompanyPaymentConfiguration : BaseEntityConfiguration<IdtCompanyPayment>
    {
        public override void Configure(EntityTypeBuilder<IdtCompanyPayment> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.EquipmentId)
                .IsRequired(false)
                .HasMaxLength(maxLength: 450);

            builder.Property(x => x.Amount)
             .IsRequired()
             .HasPrecision(18, 2);
            builder.Property(x => x.ExpenseId)
             .IsRequired()
             .HasMaxLength(maxLength: 450);
            builder.Property(x => x.PersonnelNote)
             .IsRequired(false)
             .HasMaxLength(maxLength: 500);
            builder.Property(x => x.ChiefNote)
             .IsRequired(false)
             .HasMaxLength(maxLength: 500);
            builder.Property(x => x.FileIds)
        .IsRequired()
        .HasColumnType("nvarchar(max)");

            builder.Property(x => x.SelectedApproverId)
             .IsRequired(false)
             .HasMaxLength(maxLength: 450);

            builder.Property(x => x.Status)
                 .IsRequired()
                 .HasDefaultValue(CompanyPaymentStatus.Pending)
                 .HasConversion<int>();

            builder.Property(x => x.ProjectId)
          .IsRequired()
          .HasMaxLength(maxLength: 450);

            builder.HasOne(x => x.Equipment)
          .WithMany(x => x.CompanyPayments)
          .HasForeignKey(x => x.EquipmentId)
          .OnDelete(DeleteBehavior.Restrict)
          .IsRequired(false);
            builder.HasOne(x => x.Expense)
       .WithMany(x => x.CompanyPayments)
       .HasForeignKey(x => x.ExpenseId)
       .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Project)
    .WithMany(x => x.CompanyPayments)
    .HasForeignKey(x => x.ProjectId)
    .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Approver)
             .WithMany()
             .HasForeignKey(x => x.SelectedApproverId)
             .OnDelete(DeleteBehavior.Restrict)
             .IsRequired(false);
            builder.HasOne(x => x.CreatedByUser)
           .WithMany()
           .HasForeignKey(x => x.CreatedBy)
           .OnDelete(DeleteBehavior.Restrict);
        }
    }
}