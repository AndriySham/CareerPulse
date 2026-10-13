using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Enums;
using CareerPulse.Domain.ValueObjects;
using CareerPulse.Infrastructure.Persistence;
using AppEntity = CareerPulse.Domain.Entities.Application;

namespace CareerPulse.Application.Tests.TestHelpers;

public static class TestDataSeeder
{
    /// <summary>
    /// Creates and saves a Company, Vacancy, Resume with a revision and an Application linking them.
    /// </summary>
    public static async Task<AppEntity> SeedApplicationAsync(CareerPulseDbContext context)
    {
        var company = Company.Create("QueryCorp");
        var vacancy = Vacancy.Create(company.Id, "Backend Engineer");
        var personalInfo = PersonalInfo.Create("Bob Martin", "bob@example.com");
        var resume = Resume.Create("Bob`s Resume", ResumeTrack.Backend, CareerLevel.Senior, "Clean Coder");
        var revision = resume.CreateFirstRevision("Clean Coder", personalInfo);
        var app = AppEntity.Create(company.Id, revision.Id, "GitHub Jobs", vacancy.Id);

        context.Companies.Add(company);
        context.Vacancies.Add(vacancy);
        context.Resumes.Add(resume);
        context.ResumeRevisions.Add(revision);
        context.Applications.Add(app);
        await context.SaveChangesAsync();

        return app;
    }
}
