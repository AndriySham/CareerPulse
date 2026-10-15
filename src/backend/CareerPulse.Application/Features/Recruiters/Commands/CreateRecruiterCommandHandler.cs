using CareerPulse.Application.Common.Mappings;
using CareerPulse.Application.DTOs.Recruiters;
using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Interfaces;
using CareerPulse.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Recruiters.Commands;

/// <summary>
/// Command handler for creating a Recruiter entity.
/// </summary>
public sealed class CreateRecruiterCommandHandler
    : IRequestHandler<CreateRecruiterCommand, RecruiterDto>
{
    private readonly IApplicationDbContext _context;

    public CreateRecruiterCommandHandler(IApplicationDbContext context)
    {
        _context = context;        
    }

    public async Task<RecruiterDto> Handle(
        CreateRecruiterCommand request,
        CancellationToken cancellationToken)
    {
        var companyExists = await _context.Companies
            .AnyAsync(x => x.Id == request.CompanyId, cancellationToken);
        if (!companyExists) 
        {
            throw new ResourceNotFoundException($"Company with ID '{request.CompanyId}' was not found.");
        }

        var recruiter = Recruiter.Create(
            request.CompanyId,
            request.Dto.Name,
            request.Dto.Phone,
            request.Dto.Email,
            request.Dto.LinkedInUrl,
            request.Dto.TelegramUrl,
            request.Dto.Notes);

        _context.Recruiters.Add(recruiter);
        await _context.SaveChangesAsync(cancellationToken);

        return RecruiterMapping.MapToDto(recruiter);
    }
}
