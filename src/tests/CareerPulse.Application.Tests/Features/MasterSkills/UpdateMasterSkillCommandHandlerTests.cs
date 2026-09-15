using CareerPulse.Application.DTOs.MasterSkills;
using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Features.MasterSkills.Commands.UpdateMasterSkill;
using CareerPulse.Application.Tests.TestHelpers;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CareerPulse.Application.Tests.Features.MasterSkills;

public class UpdateMasterSkillCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldUpdateMasterSkill()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var skill = MasterSkill.Create("C#",SkillCategory.ProgrammingLanguage);
        skill.AddAlias("csharp");
        skill.AddAlias("C Sharp");
        context.MasterSkills.Add(skill);

        await context.SaveChangesAsync(CancellationToken.None);

        var dto = new UpdateMasterSkillDto
        {
            Name = ".NET",
            Category = SkillCategory.Framework,
            Aliases = ["dotnet", "dot net"]
        };

        var command = new UpdateMasterSkillCommand(skill.Id, dto);
        var handler = new UpdateMasterSkillCommandHandler(context);

        // Act
        var result = await handler.Handle(command,CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(".NET");
        result.Category.Should().Be(SkillCategory.Framework);
        result.Aliases.Should().BeEquivalentTo(["dotnet", "dot net"]);

        var dbSkill = await context.MasterSkills
            .Include(x => x.Aliases)
            .FirstOrDefaultAsync(x => x.Id == skill.Id, CancellationToken.None);

        dbSkill.Should().NotBeNull();
        dbSkill!.Name.Should().Be(".NET");
        dbSkill.Category.Should().Be(SkillCategory.Framework);
        dbSkill.Aliases.Select(x => x.AliasName).Should().BeEquivalentTo(["dotnet", "dot net"]);
    }

    [Fact]
    public async Task Handle_WhenMasterSkillDoesNotExist_ShouldThrowResourceNotFoundExceprion()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var skillId = Guid.NewGuid();

        var dto = new UpdateMasterSkillDto
        {
            Name = ".NET",
            Category = SkillCategory.Framework,
            Aliases = ["dotnet"]
        };

        var command = new UpdateMasterSkillCommand(skillId, dto);
        var handler = new UpdateMasterSkillCommandHandler(context);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>()
            .WithMessage($"MasterSkill with ID '{skillId}' was not found");
    }

    [Fact]
    public async Task Hanle_WhenNewNameAlreadyExists_ShouldThrowConflictException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var existingSkill = MasterSkill.Create(".NET", SkillCategory.Framework);
        var skillToUpdate = MasterSkill.Create("C#", SkillCategory.ProgrammingLanguage);
        
        context.MasterSkills.Add(existingSkill);
        context.MasterSkills.Add(skillToUpdate);

        await context.SaveChangesAsync(CancellationToken.None);

        var dto = new UpdateMasterSkillDto
        {
            Name = ".NET",
            Category = SkillCategory.Framework,
            Aliases = []
        };

        var command = new UpdateMasterSkillCommand(skillToUpdate.Id, dto);
        var handler = new UpdateMasterSkillCommandHandler(context);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("New MasterSkill name '.NET' already exists.");
    }

    [Fact]
    public async Task Handle_WhenNewNameConflictsWithAlias_ShouldThrowConflictException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var existingSkill = MasterSkill.Create("C#", SkillCategory.ProgrammingLanguage);
        existingSkill.AddAlias("csharp");

        var skillToUpdate = MasterSkill.Create(".NET", SkillCategory.Framework);

        context.MasterSkills.Add(existingSkill);
        context.MasterSkills.Add(skillToUpdate);

        await context.SaveChangesAsync(CancellationToken.None);

        var dto = new UpdateMasterSkillDto
        {
            Name = "csharp",
            Category = SkillCategory.Framework,
            Aliases = []
        };

        var command = new UpdateMasterSkillCommand(skillToUpdate.Id, dto);

        var handler = new UpdateMasterSkillCommandHandler(context);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("New MasterSkill Name 'csharp' conflicts with an existing alias.");
    }

    [Fact]
    public async Task Handle_WhenAliasConflictsWithAnotherSkillName_ShouldThrowConflictException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var existingSkill = MasterSkill.Create(".NET", SkillCategory.Framework);

        var skillToUpdate = MasterSkill.Create("C#", SkillCategory.ProgrammingLanguage);

        context.MasterSkills.Add(existingSkill);
        context.MasterSkills.Add(skillToUpdate);

        await context.SaveChangesAsync(CancellationToken.None);

        var dto = new UpdateMasterSkillDto
        {
            Name = "C#",
            Category = SkillCategory.ProgrammingLanguage,
            Aliases = [".NET"]
        };

        var command = new UpdateMasterSkillCommand(skillToUpdate.Id,dto);
        var handler = new UpdateMasterSkillCommandHandler(context);

        // Act
        var act = () => handler.Handle(command,CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("Alias '.NET' conflicts with an existing skill or alias.");
    }

    [Fact]
    public async Task Handle_WhenAliasConflictsWithAnotherSkillAlias_ShouldThrowConflictException()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var existingSkill = MasterSkill.Create(".NET", SkillCategory.Framework);

        existingSkill.AddAlias("dotnet");

        var skillToUpdate = MasterSkill.Create("C#", SkillCategory.ProgrammingLanguage);

        context.MasterSkills.Add(existingSkill);
        context.MasterSkills.Add(skillToUpdate);

        await context.SaveChangesAsync(CancellationToken.None);

        var dto = new UpdateMasterSkillDto
        {
            Name = "C#",
            Category = SkillCategory.ProgrammingLanguage,
            Aliases = ["dotnet"]
        };

        var command = new UpdateMasterSkillCommand(skillToUpdate.Id, dto);
        var handler = new UpdateMasterSkillCommandHandler(context);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("Alias 'dotnet' conflicts with an existing skill or alias.");
    }

    [Fact]
    public async Task Handle_WhenKeepingOwnAliases_ShouldUpdateSuccessfully()
    {
        // Arrange
        using var context = TestDbContext.CreateInMemory();

        var skill = MasterSkill.Create("C#", SkillCategory.ProgrammingLanguage);

        skill.AddAlias("csharp");
        skill.AddAlias("C Sharp");

        context.MasterSkills.Add(skill);
        await context.SaveChangesAsync(CancellationToken.None);

        var dto = new UpdateMasterSkillDto
        {
            Name = "C#",
            Category = SkillCategory.ProgrammingLanguage,
            Aliases = ["csharp", "C Sharp", "dotnet-csharp"]
        };

        var command = new UpdateMasterSkillCommand(skill.Id, dto);
        var handler = new UpdateMasterSkillCommandHandler(context);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Name.Should().Be("C#");
        result.Aliases.Should().BeEquivalentTo(["csharp", "C Sharp", "dotnet-csharp"]);
        var dbSkill = await context.MasterSkills
            .Include(x => x.Aliases)
            .FirstOrDefaultAsync(x => x.Id == skill.Id, CancellationToken.None);

        dbSkill.Should().NotBeNull();
        dbSkill!.Aliases.Select(x => x.AliasName).Should().BeEquivalentTo(["csharp", "C Sharp", "dotnet-csharp"]);
    }
}
