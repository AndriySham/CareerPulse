using CareerPulse.Domain.Exceptions;

namespace CareerPulse.Domain.Entities;

/// <summary>
/// A recruiter at a Company. Referenced by Applications via RecruiterId.
/// Has its own lifecycle and is managed independently of Company.
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

    public static Recruiter Create(
        Guid companyId,
        string name,
        string? phone = null,
        string? email = null,
        string? linkedInUrl = null,
        string? telegramUrl = null,
        string? notes = null)
    {
        if (companyId == Guid.Empty)
            throw new DomainException("Company ID is requered.");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Recruiter name is required.");

        return new Recruiter
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            Name = name,
            Phone = NormalizeText(phone),
            Email = NormalizeText(email),
            LinkedInUrl = NormalizeText(linkedInUrl),
            TelegramUrl = NormalizeText(telegramUrl),
            Notes = NormalizeText(notes),
            CreatedAt = DateTime.UtcNow
        };
    }

    private static string? NormalizeText(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

