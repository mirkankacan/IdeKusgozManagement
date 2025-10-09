using IdeKusgozManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdeKusgozManagement.Infrastructure.Data.Configurations
{
    public class IdtEquipmentConfiguration : BaseEntityConfiguration<IdtEquipment>
    {
        public override void Configure(EntityTypeBuilder<IdtEquipment> builder)
        {
            base.Configure(builder); // BaseEntity konfigürasyonlarını uygula

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
        }
    }
}