using CareerPulse.Application.DTOs.Recruiters;

namespace CareerPulse.Application.DTOs.Companies;

public sealed class CompanyDetailsDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Website { get; init; }
    public string? Industry { get; init; }
    public string? Notes { get; init; }
    public bool IsArchived { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public IReadOnlyCollection<RecruiterDto> Recruiters { get; init; } = Array.Empty<RecruiterDto>();
}
