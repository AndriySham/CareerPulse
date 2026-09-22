using CareerPulse.Application.DTOs.Languages;
using CareerPulse.Domain.Entities;

namespace CareerPulse.Application.Common.Mappings;

public class LanguageMapping
{
    public static LanguageDto MapToDto(Language language) => new()
    {
        Id = language.Id,
        ResumeRevisionId = language.ResumeRevisionId,
        LanguageName = language.LanguageName,
        Proficiency = language.Proficiency
    };
}
