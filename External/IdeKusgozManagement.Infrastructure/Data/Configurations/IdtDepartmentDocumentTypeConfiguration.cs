using IdeKusgozManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdeKusgozManagement.Infrastructure.Data.Configurations
{
    public class IdtDepartmentDocumentTypeConfiguration : BaseEntityConfiguration<IdtDepartmentDocumentType>
    {
        public override void Configure(EntityTypeBuilder<IdtDepartmentDocumentType> builder)
        {
            base.Configure(builder);

            builder.HasOne(d => d.Department)
                       .WithMany(p => p.RequiredDocuments)
                       .HasForeignKey(d => d.DepartmentId)
                       .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.DocumentType)
                   .WithMany(p => p.RequiredByDepartments)
                   .HasForeignKey(d => d.DocumentTypeId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}