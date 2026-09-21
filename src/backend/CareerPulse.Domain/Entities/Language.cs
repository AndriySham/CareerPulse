using CareerPulse.Domain.Enums;
using CareerPulse.Domain.Exceptions;

namespace CareerPulse.Domain.Entities;

public sealed class Language
{
    public Guid Id { get; private set; }
    public Guid ResumeRevisionId { get; private set; }
    public string LanguageName { get; private set; } = string.Empty;
    public LanguageProficiency Proficiency { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Language() { }

    public static Language Create(
        Guid resumeRevisionId,
        string languageName,
        LanguageProficiency proficiency)
    {
        if (string.IsNullOrWhiteSpace(languageName))
            throw new DomainException("LanguageName is required.");

        return new Language
        {
            Id = Guid.NewGuid(),
            ResumeRevisionId = resumeRevisionId,
            LanguageName = languageName.Trim(),
            Proficiency = proficiency,
            CreatedAt = DateTime.UtcNow
        };
    }

    internal Language DeepCopy(Guid newRevisionId)
    {
        return new Language
        {
            Id = Guid.NewGuid(),
            ResumeRevisionId = newRevisionId,
            LanguageName = LanguageName,
            Proficiency = Proficiency,
            CreatedAt = DateTime.UtcNow
        };
    }
}
