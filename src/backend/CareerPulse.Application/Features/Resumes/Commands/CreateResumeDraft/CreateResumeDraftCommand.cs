using CareerPulse.Application.DTOs.Resumes;
using MediatR;

namespace CareerPulse.Application.Features.Resumes.Commands.CreateResumeDraft;

/// <summary>
/// Command to create a new ResumeRevision draft (Version = 1, Status = Draft).
/// </summary>
public sealed record CreateResumeDraftCommand(CreateResumeDraftDto Dto) : IRequest<ResumeRevisionDto>;
