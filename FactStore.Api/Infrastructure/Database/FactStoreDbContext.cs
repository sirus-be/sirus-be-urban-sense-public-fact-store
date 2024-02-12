using FactStore.Api.Infrastructure.Database.ModelConfiguration;
using FactStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FactStore.Api.Infrastructure.Database
{
    public class FactStoreDbContext : DbContext
    {
        public const string ConnectionStringName = "FactStoreDatabase";

        public DbSet<FactEntity> Facts { get; set; }
        public DbSet<FactTypeEntity> FactTypes { get; set; }
        public DbSet<RoleEntity> Roles { get; set; }
        public DbSet<ExternalFactConfigEntity> ExternalFactConfigs { get; set; }

        public FactStoreDbContext(DbContextOptions<FactStoreDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new FactEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new FactTypeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new RoleEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ExternalFactConfigEntityTypeConfiguration());
        }
    }
}
