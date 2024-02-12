using FactStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FactStore.Api.Infrastructure.Database.ModelConfiguration
{
    public class ExternalFactConfigEntityTypeConfiguration : IEntityTypeConfiguration<ExternalFactConfigEntity>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<ExternalFactConfigEntity> builder)
        {
            builder.ToTable("ExternalFactConfigs");

            builder.HasKey(p => p.Id);

            builder.HasOne(fk => fk.Fact)
                .WithMany(fk => fk.ExternalFactConfigs)
                .HasForeignKey(fk => fk.FactId);

            builder.Property(p => p.Url)
                .HasMaxLength(2000)
                .IsRequired();
        }
    }
}
