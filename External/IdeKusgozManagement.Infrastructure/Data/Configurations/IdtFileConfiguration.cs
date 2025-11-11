using IdeKusgozManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdeKusgozManagement.Infrastructure.Data.Configurations
{
    public class IdtFileConfiguration : BaseEntityConfiguration<IdtFile>
    {
        public override void Configure(EntityTypeBuilder<IdtFile> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(450);
            builder.Property(x => x.Path)
                .IsRequired()
                .HasMaxLength(500);
            builder.Property(x => x.OriginalName)
                .IsRequired()
                .HasMaxLength(450);
            builder.Property(x => x.DocumentTypeId)
                .IsRequired()
                .HasMaxLength(450);
            builder.Property(x => x.TargetUserId)
             .IsRequired(false)
             .HasMaxLength(450)
             .HasDefaultValue(null);
            builder.Property(x => x.TargetProjectId)
          .IsRequired(false)
          .HasMaxLength(450)
          .HasDefaultValue(null);
            builder.Property(x => x.TargetEquipmentId)
          .IsRequired(false)
          .HasMaxLength(450)
          .HasDefaultValue(null);
            builder.Property(x => x.DepartmentId)
                .IsRequired(false)
                .HasMaxLength(450)
                .HasDefaultValue(null);
            builder.Property(x => x.StartDate)
                .IsRequired(false)
                .HasDefaultValue(null);
            builder.Property(x => x.EndDate)
                .IsRequired(false)
                .HasDefaultValue(null);

            builder.HasOne(x => x.TargetUser)
             .WithMany()
             .HasForeignKey(x => x.TargetUserId)
             .OnDelete(DeleteBehavior.Restrict)
             .IsRequired(false);

            builder.HasOne(x => x.TargetProject)
        .WithMany()
        .HasForeignKey(x => x.TargetProjectId)
        .OnDelete(DeleteBehavior.Restrict)
        .IsRequired(false);

            builder.HasOne(x => x.TargetEquipment)
           .WithMany()
           .HasForeignKey(x => x.TargetEquipmentId)
           .OnDelete(DeleteBehavior.Restrict)
           .IsRequired(false);

            builder.HasOne(x => x.TargetUser)
        .WithMany()
        .HasForeignKey(x => x.TargetUserId)
        .OnDelete(DeleteBehavior.Restrict)
        .IsRequired(false);

            builder.HasOne(x => x.Department)
                .WithMany(x => x.Files)
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            builder.HasOne(x => x.DocumentType)
                .WithMany(x => x.Files)
                .HasForeignKey(x => x.DocumentTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}