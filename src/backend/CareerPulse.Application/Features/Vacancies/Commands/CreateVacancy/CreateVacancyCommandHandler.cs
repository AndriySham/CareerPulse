using CareerPulse.Application.Common.Mappings;
using CareerPulse.Application.DTOs.Vacancies;
using CareerPulse.Application.Exceptions;
using CareerPulse.Application.Interfaces;
using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CareerPulse.Application.Features.Vacancies.Commands.CreateVacancy;

/// <summary>
/// Command handler for creating a Vacancy entity.
/// </summary>
public sealed class CreateVacancyCommandHandler
    : IRequestHandler<CreateVacancyCommand, VacancyDto>
{
    private readonly IApplicationDbContext _context;

    public CreateVacancyCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<VacancyDto> Handle(
        CreateVacancyCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var companyExists = await _context.Companies
            .AnyAsync(c => c.Id == dto.CompanyId, cancellationToken);

        if (!companyExists)
        {
            throw new ResourceNotFoundException($"Company with ID '{dto.CompanyId}' was not found.");
        }

        var vacancy = Vacancy.Create(
            dto.CompanyId,
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
            dto.Url,
            dto.PostedAt);

        vacancy.ReplaceLanguageRequirements(
            dto.LanguageRequirements.Select(x => new LanguageRequirementInput(
                x.LanguageName,
                x.Proficiency,
                x.ProficiencyDescription
            )));

        _context.Vacancies.Add(vacancy);
        await _context.SaveChangesAsync(cancellationToken);

        return VacancyMapping.MapToDto(vacancy);
    }
}
