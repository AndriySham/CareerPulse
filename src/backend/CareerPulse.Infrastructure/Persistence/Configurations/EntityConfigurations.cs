using CareerPulse.Domain.Entities;
using CareerPulse.Domain.Enums;
using CareerPulse.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareerPulse.Infrastructure.Persistence.Configurations;

public sealed class ApplicationConfiguration : IEntityTypeConfiguration<Domain.Entities.Application>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Application> builder)
    {
        builder.ToTable("Applications");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.JobSource).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(4000);

        builder.HasOne(x => x.Company)
            .WithMany()
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Vacancy)
            .WithMany()
            .HasForeignKey(x => x.VacancyId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.ResumeRevision)
            .WithMany()
            .HasForeignKey(x => x.ResumeRevisionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Interviews)
            .WithOne()
            .HasForeignKey(x => x.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.CompanyId);
        builder.HasIndex(x => x.SubmissionDate);
    }
}

public sealed class ResumeRevisionConfiguration : IEntityTypeConfiguration<ResumeRevision>
{
    public void Configure(EntityTypeBuilder<ResumeRevision> builder)
    {
        builder.ToTable("ResumeRevisions");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.ProfessionalSummary).HasColumnType("text");
        builder.Property(x => x.FileReference).HasMaxLength(1000);

        // PersonalInfo owned entity (value object stored as columns)
        builder.OwnsOne(x => x.PersonalInfo, pi =>
        {
            pi.Property(p => p.FullName).HasMaxLength(200).IsRequired();
            pi.Property(p => p.Email).HasMaxLength(254).IsRequired();
            pi.Property(p => p.Phone).HasMaxLength(50);
            pi.Property(p => p.LinkedIn).HasMaxLength(500);
            pi.Property(p => p.GitHub).HasMaxLength(500);
            pi.Property(p => p.Location).HasMaxLength(200);
        });

        builder.HasMany(x => x.Skills)
            .WithOne()
            .HasForeignKey(x => x.ResumeRevisionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.ParentRevisionId);
    }
}

public sealed class ResumeRevisionSkillConfiguration : IEntityTypeConfiguration<ResumeRevisionSkill>
{
    public void Configure(EntityTypeBuilder<ResumeRevisionSkill> builder)
    {
        builder.ToTable("ResumeRevisionSkills");
        builder.HasKey(x => new { x.ResumeRevisionId, x.MasterSkillId });
        builder.Property(x => x.ProficiencyLevel).HasDefaultValue(3);

        builder.HasOne(x => x.MasterSkill)
            .WithMany()
            .HasForeignKey(x => x.MasterSkillId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Companies");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(300).IsRequired();
        builder.Property(x => x.Website).HasMaxLength(500);
        builder.Property(x => x.Industry).HasMaxLength(150);
        builder.Property(x => x.Notes).HasMaxLength(4000);

        builder.HasMany(x => x.Vacancies)
            .WithOne(v => v.Company)
            .HasForeignKey(v => v.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.Name);
    }
}

public sealed class VacancyConfiguration : IEntityTypeConfiguration<Vacancy>
{
    public void Configure(EntityTypeBuilder<Vacancy> builder)
    {
        builder.ToTable("Vacancies");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).HasMaxLength(300).IsRequired();
        builder.Property(x => x.Description).HasColumnType("text");
        builder.Property(x => x.Url).HasMaxLength(1000);

        builder.HasIndex(x => x.CompanyId);
    }
}

public sealed class InterviewConfiguration : IEntityTypeConfiguration<Interview>
{
    public void Configure(EntityTypeBuilder<Interview> builder)
    {
        builder.ToTable("Interviews");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(x => x.Notes).HasColumnType("text");
        builder.Property(x => x.Feedback).HasColumnType("text");

        builder.HasIndex(x => x.ApplicationId);
    }
}

public sealed class MasterSkillConfiguration : IEntityTypeConfiguration<MasterSkill>
{
    public void Configure(EntityTypeBuilder<MasterSkill> builder)
    {
        builder.ToTable("MasterSkills");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Category).HasConversion<string>().HasMaxLength(50).IsRequired();

        builder.HasIndex(x => x.Name).IsUnique();
        builder.HasIndex(x => x.Category);

        builder.HasMany(x => x.Aliases)
            .WithOne(a => a.MasterSkill)
            .HasForeignKey(a => a.MasterSkillId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class MasterSkillAliasConfiguration : IEntityTypeConfiguration<MasterSkillAlias>
{
    public void Configure(EntityTypeBuilder<MasterSkillAlias> builder)
    {
        builder.ToTable("MasterSkillAliases");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.AliasName).HasMaxLength(200).IsRequired();

        builder.HasIndex(x => new { x.MasterSkillId, x.AliasName }).IsUnique();
    }
}
