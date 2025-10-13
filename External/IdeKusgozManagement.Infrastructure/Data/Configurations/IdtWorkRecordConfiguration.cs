﻿using IdeKusgozManagement.Domain.Entities;
using IdeKusgozManagement.Domain.Enums;
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
                .HasMaxLength(100)
                .IsRequired(false); // Nullable

            builder.Property(x => x.StartTime)
                .IsRequired(false); // Nullable

            builder.Property(x => x.EndTime)
                .IsRequired(false); // Nullable

            builder.Property(x => x.AdditionalStartTime)
                .IsRequired(false); // Nullable

            builder.Property(x => x.AdditionalEndTime)
                .IsRequired(false); // Nullable

            builder.Property(x => x.ProjectId)
                .HasMaxLength(450)
                .IsRequired(false); // Nullable

            builder.Property(x => x.EquipmentId)
                .HasMaxLength(450)
                .IsRequired(false); // Nullable

            builder.Property(x => x.Province)
                .HasMaxLength(100)
                .IsRequired(false); // Nullable

            builder.Property(x => x.District)
                .HasMaxLength(100)
                .IsRequired(false); // Nullable

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
                .HasDefaultValue(WorkRecordStatus.Pending)
                .HasConversion<int>(); // Enum'u int olarak kaydet

            // Relationships - Nullable foreign keys
            builder.HasOne(x => x.Equipment)
                .WithMany(x => x.WorkRecords)
                .HasForeignKey(x => x.EquipmentId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false); // Nullable relationship

            builder.HasOne(x => x.Project)
                .WithMany(x => x.WorkRecords)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false); // Nullable relationship

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