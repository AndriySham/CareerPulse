using CareerPulse.Domain.Enums;
using CareerPulse.Domain.Exceptions;

namespace CareerPulse.Domain.Entities;

/// <summary>
/// Aggregate Root: Normalized technical skill in the global MasterSkill catalog.
/// ADR 006: All skills in a ResumeRevision must resolve to a MasterSkill.
/// Aliases handle textual variations (e.g., "EF" → "Entity Framework Core").
/// </summary>
public sealed class MasterSkill
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty; // Canonical name
    public SkillCategory Category { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; }

    private readonly List<MasterSkillAlias> _aliases = new();
    public IReadOnlyCollection<MasterSkillAlias> Aliases => _aliases.AsReadOnly();

    private MasterSkill() { }

    public static MasterSkill Create(string name, SkillCategory category)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("MasterSkill name is required.");

        return new MasterSkill
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Category = category,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// ADR 006: New aliases require explicit user decision — never created automatically.
    /// </summary>
    public void AddAlias(string aliasName)
    {
        if (string.IsNullOrWhiteSpace(aliasName))
            throw new DomainException("Alias name is required.");

        if (_aliases.Any(a => a.AliasName.Equals(aliasName, StringComparison.OrdinalIgnoreCase)))
            throw new DomainException($"Alias '{aliasName}' already exists for skill '{Name}'.");

        _aliases.Add(new MasterSkillAlias(Id, aliasName));
    }

    public void Deactivate() => IsActive = false;

    public void Activate() => IsActive = true;
}
