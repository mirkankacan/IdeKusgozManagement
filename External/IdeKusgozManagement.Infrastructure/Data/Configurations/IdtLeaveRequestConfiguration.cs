using IdeKusgozManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdeKusgozManagement.Infrastructure.Data.Configurations
{
    public class IdtLeaveRequestConfiguration : BaseEntityConfiguration<IdtLeaveRequest>
    {
        public override void Configure(EntityTypeBuilder<IdtLeaveRequest> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.StartDate)
                .IsRequired();

            builder.Property(x => x.EndDate)
                .IsRequired();

            builder.Property(x => x.Reason)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Description)
                .IsRequired(false)
                .HasMaxLength(2000)
                 .HasDefaultValue(null);

            builder.Property(x => x.Status)
                .IsRequired()
                .HasConversion<int>(); // Enum'u int olarak kaydet

            builder.Property(x => x.FileId)
                .IsRequired(false)
                .HasMaxLength(450)
                .HasDefaultValue(null);

            builder.Property(x => x.Duration)
                .IsRequired(false)
                .HasMaxLength(50);

            builder.Property(x => x.RejectReason)
                .IsRequired(false)
                .HasMaxLength(1000);

            // Relationships
            builder.HasOne(x => x.File)
                .WithMany(x => x.LeaveRequests)
                .HasForeignKey(x => x.FileId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            builder.HasOne(x => x.CreatedByUser)
                .WithMany()
                .HasForeignKey(x => x.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.UpdatedByUser)
                .WithMany()
                .HasForeignKey(x => x.UpdatedBy)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
        }
    }
}