using IdeKusgozManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdeKusgozManagement.Infrastructure.Data.Configurations
{
    public class IdtDocumentTypeConfiguration : BaseEntityConfiguration<IdtDocumentType>
    {
        public override void Configure(EntityTypeBuilder<IdtDocumentType> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.RenewalPeriodInMonths)
              .HasDefaultValue(null);
        }
    }
}