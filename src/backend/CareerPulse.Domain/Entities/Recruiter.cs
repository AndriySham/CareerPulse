namespace CareerPulse.Domain.Entities;

/// <summary>
/// Represents a single Recruiter round associated with an Company.
/// Child entity of the Company aggregate.
/// </summary>
public sealed class Recruiter
{
    public Guid Id { get; private set; }
    public Guid CompanyId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public string? LinkedInUrl { get; private set; }
    public string? TelegramUrl { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public Company Company { get; private set; } = null!;

    private Recruiter() { }
}

