using CareerPulse.Application.Interfaces;
using CareerPulse.Infrastructure.Persistence;
using CareerPulse.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CareerPulse.Infrastructure.Extensions;

/// <summary>
/// Registers all Infrastructure layer services into the DI container.
/// Called from CareerPulse.Api Program.cs (Composition Root).
/// </summary>
public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ADR 001: PostgreSQL via Npgsql EF Core
        services.AddDbContext<CareerPulseDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsAssembly(typeof(CareerPulseDbContext).Assembly.FullName)));

        // ADR 003: Storage abstraction — switch provider via appsettings.json
        var storageProvider = configuration["Storage:Provider"] ?? "Local";
        if (storageProvider == "Local")
            services.AddScoped<IFileStorage, LocalFileStorage>();
        // Phase 2: else services.AddScoped<IFileStorage, GoogleDriveStorage>();

        return services;
    }
}
