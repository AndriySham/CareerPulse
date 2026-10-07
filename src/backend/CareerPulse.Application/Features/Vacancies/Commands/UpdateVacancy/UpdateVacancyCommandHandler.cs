using CareerPulse.Application.Common.Mappings;
using CareerPulse.Application.DTOs.Vacancies;
using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Interfaces;
using CareerPulse.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Vacancies.Commands.UpdateVacancy;

/// <summary>
/// Command handler for updating an existing Vacancy entity.
/// </summary>
public sealed class UpdateVacancyCommandHandler
    : IRequestHandler<UpdateVacancyCommand, VacancyDetailsDto>
{
    private readonly IApplicationDbContext _context;

    public UpdateVacancyCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<VacancyDetailsDto> Handle(
        UpdateVacancyCommand request,
        CancellationToken cancellationToken)
    {
        var vacancy = await _context.Vacancies
            .Include(x => x.LanguageRequirements)
            .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);

        if (vacancy == null)
        {
            throw new ResourceNotFoundException($"Vacancy with ID '{request.Id}' was not found.");
        }

        var dto = request.Dto;

        vacancy.Update(
            dto.Title,
            dto.Location,
            dto.WorkMode,
            dto.EmploymentType,
            dto.SalaryMin,
            dto.SalaryMax,
            dto.SalaryCurrency,
            dto.Description,
            dto.Responsibilities,
            dto.Requirements,
            dto.NiceToHave,
            dto.Benefits,
            dto.Url);

        vacancy.ReplaceLanguageRequirements(
            dto.LanguageRequirements.Select(x => new LanguageRequirementInput(
                x.LanguageName,
                x.Proficiency,
            x.ProficiencyDescription)));

        await _context.SaveChangesAsync(cancellationToken);

        return VacancyDetailsMapping.MapToDto(vacancy);
    }
}
