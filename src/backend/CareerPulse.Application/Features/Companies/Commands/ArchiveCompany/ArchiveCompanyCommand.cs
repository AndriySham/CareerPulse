using MediatR;

namespace CareerPulse.Application.Features.Companies.Commands.ArchiveCompany;

/// <summary>
/// Command to archive an existing Company.
/// </summary>
public sealed record ArchiveCompanyCommand(Guid Id) : IRequest;
