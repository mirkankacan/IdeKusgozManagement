using IdeKusgozManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdeKusgozManagement.Infrastructure.Data.Configurations
{
    public class IdtNotificationReadConfiguration : BaseEntityConfiguration<IdtNotificationRead>
    {
        public override void Configure(EntityTypeBuilder<IdtNotificationRead> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.NotificationId)
                .IsRequired();

            builder.Property(x => x.IsRead)
                .HasDefaultValue(true);

            builder.HasOne(x => x.Notification)
                .WithMany(x => x.NotificationReads)
                .HasForeignKey(x => x.NotificationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.CreatedByUser)
                .WithMany()
                .HasForeignKey(x => x.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}