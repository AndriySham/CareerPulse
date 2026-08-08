using CareerPulse.Domain.Enums;
using CareerPulse.Domain.Exceptions;
using CareerPulse.Domain.StateMachines;
using FluentAssertions;
using Xunit;

namespace CareerPulse.Application.Tests.Features.Applications.Domain;

public class ApplicationStatusMachineTests
{
    [Theory]
    [InlineData(ApplicationStatus.Draft, ApplicationStatus.Applied)]
    [InlineData(ApplicationStatus.Applied, ApplicationStatus.Viewed)]
    [InlineData(ApplicationStatus.Applied, ApplicationStatus.Rejected)]
    [InlineData(ApplicationStatus.Applied, ApplicationStatus.NoResponse)]
    [InlineData(ApplicationStatus.Viewed, ApplicationStatus.HRInterview)]
    [InlineData(ApplicationStatus.Viewed, ApplicationStatus.Rejected)]
    [InlineData(ApplicationStatus.HRInterview, ApplicationStatus.TechnicalInterview)]
    [InlineData(ApplicationStatus.HRInterview, ApplicationStatus.Rejected)]
    [InlineData(ApplicationStatus.TechnicalInterview, ApplicationStatus.Offer)]
    [InlineData(ApplicationStatus.TechnicalInterview, ApplicationStatus.Rejected)]
    public void ValidateTransition_WithValidTransition_ShouldNotThrow(ApplicationStatus current, ApplicationStatus next)
    {
        // Act
        var act = () => ApplicationStatusMachine.ValidateTransition(current, next);

        // Assert
        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(ApplicationStatus.Draft, ApplicationStatus.HRInterview)]
    [InlineData(ApplicationStatus.Draft, ApplicationStatus.Offer)]
    [InlineData(ApplicationStatus.Applied, ApplicationStatus.HRInterview)]
    [InlineData(ApplicationStatus.Offer, ApplicationStatus.Draft)]
    [InlineData(ApplicationStatus.Rejected, ApplicationStatus.Applied)]
    [InlineData(ApplicationStatus.NoResponse, ApplicationStatus.Viewed)]
    public void ValidateTransition_WithInvalidTransition_ShouldThrowDomainException(ApplicationStatus current, ApplicationStatus next)
    {
        // Act
        var act = () => ApplicationStatusMachine.ValidateTransition(current, next);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage($"*{current}*")
            .WithMessage($"*{next}*");
    }

    [Fact]
    public void GetAllowedTransitions_FromDraft_ShouldReturnApplied()
    {
        // Act
        var allowed = ApplicationStatusMachine.GetAllowedTransitions(ApplicationStatus.Draft);

        // Assert
        allowed.Should().BeEquivalentTo(new[] { ApplicationStatus.Applied });
    }

    [Fact]
    public void GetAllowedTransitions_FromTerminalState_ShouldReturnEmptySet()
    {
        // Act & Assert
        ApplicationStatusMachine.GetAllowedTransitions(ApplicationStatus.Offer).Should().BeEmpty();
        ApplicationStatusMachine.GetAllowedTransitions(ApplicationStatus.Rejected).Should().BeEmpty();
        ApplicationStatusMachine.GetAllowedTransitions(ApplicationStatus.NoResponse).Should().BeEmpty();
    }
}
