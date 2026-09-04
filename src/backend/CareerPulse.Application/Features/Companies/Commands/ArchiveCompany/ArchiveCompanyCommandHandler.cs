using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Companies.Commands.ArchiveCompany;

/// <summary>
/// Command handler for archive an existing Company entity.
/// </summary>
public sealed class ArchiveCompanyCommandHandler 
    : IRequestHandler<ArchiveCompanyCommand>
{
    private readonly IApplicationDbContext _context;

    public ArchiveCompanyCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(
        ArchiveCompanyCommand request,
        CancellationToken cancellationToken)
    {
        var company = await _context.Companies
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
    
        if (company == null)
        {
            throw new ResourceNotFoundException($"Company with ID '{request.Id}' was not found.");
        }

        company.Archive();

        await _context.SaveChangesAsync(cancellationToken);
    }
}
