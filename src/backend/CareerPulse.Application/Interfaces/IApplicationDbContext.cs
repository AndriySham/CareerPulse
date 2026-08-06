using CareerPulse.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Interfaces;

/// <summary>
/// Abstraction for DbContext in the Application layer.
/// Allows Application use cases to interact with EF Core DbSets cleanly.
/// </summary>
public interface IApplicationDbContext
{
    DbSet<Domain.Entities.Application> Applications { get; }
    DbSet<ResumeRevision> ResumeRevisions { get; }
    DbSet<ResumeRevisionSkill> ResumeRevisionSkills { get; }
    DbSet<Company> Companies { get; }
    DbSet<Vacancy> Vacancies { get; }
    DbSet<Interview> Interviews { get; }
    DbSet<MasterSkill> MasterSkills { get; }
    DbSet<MasterSkillAlias> MasterSkillAliases { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
