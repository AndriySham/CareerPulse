using CareerPulse.Domain.Exceptions;

namespace CareerPulse.Domain.Entities;

public sealed class Education
{
    public Guid Id { get; private set; }
    public Guid ResumeRevisionId { get; private set; }
    public string InstitutionName { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public int? StartYear { get; private set; }
    public int? EndYear { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Education() { }

    public static Education Create(
        Guid resumeRevisionId,
        string institutionName,
        string? description = null,
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
            Description = description,
            StartYear = startYear,
            EndYear = endYear,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        string institutionName,
        string? description = null,
        int? startYear = null,
        int? endYear = null)
    {
        if (string.IsNullOrWhiteSpace(institutionName))
            throw new DomainException("InstitutionName is required.");

        InstitutionName = institutionName.Trim();
        Description = description?.Trim();
        StartYear = startYear;
        EndYear = endYear;
    }

    internal Education DeepCopy(Guid newRevisionId)
    {
        return new Education
        {
            Id = Guid.NewGuid(),
            ResumeRevisionId = newRevisionId,
            InstitutionName = InstitutionName,
            Description = Description,
            StartYear = StartYear,
            EndYear = EndYear,
            CreatedAt = DateTime.UtcNow
        };
    }
}
