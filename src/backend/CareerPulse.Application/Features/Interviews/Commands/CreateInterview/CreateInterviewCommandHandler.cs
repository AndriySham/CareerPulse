using CareerPulse.Application.Common.Mappings;
using CareerPulse.Application.DTOs.Interviews;
using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Interfaces;
using CareerPulse.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Interviews.Commands.CreateInterview;

/// <summary>
/// Command handler for creating an Interview entity.
/// </summary>
public sealed class CreateInterviewCommandHandler
    : IRequestHandler<CreateInterviewCommand, InterviewDto>
{
    private readonly IApplicationDbContext _context;

    public CreateInterviewCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<InterviewDto> Handle(
        CreateInterviewCommand request,
        CancellationToken cancellationToken)
    {
        var applicationExist = await _context.Applications
            .AnyAsync(x => x.Id == request.ApplicationId, cancellationToken);
        if (!applicationExist)
        {
            throw new ResourceNotFoundException($"Application with ID '{request.ApplicationId}' was not found.");
        }

        var interview = Interview.Create(
            request.ApplicationId,
            request.Dto.Type,
            request.Dto.ScheduledAt,
            request.Dto.ConductedAt,
            request.Dto.Notes,
            request.Dto.Feedback);

        _context.Interviews.Add(interview);
        await _context.SaveChangesAsync(cancellationToken);

        return InterviewMapping.MapToDto(interview);
    }
}
