using IdeKusgozManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdeKusgozManagement.Infrastructure.Data.Configurations
{
    public class IdtProjectConfiguration : BaseEntityConfiguration<IdtProject>
    {
        public override void Configure(EntityTypeBuilder<IdtProject> builder)
        {
            base.Configure(builder);
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(300);
            builder.Property(x => x.StartDate)
                .IsRequired();
            builder.Property(x => x.EndDate)
                .IsRequired();
            builder.Property(x => x.TargetEquipmentIds)
             .IsRequired(false);
            builder.Property(x => x.TargetUserIds)
            .IsRequired(false);
            builder.Property(x => x.ProjectColor)
                .HasMaxLength(10)
             .IsRequired();
            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
        }
    }
}