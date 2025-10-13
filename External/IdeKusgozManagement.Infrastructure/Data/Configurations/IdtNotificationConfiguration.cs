using IdeKusgozManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdeKusgozManagement.Infrastructure.Data.Configurations
{
    public class IdtNotificationConfiguration : BaseEntityConfiguration<IdtNotification>
    {
        public override void Configure(EntityTypeBuilder<IdtNotification> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Message)
                .IsRequired()
                .HasMaxLength(2000);
            builder.Property(x => x.Type)
                .IsRequired()
                .HasConversion<int>(); // Enum'u int olarak kaydet

            builder.Property(x => x.TargetUsers)
                .IsRequired(false)
                .HasMaxLength(1000);

            builder.Property(x => x.TargetRoles)
                .IsRequired(false)
                .HasMaxLength(1000);

            builder.Property(x => x.RedirectUrl)
                .IsRequired(false)
                .HasMaxLength(2000);

            builder.HasOne(x => x.CreatedByUser)
                .WithMany()
                .HasForeignKey(x => x.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}