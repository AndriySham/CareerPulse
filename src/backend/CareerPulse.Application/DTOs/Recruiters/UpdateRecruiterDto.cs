namespace CareerPulse.Application.DTOs.Recruiters;

/// <summary>
/// Input DTO for updating a Recruiter.
/// </summary>
public sealed class UpdateRecruiterDto
{
    public string Name { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string? Email { get; init; }
    public string? LinkedInUrl { get; init; }
    public string? TelegramUrl { get; init; }
    public string? Notes { get; init; }
}
