using CareerPulse.Domain.Entities;
using CareerPulse.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using CareerPulse.Application.Interfaces;

namespace CareerPulse.Infrastructure.Persistence;

/// <summary>
/// EF Core DbContext for CareerPulse.
/// ADR 001: PostgreSQL via Npgsql.
/// ADR 002: PostgreSQL is the sole SSOT — no JSON files on disk.
/// Configurations are in Configurations/ folder (IEntityTypeConfiguration pattern).
/// </summary>
public sealed class CareerPulseDbContext : DbContext, IApplicationDbContext
{
    public CareerPulseDbContext(DbContextOptions<CareerPulseDbContext> options) : base(options) { }

    public DbSet<Domain.Entities.Application> Applications => Set<Domain.Entities.Application>();
    public DbSet<Resume> Resumes => Set<Resume>();
    public DbSet<ResumeRevision> ResumeRevisions => Set<ResumeRevision>();
    public DbSet<ResumeRevisionSkill> ResumeRevisionSkills => Set<ResumeRevisionSkill>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Vacancy> Vacancies => Set<Vacancy>();
    public DbSet<Interview> Interviews => Set<Interview>();
    public DbSet<MasterSkill> MasterSkills => Set<MasterSkill>();
    public DbSet<MasterSkillAlias> MasterSkillAliases => Set<MasterSkillAlias>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all IEntityTypeConfiguration<T> from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CareerPulseDbContext).Assembly);
    }
}
