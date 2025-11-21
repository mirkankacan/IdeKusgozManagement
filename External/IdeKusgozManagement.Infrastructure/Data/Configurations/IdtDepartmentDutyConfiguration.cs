using IdeKusgozManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdeKusgozManagement.Infrastructure.Data.Configurations
{
    public class IdtDepartmentDutyConfiguration : BaseEntityConfiguration<IdtDepartmentDuty>
    {
        public override void Configure(EntityTypeBuilder<IdtDepartmentDuty> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(x => x.DepartmentId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(x => x.Scope)
             .IsRequired()
             .HasConversion<int>();

            builder.HasMany(x => x.Users)
            .WithOne(x => x.DepartmentDuty)
            .HasForeignKey(x => x.DepartmentDutyId)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}