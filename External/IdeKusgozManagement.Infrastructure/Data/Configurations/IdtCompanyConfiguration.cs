using IdeKusgozManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdeKusgozManagement.Infrastructure.Data.Configurations
{
    public class IdtCompanyConfiguration : BaseEntityConfiguration<IdtCompany>
    {
        public override void Configure(EntityTypeBuilder<IdtCompany> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);
        }
    }
}