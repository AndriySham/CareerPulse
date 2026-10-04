namespace CareerPulse.Application.DTOs.Recruiters;

/// <summary>
/// DTO representing a Recruiter entity.
/// </summary>
public sealed class RecruiterDto
{
    public Guid Id { get; init; }
    public Guid CompanyId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string? Email { get; init; }
    public string? LinkedInUrl { get; init; }
    public string? TelegramUrl { get; init; }
    public string? Notes { get; init; }
}
