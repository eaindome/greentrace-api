using GreenTrace.Server.Common.Data;
using GreenTrace.Server.Features.Roles;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Common.Data;

partial class AppDbContext
{
    // Roles DbSet is provided by IdentityDbContext — no need to re-declare

    private void ConfigureRoles(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(r => r.permissions)
                .HasColumnType("text[]");

            entity.Property(r => r.description)
                .HasMaxLength(512);
        });
    }
}
