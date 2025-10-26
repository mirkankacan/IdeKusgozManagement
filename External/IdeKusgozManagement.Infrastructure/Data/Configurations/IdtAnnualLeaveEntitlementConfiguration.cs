using IdeKusgozManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdeKusgozManagement.Infrastructure.Data.Configurations
{
    internal class IdtAnnualLeaveEntitlementConfiguration : BaseEntityConfiguration<IdtAnnualLeaveEntitlement>
    {
        public override void Configure(EntityTypeBuilder<IdtAnnualLeaveEntitlement> builder)
        {
            base.Configure(builder); // BaseEntity konfigürasyonlarını uygula

            builder.Property(x => x.UserId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(x => x.Entitlement)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Year)
                .IsRequired();

            builder.HasOne(x => x.User)
                  .WithMany()
                  .HasForeignKey(x => x.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        }
    }
}