using IdeKusgozManagement.Domain.Entities;
using IdeKusgozManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdeKusgozManagement.Infrastructure.Data.Configurations
{
    public class IdtAdvanceConfiguration : BaseEntityConfiguration<IdtAdvance>
    {
        public override void Configure(EntityTypeBuilder<IdtAdvance> builder)
        {
            base.Configure(builder); // BaseEntity konfigürasyonlarını uygula

            builder.Property(x => x.Amount)
               .IsRequired()
               .HasPrecision(18, 2);
            builder.Property(x => x.Reason)
             .IsRequired()
             .HasMaxLength(500);
            builder.Property(x => x.Status)
                .IsRequired()
                .HasDefaultValue(AdvanceStatus.Pending)
                .HasConversion<int>();

            builder.Property(x => x.Description)
            .HasDefaultValue(null);

            builder.Property(x => x.UnitManagerProcessedDate)
                .IsRequired(false)
                .HasDefaultValue(null);

            builder.Property(x => x.ChiefProcessedDate)
                .IsRequired(false)
                .HasDefaultValue(null);

            builder.Property(x => x.ProcessedByUnitManagerId)
                .IsRequired(false)
                .HasMaxLength(450)
                .HasDefaultValue(null);

            builder.Property(x => x.ProcessedByChiefId)
                .IsRequired(false)
                .HasMaxLength(450)
                .HasDefaultValue(null);

            builder.Property(x => x.ChiefRejectReason)
                .IsRequired(false)
             .HasMaxLength(500)
             .HasDefaultValue(null);
            builder.Property(x => x.UnitManagerRejectReason)
                .IsRequired(false)
             .HasMaxLength(500)
             .HasDefaultValue(null);

            builder.HasOne(x => x.CreatedByUser)
             .WithMany()
             .HasForeignKey(x => x.CreatedBy)
             .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ChiefUser)
             .WithMany()
             .HasForeignKey(x => x.ProcessedByChiefId)
             .OnDelete(DeleteBehavior.NoAction)
             .IsRequired(false);

            builder.HasOne(x => x.UnitManagerUser)
             .WithMany()
             .HasForeignKey(x => x.ProcessedByUnitManagerId)
             .OnDelete(DeleteBehavior.NoAction)
             .IsRequired(false);

            builder.HasOne(x => x.UpdatedByUser)
                .WithMany()
                .HasForeignKey(x => x.UpdatedBy)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);
        }
    }
}