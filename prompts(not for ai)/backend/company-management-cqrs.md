Read AGENTS.md, ADR.md, DOMAIN_MODEL.md, ARCHITECTURE.md, and MEMORY.md.

Execute Lesson 24.4.1: Company CQRS Use Cases.

Before implementation, inspect the existing Company domain entity, its domain methods, EF Core configuration, IApplicationDbContext, and existing CQRS/DTO conventions.

Implement Company management use cases under CareerPulse.Application:

1. DTOs:
- CompanyDto
- CreateCompanyDto
- UpdateCompanyDto

2. CQRS Features:
- CreateCompanyCommand + Handler + FluentValidation Validator
- UpdateCompanyCommand + Handler + FluentValidation Validator
- GetCompaniesQuery + Handler
- GetCompanyByIdQuery + Handler

Requirements:
- Follow the existing Clean Architecture and CQRS structure.
- Use IApplicationDbContext for persistence.
- Use existing Company domain methods and invariants.
- Do not invent domain methods, properties, or business rules.
- CreateCompany must enforce the existing company-name uniqueness rule.
- UpdateCompany must modify only fields supported by the existing domain model.
- Queries must use AsNoTracking().
- Use CancellationToken for async database operations.
- Follow the existing DomainException / RFC 7807 error-handling conventions.

Do NOT implement:
- Vacancy
- Application
- Interview
- API Controllers
- Unit tests

After implementation:
- Run dotnet build "src/backend/CareerPulse.sln" -c Release.
- Fix compilation errors.
- Review git diff and ensure only Lesson 24.4.1 related files were changed.

At completion provide:
- Files created/modified.
- CQRS use cases implemented.
- Architectural decisions or assumptions.
- Any discrepancy found between the documentation and actual Company domain model.
- Build result.
- Recommended Git commit message.

Stop after Lesson 24.4.1 is fully completed and verified.
