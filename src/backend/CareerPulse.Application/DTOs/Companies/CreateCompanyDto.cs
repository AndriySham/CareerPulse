namespace CareerPulse.Application.DTOs.Companies;

/// <summary>
/// Input DTO for creating a new Company.
/// </summary>
public sealed class CreateCompanyDto
{
    public string Name { get; init; } = string.Empty;
    public string? Website { get; init; }
    public string? Industry { get; init; }
}
