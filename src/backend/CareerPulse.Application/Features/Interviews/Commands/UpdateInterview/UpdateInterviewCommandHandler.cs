using CareerPulse.Application.Common.Mappings;
using CareerPulse.Application.DTOs.Interviews;
using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Interviews.Commands.UpdateInterview;

/// <summary>
/// Command handler for updating an existing Interview Entity.
/// </summary>
public sealed class UpdateInterviewCommandHandler 
    : IRequestHandler<UpdateInterviewCommand, InterviewDto>
{
    private readonly IApplicationDbContext _context;

    public UpdateInterviewCommandHandler(IApplicationDbContext contex)
    {
        _context = contex;
    }

    public async Task<InterviewDto> Handle(
        UpdateInterviewCommand request,
        CancellationToken cancellationToken)
    {
        var interview = await _context.Interviews
            .FirstOrDefaultAsync(x => x.Id == request.InterviewId &&
                x.ApplicationId == request.ApplicationId,
            cancellationToken);

        if (interview == null)
        {
            throw new ResourceNotFoundException($"Resource with application ID '{request.ApplicationId}' and interview ID '{request.InterviewId}' was not found.");
        }

        interview.Update(
            request.Dto.Type,
            request.Dto.ScheduledAt,
            request.Dto.ConductedAt,
            request.Dto.Notes,
            request.Dto.Feedback);

        await _context.SaveChangesAsync(cancellationToken);

        return InterviewMapping.MapToDto(interview);
    }
}
