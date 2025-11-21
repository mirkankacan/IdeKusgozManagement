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
            builder.Property(x => x.TargetDepartmentId)
           .IsRequired(false)
           .HasMaxLength(450)
           .HasDefaultValue(null);
            builder.Property(x => x.TargetDepartmentDutyId)
           .IsRequired(false)
           .HasMaxLength(450)
           .HasDefaultValue(null);
            builder.Property(x => x.TargetCompanyId)
         .IsRequired(false)
         .HasMaxLength(450)
         .HasDefaultValue(null);
            builder.Property(x => x.TargetEquipmentId)
           .IsRequired(false)
           .HasMaxLength(450)
           .HasDefaultValue(null);
            builder.Property(x => x.TargetUserId)
             .IsRequired(false)
             .HasMaxLength(450)
             .HasDefaultValue(null);
            builder.Property(x => x.TargetProjectId)
                .IsRequired(false)
                .HasMaxLength(450)
                .HasDefaultValue(null);
            builder.Property(x => x.StartDate)
                .IsRequired(false)
                .HasDefaultValue(null);
            builder.Property(x => x.EndDate)
                .IsRequired(false)
                .HasDefaultValue(null);

            builder.HasOne(x => x.DocumentType)
            .WithMany(x => x.Files)
            .HasForeignKey(x => x.DocumentTypeId)
            .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.TargetDepartment)
          .WithMany(x => x.Files)
          .HasForeignKey(x => x.TargetDepartmentId)
          .OnDelete(DeleteBehavior.Restrict)
          .IsRequired(false);
            builder.HasOne(x => x.TargetDepartmentDuty)
            .WithMany(x => x.Files)
            .HasForeignKey(x => x.TargetDepartmentDutyId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
            builder.HasOne(x => x.TargetCompany)
        .WithMany(x => x.Files)
        .HasForeignKey(x => x.TargetCompanyId)
        .OnDelete(DeleteBehavior.Restrict)
        .IsRequired(false);
            builder.HasOne(x => x.TargetEquipment)
          .WithMany(x => x.Files)
          .HasForeignKey(x => x.TargetEquipmentId)
          .OnDelete(DeleteBehavior.Restrict)
          .IsRequired(false);
            builder.HasOne(x => x.TargetUser)
             .WithMany()
             .HasForeignKey(x => x.TargetUserId)
             .OnDelete(DeleteBehavior.Restrict)
             .IsRequired(false);
            builder.HasOne(x => x.TargetProject)
             .WithMany(x => x.Files)
             .HasForeignKey(x => x.TargetProjectId)
             .OnDelete(DeleteBehavior.Restrict)
             .IsRequired(false);
        }
    }
}