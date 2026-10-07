using CareerPulse.Domain.Enums;
using CareerPulse.Domain.Exceptions;
using CareerPulse.Domain.Models;

namespace CareerPulse.Domain.Entities;

/// <summary>
/// A job opportunity associated with a Company.
/// Child entity of the Company aggregate.
/// </summary>
public sealed class Vacancy
{
    private readonly List<VacancyLanguageRequirement> _languageRequirements = new();

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
    public IReadOnlyCollection<VacancyLanguageRequirement> LanguageRequirements 
        => _languageRequirements.AsReadOnly();
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

        ValidateCore(title, salaryMin, salaryMax);

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
        ValidateCore(title, salaryMin, salaryMax);

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

    public void AddLanguageRequirement(
        string languageName,
        VacancyLanguageProficiency? proficiency,
        string? proficiencyDescription)
    {
        var requirement = VacancyLanguageRequirement.Create(
            Id,
            languageName,
            proficiency,
            proficiencyDescription);

        _languageRequirements.Add(requirement);
    }

    public void ReplaceLanguageRequirements(IEnumerable<LanguageRequirementInput> requirements)
    {
        var incoming = requirements
            .Select(x => new
            {
                LanguageName = x.LanguageName?.Trim() ?? string.Empty,
                x.Proficiency,
                ProficiencyDescription = x.ProficiencyDescription?.Trim()
            }).ToList();

        if (incoming.Any(x => string.IsNullOrWhiteSpace(x.LanguageName)))
            throw new DomainException("LanguageName is required.");

        if (incoming.GroupBy(x => x.LanguageName, StringComparer.OrdinalIgnoreCase)
            .Any(g => g.Count() > 1))
        {
            throw new DomainException("Duplicate language requirements are not allowed.");
        }

        _languageRequirements.Clear();

        foreach (var item in incoming)
        {
            AddLanguageRequirement(
                item.LanguageName,
                item.Proficiency,
                item.ProficiencyDescription);
        }

        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateCore(string title, int? salaryMin, int? salaryMax)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Vacancy title is required.");

        if (salaryMin.HasValue && salaryMax.HasValue && salaryMin > salaryMax)
            throw new DomainException("Minimum salary cannot be greater than maximum salary.");
    }
}
