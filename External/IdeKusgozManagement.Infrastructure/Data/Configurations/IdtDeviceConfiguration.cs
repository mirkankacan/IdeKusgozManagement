using IdeKusgozManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdeKusgozManagement.Infrastructure.Data.Configurations
{
    public class IdtDeviceConfiguration : BaseEntityConfiguration<IdtDevice>
    {
        public override void Configure(EntityTypeBuilder<IdtDevice> builder)
        {
            base.Configure(builder);
            builder.Property(e => e.UserId)
             .HasMaxLength(450)
             .IsRequired();

            builder.Property(e => e.DeviceToken)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(e => e.Platform)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.OneSignalPlayerId)
                .HasMaxLength(450);

            builder.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
        }
    }
}