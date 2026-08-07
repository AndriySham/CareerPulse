namespace CareerPulse.Application.DTOs.Companies;

/// <summary>
/// Input DTO for updating an existing Company.
/// </summary>
public sealed class UpdateCompanyDto
{
    public string Name { get; init; } = string.Empty;
    public string? Website { get; init; }
    public string? Industry { get; init; }
    public string? Notes { get; init; }
}
