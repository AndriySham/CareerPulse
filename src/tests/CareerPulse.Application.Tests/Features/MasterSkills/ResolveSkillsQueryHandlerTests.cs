using CareerPulse.Application.DTOs.MasterSkills;
using CareerPulse.Application.Features.MasterSkills.Queries.ResolveSkills;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CareerPulse.Application.Tests.Features.MasterSkills;

public class ResolveSkillsQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenRawSkillsMatchCanonicalNameAndAlias_ShouldAutoResolveCorrectly()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var csharp = MasterSkill.Create("C#", SkillCategory.ProgrammingLanguage);
        var efCore = MasterSkill.Create("Entity Framework Core", SkillCategory.ORM);
        efCore.AddAlias("EF Core");

        context.MasterSkills.AddRange(csharp, efCore);
        await context.SaveChangesAsync();

        var handler = new ResolveSkillsQueryHandler(context);
        var query = new ResolveSkillsQuery(new[] { "c#", "  ef core  ", "ReactJS" });

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.ResolvedSkills.Should().HaveCount(2);
        result.UnknownSkills.Should().HaveCount(1);

        var csharpRes = result.ResolvedSkills.First(r => r.RawText == "c#");
        csharpRes.IsResolved.Should().BeTrue();
        csharpRes.ResolutionStatus.Should().Be(SkillResolutionStatus.AutoResolved);
        csharpRes.MasterSkill!.Name.Should().Be("C#");

        var efRes = result.ResolvedSkills.First(r => r.RawText == "  ef core  ");
        efRes.IsResolved.Should().BeTrue();
        efRes.ResolutionStatus.Should().Be(SkillResolutionStatus.AutoResolved);
        efRes.MasterSkill!.Name.Should().Be("Entity Framework Core");

        var unknownRes = result.UnknownSkills.Single();
        unknownRes.RawText.Should().Be("ReactJS");
        unknownRes.IsResolved.Should().BeFalse();
        unknownRes.ResolutionStatus.Should().Be(SkillResolutionStatus.NeedsUserInput);
        unknownRes.MasterSkill.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldBeReadOnlyAndNotPersistNewMasterSkills()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var handler = new ResolveSkillsQueryHandler(context);
        var query = new ResolveSkillsQuery(new[] { "Unknown Skill 1", "Unknown Skill 2" });

        // Act
        await handler.Handle(query, CancellationToken.None);

        // Assert
        var count = await context.MasterSkills.CountAsync();
        count.Should().Be(0);
    }

    [Fact]
    public async Task Handle_WhenMasterSkillIsInactive_ShouldNotMatchAndMarkAsNeedsUserInput()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();
        var inactiveSkill = MasterSkill.Create("Python", SkillCategory.ProgrammingLanguage);
        inactiveSkill.Deactivate();

        context.MasterSkills.Add(inactiveSkill);
        await context.SaveChangesAsync();

        var handler = new ResolveSkillsQueryHandler(context);
        var query = new ResolveSkillsQuery(new[] { "Python" });

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.ResolvedSkills.Should().BeEmpty();
        result.UnknownSkills.Should().HaveCount(1);
        result.UnknownSkills[0].ResolutionStatus.Should().Be(SkillResolutionStatus.NeedsUserInput);
    }
}
