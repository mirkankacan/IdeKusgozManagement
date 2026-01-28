using IdeKusgozManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdeKusgozManagement.Infrastructure.Data.Configurations
{
    public class IdtTrafficTicketConfiguration : BaseEntityConfiguration<IdtTrafficTicket>
    {
        public override void Configure(EntityTypeBuilder<IdtTrafficTicket> builder)
        {
            base.Configure(builder); // BaseEntity konfigürasyonlarını uygula

            builder.Property(x => x.ProjectId)
                .HasMaxLength(450)
                .IsRequired();
            builder.Property(x => x.EquipmentId)
                 .HasMaxLength(450)
                .IsRequired();
            builder.Property(x => x.Type)
                .IsRequired()
                .HasConversion<int>();
            builder.Property(x => x.TargetUserId)
                .IsRequired(false);
            builder.Property(x => x.FileId)
            .IsRequired(false);
            builder.Property(x => x.Amount)
               .IsRequired()
               .HasPrecision(18, 2);
            builder.Property(x => x.TicketDate)
                .IsRequired();

            builder.HasOne(x => x.Project)
          .WithMany(x => x.TrafficTickets)
          .HasForeignKey(x => x.ProjectId)
          .OnDelete(DeleteBehavior.Restrict)
          .IsRequired(true);

            builder.HasOne(x => x.Equipment)
          .WithMany(x => x.TrafficTickets)
          .HasForeignKey(x => x.EquipmentId)
          .OnDelete(DeleteBehavior.Restrict)
          .IsRequired(true);

            builder.HasOne(x => x.CreatedByUser)
             .WithMany()
             .HasForeignKey(x => x.CreatedBy)
             .OnDelete(DeleteBehavior.Restrict)
             .IsRequired(true);

            builder.HasOne(x => x.File)
            .WithMany(x => x.TrafficTickets)
            .HasForeignKey(x => x.FileId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);
            builder.HasOne(x => x.TargetUser)
             .WithMany()
             .HasForeignKey(x => x.TargetUserId)
             .OnDelete(DeleteBehavior.Restrict)
             .IsRequired(false);
        }
    }
}