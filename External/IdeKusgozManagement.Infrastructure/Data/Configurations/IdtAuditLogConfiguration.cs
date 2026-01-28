using IdeKusgozManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdeKusgozManagement.Infrastructure.Data.Configurations
{
    public class IdtAuditLogConfiguration : IEntityTypeConfiguration<IdtAuditLog>
    {
        public void Configure(EntityTypeBuilder<IdtAuditLog> builder)
        {
            builder.HasKey(keyExpression: x => x.Id);

            builder.Property(x => x.TableName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Operation)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(x => x.OldValue)
                  .IsRequired(false)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.NewValue)
                                  .IsRequired(false)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.CreatedDate)
                .IsRequired();

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(450);
        }
    }
}