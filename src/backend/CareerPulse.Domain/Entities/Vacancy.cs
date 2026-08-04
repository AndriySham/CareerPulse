using CareerPulse.Domain.Exceptions;

namespace CareerPulse.Domain.Entities;

/// <summary>
/// A job opportunity associated with a Company.
/// Child entity of the Company aggregate.
/// </summary>
public sealed class Vacancy
{
    public Guid Id { get; private set; }
    public Guid CompanyId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? Url { get; private set; }
    public DateTime? PostedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Navigation
    public Company Company { get; private set; } = null!;

    private Vacancy() { }

    public static Vacancy Create(Guid companyId, string title, string? url = null, DateTime? postedAt = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Vacancy title is required.");

        return new Vacancy
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            Title = title.Trim(),
            Url = url?.Trim(),
            PostedAt = postedAt,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(string title, string? description = null, string? url = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Vacancy title is required.");

        Title = title.Trim();
        Description = description;
        Url = url?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }
}
