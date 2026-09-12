using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Vacancies.Commands.DeleteVacancy;

/// <summary>
/// Command handler for delete an existing Vacancy entity.
/// </summary>
public sealed class DeleteVacancyCommandHandler : IRequestHandler<DeleteVacancyCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteVacancyCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(
        DeleteVacancyCommand request,
        CancellationToken cancellationToken)
    {
        var vacancy = await _context
            .Vacancies.FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);

        if (vacancy == null)
        {
            throw new ResourceNotFoundException($"Vacancy with ID '{request.Id}' was not found.");
        }

        var isApplied = await _context
            .Applications.AnyAsync(a => a.VacancyId == request.Id, cancellationToken);

        if (isApplied)
        {
            throw new ConflictException("There is an application for the vacancy. Deletion is prohibited.");
        }

        _context.Vacancies.Remove(vacancy);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
