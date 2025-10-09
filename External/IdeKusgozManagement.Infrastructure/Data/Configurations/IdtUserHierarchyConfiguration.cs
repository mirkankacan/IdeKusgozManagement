using IdeKusgozManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdeKusgozManagement.Infrastructure.Data.Configurations
{
    public class IdtUserHierarchyConfiguration : BaseEntityConfiguration<IdtUserHierarchy>
    {
        public override void Configure(EntityTypeBuilder<IdtUserHierarchy> builder)
        {
            base.Configure(builder);
            builder.Property(x => x.SuperiorId)
                .IsRequired();
            builder.Property(x => x.SubordinateId)
                .IsRequired();
        }
    }
}