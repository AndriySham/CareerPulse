using CareerPulse.Domain.Exceptions;

namespace CareerPulse.Domain.Entities;

public sealed class WorkExperience
{
    public Guid Id { get; private set; }
    public Guid ResumeRevisionId { get; private set; }
    public string CompanyName { get; private set; } = string.Empty;
    public string PositionTitle { get; private set; } = string.Empty;
    public int StartMonth { get; private set; }
    public int StartYear { get; private set; }
    public int? EndMonth { get; private set; }
    public int? EndYear { get; private set; }
    public bool IsCurrentJob { get; private set; }
    public string? Description { get; private set; }
    public string? Achievements { get; private set; }
    public string? TechStack { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private WorkExperience() { }

    public static WorkExperience Create(
        Guid resumeRevisionId,
        string companyName,
        string positionTitle,
        int startMonth,
        int startYear,
        int? endMonth = null,
        int? endYear = null,
        bool isCurrentJob = false,
        string? description = null,
        string? achievements = null,
        string? techStack = null)
    {
        if (string.IsNullOrWhiteSpace(companyName))
            throw new DomainException("CompanyName is required.");

        if (string.IsNullOrWhiteSpace(positionTitle))
            throw new DomainException("PositionTitle is required.");

        if (startMonth < 1 || startMonth > 12)
            throw new DomainException("StartMonth must be between 1 and 12.");

        return new WorkExperience
        {
            Id = Guid.NewGuid(),
            ResumeRevisionId = resumeRevisionId,
            CompanyName = companyName.Trim(),
            PositionTitle = positionTitle.Trim(),
            StartMonth = startMonth,
            StartYear = startYear,
            EndMonth = endMonth,
            EndYear = endYear,
            IsCurrentJob = isCurrentJob,
            Description = description,
            Achievements = achievements,
            TechStack = techStack,
            CreatedAt = DateTime.UtcNow
        };
    }

    internal WorkExperience DeepCopy(Guid newRevisionId)
    {
        return new WorkExperience
        {
            Id = Guid.NewGuid(),
            ResumeRevisionId = newRevisionId,
            CompanyName = CompanyName,
            PositionTitle = PositionTitle,
            StartMonth = StartMonth,
            StartYear = StartYear,
            EndMonth = EndMonth,
            EndYear = EndYear,
            IsCurrentJob = IsCurrentJob,
            Description = Description,
            Achievements = Achievements,
            TechStack = TechStack,
            CreatedAt = DateTime.UtcNow
        };
    }
}
