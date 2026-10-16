using CareerPulse.Application.Common.Mappings;
using CareerPulse.Application.DTOs.Recruiters;
using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Recruiters.Commands.UpdateRecruiter;

/// <summary>
/// Command handler for updating a Recruiter entity.
/// </summary>
public sealed class UpdateRecruiterCommandHandler
    : IRequestHandler<UpdateRecruiterCommand, RecruiterDto>
{
    private readonly IApplicationDbContext _context;

    public UpdateRecruiterCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RecruiterDto> Handle(
        UpdateRecruiterCommand request,
        CancellationToken cancellationToken)
    {
        var recruiter = await _context.Recruiters
            .FirstOrDefaultAsync(x => x.Id == request.RecruiterId
                && x.CompanyId == request.CompanyId, cancellationToken);
        if (recruiter == null)
        {
            throw new ResourceNotFoundException($"Resource with Company ID '{request.CompanyId}' " +
                $"and Recruiter ID '{request.RecruiterId}' was not found.");
        }

        recruiter.Update(
            request.Dto.Name,
            request.Dto.Phone,
            request.Dto.Email,
            request.Dto.LinkedInUrl,
            request.Dto.TelegramUrl,
            request.Dto.Notes);

        await _context.SaveChangesAsync(cancellationToken);

        return RecruiterMapping.MapToDto(recruiter);
    }
}
