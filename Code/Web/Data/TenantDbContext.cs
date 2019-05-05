using System;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using MultiTenant.Helpers;
using MultiTenant.Web.Helpers.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiTenant.Web.Data.Interfaces;
using System.Reflection;
using System.Linq;

namespace MultiTenant.Web.Data
{
    public partial class TenantDbContext : DbContext
    {
        private readonly HttpContext _http;
        private readonly ITenantHelper _tenantHelper;

        public TenantDbContext(HttpContext httpContext,
            ITenantHelper tenantHelper)
        {
            this._http = httpContext;
            this._tenantHelper = tenantHelper;
        }

        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => e.OrderDetailId);

                entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");

                entity.Property(e => e.ItemPrice).HasColumnType("money");

                entity.Property(e => e.LinePrice).HasColumnType("money");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderDetails_Orders");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderDetails_Products");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.OrderId);

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.OrderDate).HasColumnType("date");

                entity.Property(e => e.TenantId).HasColumnName("TenantID");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductId);

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.Description).IsRequired();

                entity.Property(e => e.ModelNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Price).HasColumnType("money");
            });

            // Configure entity filter
            //modelBuilder.Entity<Order>()
            //    .HasQueryFilter(o =>
            //        EF.Property<int>(o, "TenantId") == ClaimsHelper.GetClaim<int>(_http.User, "Tenant"));

            //SetGlobalQueryFilters(modelBuilder);
        }

        private void SetGlobalQueryFilters(ModelBuilder modelBuilder)
        {
            foreach (var tp in modelBuilder.Model.GetEntityTypes())
            {
                var t = tp.ClrType;

                if (typeof(ITenant).IsAssignableFrom(t))
                {
                    var method = SetGlobalQueryForSoftDeleteAndTenantMethodInfo.MakeGenericMethod(t);
                    method.Invoke(this, new object[] { modelBuilder });
                }

            }
        }

        private static readonly MethodInfo SetGlobalQueryForSoftDeleteAndTenantMethodInfo = 
            typeof(TenantDbContext)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Single(t => t.IsGenericMethod && t.Name == "SetGlobalQueryForTenant");

        public void SetGlobalQueryForTenant<T>(ModelBuilder builder) where T : class, ITenant
        {
            builder.Entity<T>()
               .HasQueryFilter(o =>
                   EF.Property<int>(o, "TenantId") == ClaimsHelper.GetClaim<int>(_http.User, "Tenant"));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var tenantId = ClaimsHelper.GetClaim<int>(_http.User, "Tenant");
            var t = _tenantHelper.GetTenant(tenantId);
            var constr = $"server={t.DatabaseServer};database={t.Database};integrated security=true;";

            optionsBuilder.UseSqlServer(constr);
        }
    }
}
