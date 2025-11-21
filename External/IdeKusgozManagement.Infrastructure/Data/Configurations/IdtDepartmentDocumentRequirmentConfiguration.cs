using IdeKusgozManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdeKusgozManagement.Infrastructure.Data.Configurations
{
    public class IdtDepartmentDocumentRequirmentConfiguration : BaseEntityConfiguration<IdtDepartmentDocumentRequirment>
    {
        public override void Configure(EntityTypeBuilder<IdtDepartmentDocumentRequirment> builder)
        {
            base.Configure(builder);
            builder.Property(x => x.DepartmentId)
             .IsRequired()
             .HasMaxLength(450);
            builder.Property(x => x.DocumentTypeId)
                .IsRequired()
                .HasMaxLength(450);
            builder.Property(x => x.DepartmentDutyId)
                .IsRequired()
                .HasMaxLength(450);
            builder.Property(x => x.CompanyId)
             .IsRequired(false)
             .HasMaxLength(450);

            builder.HasOne(d => d.Department)
               .WithMany(p => p.RequiredDepartmentDocuments)
               .HasForeignKey(d => d.DepartmentId)
               .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(d => d.DocumentType)
                 .WithMany(p => p.RequiredDepartmentDocuments)
                 .HasForeignKey(d => d.DocumentTypeId)
                 .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(d => d.DepartmentDuty)
                       .WithMany(p => p.RequiredDepartmentDocuments)
                       .HasForeignKey(d => d.DepartmentDutyId)
                       .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(d => d.Company)
                    .WithMany(p => p.RequiredDepartmentDocuments)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);
        }
    }
}