using FactStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FactStore.Api.Infrastructure.Database.ModelConfiguration
{
    public class FactEntityTypeConfiguration : IEntityTypeConfiguration<FactEntity>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<FactEntity> builder)
        {
            builder.ToTable("Facts");
            //builder.AsDomainEntity<Company, Guid>();
            //builder.AsFullAudited<Company, Guid, Guid>();

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Key)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(p => p.Value)
                .HasMaxLength(2000)
                .IsRequired();

            builder.HasOne(fk => fk.FactType)
                .WithMany(fk => fk.Facts)
                .HasForeignKey(fk => fk.FactTypeId);

            builder.HasIndex(idx => idx.Key)
                .IsUnique(false)
                .HasDatabaseName("IDX_Fact_Key");
        }
    }
}
