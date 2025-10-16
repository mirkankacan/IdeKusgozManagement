using IdeKusgozManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdeKusgozManagement.Infrastructure.Data.Configurations
{
    public class IdtWorkRecordExpenseConfiguration : BaseEntityConfiguration<IdtWorkRecordExpense>
    {
        public override void Configure(EntityTypeBuilder<IdtWorkRecordExpense> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.WorkRecordId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(x => x.ExpenseId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(x => x.Description)
                .IsRequired(false)
                .HasMaxLength(1000);

            builder.Property(x => x.Amount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(x => x.FileId)
                .IsRequired()
                .HasMaxLength(450);

            builder.HasOne(x => x.WorkRecord)
                .WithMany(x => x.WorkRecordExpenses)
                .HasForeignKey(x => x.WorkRecordId)
                .OnDelete(DeleteBehavior.Cascade); // WorkRecord silinince expenses de silinir

            builder.HasOne(x => x.Expense)
                .WithMany(x => x.WorkRecordExpenses)
                .HasForeignKey(x => x.ExpenseId)
                .OnDelete(DeleteBehavior.Restrict); // Expense silinemez, kullanımda

            builder.HasOne(x => x.File)
                .WithMany(x => x.WorkRecordExpenses)
                .HasForeignKey(x => x.FileId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // Indexes
            builder.HasIndex(x => x.WorkRecordId); // WorkRecord'a göre expense'leri getirmek için
        }
    }
}