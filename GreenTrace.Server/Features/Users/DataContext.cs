using GreenTrace.Server.Common.Data;
using GreenTrace.Server.Features.Users;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Common.Data;

partial class AppDbContext
{
    // Users DbSet is provided by IdentityDbContext — no need to re-declare

    private void ConfigureUsers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.fullName).IsRequired().HasMaxLength(256);
            entity.Property(u => u.organizationId).IsRequired();

            entity.HasOne(u => u.organization)
                .WithMany()
                .HasForeignKey(u => u.organizationId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
