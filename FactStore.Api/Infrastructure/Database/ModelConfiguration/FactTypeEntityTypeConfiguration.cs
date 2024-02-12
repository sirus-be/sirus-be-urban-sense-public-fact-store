using FactStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FactStore.Api.Infrastructure.Database.ModelConfiguration
{
    public class FactTypeEntityTypeConfiguration : IEntityTypeConfiguration<FactTypeEntity>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<FactTypeEntity> builder)
        {
            builder.ToTable("FactTypes");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.HasIndex(idx => idx.Name)
                .IsUnique(false)
                .HasDatabaseName("IDX_FactType_Name");

            builder.HasMany(b => b.Roles)
               .WithMany(c => c.FactTypes)
               .UsingEntity(j => j.ToTable("FactTypeRoles"));
        }
    }
}
