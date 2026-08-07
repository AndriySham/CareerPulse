using CareerPulse.Domain.Enums;
using CareerPulse.Domain.Exceptions;
using CareerPulse.Domain.StateMachines;
using FluentAssertions;
using Xunit;

namespace CareerPulse.Domain.Tests.StateMachines;

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
    public void ValidateTransition_WhenTransitionIsValid_ShouldNotThrow(ApplicationStatus current, ApplicationStatus next)
    {
        // Act
        var act = () => ApplicationStatusMachine.ValidateTransition(current, next);

        // Assert
        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(ApplicationStatus.Draft, ApplicationStatus.Offer)]
    [InlineData(ApplicationStatus.Draft, ApplicationStatus.Viewed)]
    [InlineData(ApplicationStatus.Draft, ApplicationStatus.Rejected)]
    [InlineData(ApplicationStatus.Offer, ApplicationStatus.Applied)]
    [InlineData(ApplicationStatus.Rejected, ApplicationStatus.Applied)]
    [InlineData(ApplicationStatus.NoResponse, ApplicationStatus.Viewed)]
    [InlineData(ApplicationStatus.TechnicalInterview, ApplicationStatus.HRInterview)]
    public void ValidateTransition_WhenTransitionIsInvalid_ShouldThrowDomainException(ApplicationStatus current, ApplicationStatus next)
    {
        // Act
        var act = () => ApplicationStatusMachine.ValidateTransition(current, next);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage($"*{current}*");
    }

    [Fact]
    public void GetAllowedTransitions_ShouldReturnCorrectAllowedSet()
    {
        // Act & Assert
        ApplicationStatusMachine.GetAllowedTransitions(ApplicationStatus.Draft)
            .Should().BeEquivalentTo(new[] { ApplicationStatus.Applied });

        ApplicationStatusMachine.GetAllowedTransitions(ApplicationStatus.Applied)
            .Should().BeEquivalentTo(new[] { ApplicationStatus.Viewed, ApplicationStatus.Rejected, ApplicationStatus.NoResponse });

        ApplicationStatusMachine.GetAllowedTransitions(ApplicationStatus.Offer)
            .Should().BeEmpty();
    }
}
