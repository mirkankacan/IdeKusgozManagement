using IdeKusgozManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdeKusgozManagement.Infrastructure.Data.Configurations
{
    public class IdtParameterConfiguration : BaseEntityConfiguration<IdtParameter>
    {
        public void Configure(EntityTypeBuilder<IdtParameter> builder)
        {
            base.Configure(builder); // BaseEntity konfigürasyonlarını uygula

            builder.Property(x => x.Key)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Value)
              .IsRequired()
              .HasMaxLength(50);
        }
    }
}