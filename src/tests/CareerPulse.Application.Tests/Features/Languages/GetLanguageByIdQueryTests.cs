using CareerPulse.Application.Features.Languages.Queries.GetLanguageById;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Enums;
using CareerPulse.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace CareerPulse.Application.Tests.Features.Languages
{
    public class GetLanguageByIdQueryTests
    {
        [Fact]
        public async Task Handle_WhenLanguageExists_ShouldReturnMappedLanguageDto()
        {
            // Arrange
            using var context = TestDbContext.CreateInMemory();

            var personalInfo = PersonalInfo.Create("Alice Smith", "alice@example.com");
            var resume = Resume.Create("Alice's Resume", ResumeTrack.FullStack, CareerLevel.Senior, "Dotnet Dev");
            var revision = resume.CreateFirstRevision("Dotnet Dev", personalInfo);

            var language = Language.Create(revision.Id, "English", LanguageProficiency.B1);

            context.Resumes.Add(resume);
            context.ResumeRevisions.Add(revision);
            context.Languages.Add(language);
            await context.SaveChangesAsync(CancellationToken.None);

            var query = new GetLanguageByIdQuery(language.Id);
            var handler = new GetLanguageByIdQueryHandler(context);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(language.Id);
            result.ResumeRevisionId.Should().Be(language.ResumeRevisionId);
            result.LanguageName.Should().Be("English");
            result.Proficiency.Should().Be(LanguageProficiency.B1);
        }

        [Fact]
        public async Task Handle_WhenLanguageDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            using var context = TestDbContext.CreateInMemory();

            var query = new GetLanguageByIdQuery(Guid.NewGuid());
            var handler = new GetLanguageByIdQueryHandler(context);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}
