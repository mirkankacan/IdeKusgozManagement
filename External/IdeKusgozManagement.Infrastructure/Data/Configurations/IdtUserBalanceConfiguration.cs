using IdeKusgozManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdeKusgozManagement.Infrastructure.Data.Configurations
{
    public class IdtUserBalanceConfiguration : BaseEntityConfiguration<IdtUserBalance>
    {
        public override void Configure(EntityTypeBuilder<IdtUserBalance> builder)
        {
            base.Configure(builder); // BaseEntity konfigürasyonlarını uygula

            builder.Property(x => x.UserId)
                .IsRequired()
                .HasMaxLength(450);
            builder.Property(x => x.Type)
               .IsRequired()
               .HasConversion<int>();
            builder.Property(x => x.Balance)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.HasOne(x => x.User)
                  .WithMany()
                  .HasForeignKey(x => x.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        }
    }
}