using CareerPulse.Application.Features.Languages.Queries.GetLanguages;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Enums;
using CareerPulse.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace CareerPulse.Application.Tests.Features.Languages
{
    public class GetLanguagesQueryTests
    {
        [Fact]
        public async Task Handle_GetLanguagesForResumeRevisionId_ShouldReturnAllLanguages()
        {
            // Arrange
            using var context = TestDbContext.CreateInMemory();

            var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
            var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
            var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);
            var language1 = Language.Create(revision.Id, "English", LanguageProficiency.B1);
            var language2 = Language.Create(revision.Id, "Ukranian", LanguageProficiency.C2);

            context.Resumes.Add(resume);
            context.ResumeRevisions.Add(revision);
            context.Languages.Add(language1);
            context.Languages.Add(language2);
            await context.SaveChangesAsync(CancellationToken.None);

            var query = new GetLanguagesQuery(revision.Id);
            var handler = new GetLanguagesQueryHandler(context);

            // Act 
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result[0].LanguageName.Should().Be("English");
            result[0].Proficiency.Should().Be(LanguageProficiency.B1);
            result[1].LanguageName.Should().Be("Ukranian");
            result[1].Proficiency.Should().Be(LanguageProficiency.C2);
        }

        [Fact]
        public async Task Hadnle_WhenLanguagesDoesNotExcist_ShouldReturnEmptyList()
        {
            // Arrange
            using var context = TestDbContext.CreateInMemory();

            var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
            var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
            var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);

            context.Resumes.Add(resume);
            context.ResumeRevisions.Add(revision);
            await context.SaveChangesAsync(CancellationToken.None);

            var query = new GetLanguagesQuery(revision.Id);
            var handler = new GetLanguagesQueryHandler(context);

            // Act 
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }
    }
}
