using IdeKusgozManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdeKusgozManagement.Infrastructure.Data.Configurations
{
    public class IdtFileConfiguration : BaseEntityConfiguration<IdtFile>
    {
        public override void Configure(EntityTypeBuilder<IdtFile> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(450);
            builder.Property(x => x.Path)
                .IsRequired()
                .HasMaxLength(500);
            builder.Property(x => x.OriginalName)
                .IsRequired()
                .HasMaxLength(450);
        }
    }
}