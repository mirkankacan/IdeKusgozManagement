using IdeKusgozManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdeKusgozManagement.Infrastructure.Data.Configurations
{
    public class IdtMessageConfiguration : BaseEntityConfiguration<IdtMessage>
    {
        public override void Configure(EntityTypeBuilder<IdtMessage> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Content)
                .IsRequired()
                .HasMaxLength(2000);
            builder.Property(x => x.TargetUsers)
                .HasMaxLength(1000);

            builder.Property(x => x.TargetRoles)
                .HasMaxLength(1000);

            builder.HasOne(x => x.CreatedByUser)
                .WithMany()
                .HasForeignKey(x => x.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}