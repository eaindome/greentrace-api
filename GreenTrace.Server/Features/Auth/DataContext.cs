using GreenTrace.Server.Common.Data;
using GreenTrace.Server.Features.Auth;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Common.Data;

partial class AppDbContext
{
    public DbSet<RefreshToken> refreshTokens { get; set; }

    private void ConfigureAuth(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(rt => rt.id);
            entity.HasIndex(rt => rt.token).IsUnique();
            entity.HasIndex(rt => rt.userId);
            entity.Property(rt => rt.token).IsRequired().HasMaxLength(512);
        });
    }
}
