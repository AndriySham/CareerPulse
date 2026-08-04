using CareerPulse.Domain.Exceptions;
using CareerPulse.Domain.ValueObjects;

namespace CareerPulse.Domain.Entities;

/// <summary>
/// Aggregate Root: Employer organization.
/// Owns Vacancies. Never hard-deleted — preserves application history.
/// </summary>
public sealed class Company
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Website { get; private set; }
    public string? Industry { get; private set; }
    public string? Notes { get; private set; }
    public bool IsArchived { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<Vacancy> _vacancies = new();
    public IReadOnlyCollection<Vacancy> Vacancies => _vacancies.AsReadOnly();

    private Company() { }

    public static Company Create(string name, string? website = null, string? industry = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Company name is required.");

        return new Company
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Website = website?.Trim(),
            Industry = industry?.Trim(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, string? website = null, string? industry = null, string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Company name is required.");

        Name = name.Trim();
        Website = website?.Trim();
        Industry = industry?.Trim();
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Archive() => IsArchived = true;
}
