namespace CareerPulse.Domain.ValueObjects;

/// <summary>
/// Value Object: Developer contact details embedded in a ResumeRevision.
/// Immutable by design — structural equality over identity.
/// DOMAIN_MODEL.md §4: PersonalInfo is a C# Value Object.
/// </summary>
public sealed record PersonalInfo
{
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string? LinkedIn { get; init; }
    public string? GitHub { get; init; }
    public string? Location { get; init; }

    private PersonalInfo() { }

    public static PersonalInfo Create(
        string fullName,
        string email,
        string? phone = null,
        string? linkedIn = null,
        string? gitHub = null,
        string? location = null)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("FullName is required.", nameof(fullName));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));

        return new PersonalInfo
        {
            FullName = fullName.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            Phone = phone?.Trim(),
            LinkedIn = linkedIn?.Trim(),
            GitHub = gitHub?.Trim(),
            Location = location?.Trim()
        };
    }
}
