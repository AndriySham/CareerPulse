using CareerPulse.Domain.Enums;
using CareerPulse.Domain.Exceptions;

namespace CareerPulse.Domain.Entities;

/// <summary>
/// A job opportunity associated with a Company.
/// Child entity of the Company aggregate.
/// </summary>
public sealed class Vacancy
{
    public Guid Id { get; private set; }
    public Guid CompanyId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Location { get; private set; }
    public WorkMode? WorkMode { get; private set; }
    public EmploymentType? EmploymentType {  get; private set; }
    public int? SalaryMin { get; private set; }
    public int? SalaryMax { get; private set; }
    public SalaryCurrency? SalaryCurrency { get; private set; }
    public string? Description { get; private set; }
    public string? Responsibilities { get; private set; }
    public string? Requirements { get; private set; }
    public string? NiceToHave { get; private set; }
    public string? Benefits { get; private set; }
    public string? Url { get; private set; }
    public DateTime? PostedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Navigation
    public Company Company { get; private set; } = null!;

    private Vacancy() { }

    public static Vacancy Create(
        Guid companyId,
        string title,
        string? location = null,
        WorkMode? mode = null,
        EmploymentType? employmentType = null,
        int? salaryMin = null,
        int? salaryMax = null,
        SalaryCurrency? currency = null,
        string? description = null,
        string? responsibilities = null,
        string? requirements = null,
        string? niceToHave = null,
        string? benefits = null,
        string? url = null,
        DateTime? postedAt = null)
    {
        if (companyId == Guid.Empty)
            throw new DomainException("Company ID is required.");


        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Vacancy title is required.");

        if (salaryMin.HasValue && salaryMax.HasValue && salaryMin > salaryMax)
            throw new DomainException("Minimum salary cannot be greater than maximum salary.");

        return new Vacancy
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            Title = title.Trim(),
            Location = location?.Trim(),
            WorkMode = mode,
            EmploymentType = employmentType,
            SalaryMin = salaryMin,
            SalaryMax = salaryMax,
            SalaryCurrency = currency,
            Description = description,
            Responsibilities = responsibilities,
            Requirements = requirements,
            Benefits = benefits,
            NiceToHave = niceToHave,
            Url = url?.Trim(),
            PostedAt = postedAt,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        string title,
        string? location = null,
        WorkMode? mode = null,
        EmploymentType? employmentType = null,
        int? salaryMin = null,
        int? salaryMax = null,
        SalaryCurrency? currency = null,
        string? description = null,
        string? responsibilities = null,
        string? requirements = null,
        string? niceToHave = null,
        string? benefits = null,
        string? url = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Vacancy title is required.");

        if (salaryMin.HasValue && salaryMax.HasValue && salaryMin > salaryMax)
            throw new DomainException("Minimum salary cannot be greater than maximum salary.");

        Title = title.Trim();
        Location = location?.Trim();
        WorkMode = mode;
        EmploymentType = employmentType;
        SalaryMin = salaryMin;
        SalaryMax = salaryMax;
        SalaryCurrency = currency;
        Description = description;
        Responsibilities = responsibilities;
        Requirements = requirements;
        Benefits = benefits;
        NiceToHave = niceToHave;
        Url = url?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }
}
