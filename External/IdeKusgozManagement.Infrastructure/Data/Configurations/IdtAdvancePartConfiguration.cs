using IdeKusgozManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdeKusgozManagement.Infrastructure.Data.Configurations
{
    public class IdtAdvancePartConfiguration : BaseEntityConfiguration<IdtAdvancePart>
    {
        public override void Configure(EntityTypeBuilder<IdtAdvancePart> builder)
        {
            base.Configure(builder); // BaseEntity konfigürasyonlarını uygula

            builder.Property(x => x.AdvanceId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(x => x.Amount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(x => x.ApprovalDate)
                .IsRequired();

            builder.Property(x => x.ApprovedById)
                .IsRequired(false)
                .HasMaxLength(450)
                .HasDefaultValue(null);

            builder.Property(x => x.ApprovedDate)
                .IsRequired(false)
                .HasDefaultValue(null);

            // Relationship with IdtAdvance
            builder.HasOne(x => x.Advance)
                .WithMany(x => x.AdvanceParts)
                .HasForeignKey(x => x.AdvanceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship with ApplicationUser (ApprovedBy)
            builder.HasOne(x => x.ApprovedByUser)
                .WithMany()
                .HasForeignKey(x => x.ApprovedById)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);
        }
    }
}

