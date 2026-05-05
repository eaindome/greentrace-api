using System.Text;
using GreenTrace.Server.Common.Auth;
using GreenTrace.Server.Common.Data;
using GreenTrace.Server.Features.Roles;
using GreenTrace.Server.Features.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace GreenTrace.Server.Common.Extensions;

/// <summary>
/// DI registration helpers for the application.
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Registers the database context with PostgreSQL connection.
    /// Dual registration: pooled factory for HotChocolate, scoped for Identity's UserManager/RoleManager.
    /// </summary>
    public static IServiceCollection AddData(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("DefaultConnection");

        // Pooled factory for HotChocolate resolver injection (timestamps only — no audit log writes)
        services.AddPooledDbContextFactory<AppDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
            options.AddInterceptors(new AuditInterceptor());
        });

        // Scoped DbContext for Identity's UserManager/RoleManager (full audit logging with user context).
        // Registered via AddScoped rather than AddDbContext to avoid EF registering
        // IDbContextOptionsConfiguration<AppDbContext> as scoped, which conflicts with the
        // singleton pooled factory above and causes a root-provider resolution error at startup.
        services.AddScoped(sp =>
        {
            var http = sp.GetRequiredService<IHttpContextAccessor>();
            var userService = sp.GetRequiredService<UserService>();
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(connectionString)
                .AddInterceptors(new AuditInterceptor(http, userService))
                .Options;
            return new AppDbContext(options);
        });

        return services;
    }

    /// <summary>
    /// Registers ASP.NET Identity (no cookie auth — JWT only).
    /// </summary>
    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services.AddIdentityCore<User>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<Role>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }

    /// <summary>
    /// Configures JWT authentication from appsettings.json "Jwt" section.
    /// </summary>
    public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration config)
    {
        var jwtSettings = config.GetSection("Jwt").Get<JwtSettings>()
            ?? throw new InvalidOperationException("JWT settings not configured. Add a 'Jwt' section to appsettings.json.");

        services.AddSingleton(jwtSettings);
        services.AddSingleton<TokenService>();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

        services.AddAuthorization();

        return services;
    }

    /// <summary>
    /// Registers the AuthContext and UserService.
    /// </summary>
    public static IServiceCollection AddAuthContext(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddSingleton<UserService>();
        services.AddHostedService(sp => sp.GetRequiredService<UserService>());
        services.AddTransient<AuthContext>();
        return services;
    }
}
