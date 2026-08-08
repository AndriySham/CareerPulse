using CareerPulse.Application.DTOs.Vacancies;
using CareerPulse.Application.Features.Vacancies.Commands.CreateVacancy;
using CareerPulse.Application.Interfaces;
using CareerPulse.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Vacancies.Commands.UpdateVacancy;

/// <summary>
/// Command handler for updating an existing Vacancy entity.
/// </summary>
public sealed class UpdateVacancyCommandHandler
    : IRequestHandler<UpdateVacancyCommand, VacancyDto>
{
    private readonly IApplicationDbContext _context;

    public UpdateVacancyCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<VacancyDto> Handle(
        UpdateVacancyCommand request,
        CancellationToken cancellationToken)
    {
        var vacancy = await _context.Vacancies
            .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);

        if (vacancy == null)
        {
            throw new DomainException($"Vacancy with ID '{request.Id}' was not found.");
        }

        var dto = request.Dto;
        var trimmedTitle = dto.Title?.Trim() ?? string.Empty;

        vacancy.Update(trimmedTitle, dto.Description, dto.Url);

        await _context.SaveChangesAsync(cancellationToken);

        return CreateVacancyCommandHandler.MapToDto(vacancy);
    }
}
