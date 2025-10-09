using IdeKusgozManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdeKusgozManagement.Infrastructure.Data.Configurations
{
    public class IdtWorkRecordConfiguration : BaseEntityConfiguration<IdtWorkRecord>
    {
        public override void Configure(EntityTypeBuilder<IdtWorkRecord> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Date)
                .IsRequired();

            builder.Property(x => x.ExcuseReason)
                .HasMaxLength(100);

            builder.Property(x => x.StartTime)
                .IsRequired();

            builder.Property(x => x.EndTime)
                .IsRequired();

            builder.Property(x => x.ProjectId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(x => x.EquipmentId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(x => x.Province)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.District)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.HasBreakfast)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.HasLunch)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.HasDinner)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.HasNightMeal)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.HasTravel)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.Status)
                .IsRequired()
                .HasConversion<int>(); // Enum'u int olarak kaydet

            // Relationships
            builder.HasOne(x => x.Equipment)
                .WithMany(x => x.WorkRecords)
                .HasForeignKey(x => x.EquipmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Project)
                .WithMany(x => x.WorkRecords)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.CreatedByUser)
                .WithMany()
                .HasForeignKey(x => x.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.UpdatedByUser)
                .WithMany()
                .HasForeignKey(x => x.UpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.Date); // Tarih bazlı sorgular için
            builder.HasIndex(x => x.CreatedBy); // Kullanıcı kayıtları için
        }
    }
}