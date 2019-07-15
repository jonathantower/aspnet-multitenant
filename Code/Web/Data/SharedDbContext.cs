using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using MultiTenant.Web.Options;

namespace JwtAuthenticationHelper.Data
{
    public partial class SharedDbContext : DbContext
    {
        private readonly DatabaseOptions _options;

        public SharedDbContext(IOptions<DatabaseOptions> options)
        {
            this._options = options.Value;
        }

        public virtual DbSet<Tenant> Tenants { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<Tenant>(entity =>
            {
                entity.HasKey(e => e.TenantId);

                entity.Property(e => e.TenantId).HasColumnName("TenantID");

                entity.Property(e => e.Database)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.DatabaseServer)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.TenantName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.TenantId).HasColumnName("TenantID");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Tenant)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.TenantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Users_Users");
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_options.AuthConnectionString);
        }
    }
}
