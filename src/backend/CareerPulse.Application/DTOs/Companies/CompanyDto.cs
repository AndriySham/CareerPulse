namespace CareerPulse.Application.DTOs.Companies;

/// <summary>
/// DTO representing a Company entity.
/// </summary>
public sealed class CompanyDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Website { get; init; }
    public string? Industry { get; init; }
    public string? Notes { get; init; }
    public bool IsArchived { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
