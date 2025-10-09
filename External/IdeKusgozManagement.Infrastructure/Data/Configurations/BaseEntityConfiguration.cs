using IdeKusgozManagement.Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdeKusgozManagement.Infrastructure.Data.Configurations
{
    public abstract class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CreatedDate)
                .IsRequired();
            builder.Property(x => x.CreatedBy)
           .IsRequired();
            builder.Property(x => x.UpdatedDate);

            builder.Property(x => x.UpdatedBy);
        }
    }
}