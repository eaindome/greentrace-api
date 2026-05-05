using GreenTrace.Server.Common.Data;
using GreenTrace.Server.Features.Products;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Common.Data;

partial class AppDbContext
{
    public DbSet<Product> products { get; set; }

    private void ConfigureProducts(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.id);

            entity.HasIndex(p => p.organizationId);
            entity.HasIndex(p => new { p.organizationId, p.sku })
                .IsUnique()
                .HasFilter("\"sku\" IS NOT NULL");

            entity.Property(p => p.name).IsRequired().HasMaxLength(256);
            entity.Property(p => p.sku).HasMaxLength(64);
            entity.Property(p => p.unitOfMeasure).HasMaxLength(32);
            entity.Property(p => p.description).HasMaxLength(1024);

            entity.HasOne(p => p.organization)
                .WithMany()
                .HasForeignKey(p => p.organizationId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(p => p.domain)
                .WithMany()
                .HasForeignKey(p => p.domainId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
