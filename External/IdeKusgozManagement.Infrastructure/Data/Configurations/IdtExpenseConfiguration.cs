using IdeKusgozManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdeKusgozManagement.Infrastructure.Data.Configurations
{
    public class IdtExpenseConfiguration : BaseEntityConfiguration<IdtExpense>
    {
        public override void Configure(EntityTypeBuilder<IdtExpense> builder)
        {
            base.Configure(builder); // BaseEntity konfigürasyonlarını uygula

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
        }
    }
}