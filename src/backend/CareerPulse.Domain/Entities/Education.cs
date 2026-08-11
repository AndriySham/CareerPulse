using CareerPulse.Domain.Exceptions;

namespace CareerPulse.Domain.Entities;

public sealed class Education
{
    public Guid Id { get; private set; }
    public Guid ResumeRevisionId { get; private set; }
    public string InstitutionName { get; private set; } = string.Empty;
    public string? Degree { get; private set; }
    public string? FieldOfStudy { get; private set; }
    public int? StartYear { get; private set; }
    public int? EndYear { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Education() { }

    public static Education Create(
        Guid resumeRevisionId,
        string institutionName,
        string? degree = null,
        string? fieldOfStudy = null,
        int? startYear = null,
        int? endYear = null)
    {
        if (string.IsNullOrWhiteSpace(institutionName))
            throw new DomainException("InstitutionName is required.");

        return new Education
        {
            Id = Guid.NewGuid(),
            ResumeRevisionId = resumeRevisionId,
            InstitutionName = institutionName.Trim(),
            Degree = degree,
            FieldOfStudy = fieldOfStudy,
            StartYear = startYear,
            EndYear = endYear,
            CreatedAt = DateTime.UtcNow
        };
    }

    internal Education DeepCopy(Guid newRevisionId)
    {
        return new Education
        {
            Id = Guid.NewGuid(),
            ResumeRevisionId = newRevisionId,
            InstitutionName = InstitutionName,
            Degree = Degree,
            FieldOfStudy = FieldOfStudy,
            StartYear = StartYear,
            EndYear = EndYear,
            CreatedAt = DateTime.UtcNow
        };
    }
}
