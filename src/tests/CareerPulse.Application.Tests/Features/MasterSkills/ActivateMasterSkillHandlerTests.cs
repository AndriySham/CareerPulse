using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Features.MasterSkills.Commands.ActivateMasterSkill;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CareerPulse.Application.Tests.Features.MasterSkills;

public class ActivateMasterSkillHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldActivateMasterSkill()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var skill = MasterSkill.Create("C#", SkillCategory.ProgrammingLanguage);
        context.MasterSkills.Add(skill);
        skill.Deactivate();
        await context.SaveChangesAsync(CancellationToken.None);

        var command = new ActivateMasterSkillCommand(skill.Id);
        var handler = new ActivateMasterSkillCommandHandler(context);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var dbSkill = await context.MasterSkills.FirstOrDefaultAsync(x => x.Id == skill.Id);

        dbSkill.Should().NotBeNull();
        dbSkill!.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenMasterSkillDoesNotExist_ShouldThrowResourseNotFoundException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var skillId = Guid.NewGuid();
        var command = new ActivateMasterSkillCommand(skillId);
        var handler = new ActivateMasterSkillCommandHandler(context);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert

        await act.Should().ThrowAsync<ResourceNotFoundException>()
            .WithMessage($"MasterSkill with ID '{skillId}' was not found.");
    }

    [Fact]
    public async Task Handle_WhenMasterSkillAlreadyActive_ShouldRemainActive()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var skill = MasterSkill.Create("C#", SkillCategory.ProgrammingLanguage);
        context.MasterSkills.Add(skill);
        await context.SaveChangesAsync(CancellationToken.None);

        var command = new ActivateMasterSkillCommand(skill.Id);
        var handler = new ActivateMasterSkillCommandHandler(context);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var dbSkill = await context.MasterSkills.FirstOrDefaultAsync(x => x.Id == skill.Id);

        dbSkill.Should().NotBeNull();
        dbSkill!.IsActive.Should().BeTrue();
    }
}
