using CareerPulse.Application.DTOs.Resumes;
using MediatR;

namespace CareerPulse.Application.Features.Resumes.Commands.UpdateResumeDraft;

/// <summary>
/// Command to update summary and skills of an existing draft revision.
/// ADR 005: Enforces EnsureDraft() — throws DomainException if Applied.
/// </summary>
public sealed record UpdateResumeDraftCommand(Guid Id, UpdateResumeDraftDto Dto) : IRequest<ResumeRevisionDto>;
