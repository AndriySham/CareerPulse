Read AGENTS.md, ADR.md, DOMAIN_MODEL.md, ARCHITECTURE.md, and MEMORY.md.

Execute unit testing for Company CQRS Use Cases.

## Feature Under Test

Company CQRS Use Cases & DTOs

Unit testing of Company management commands (CreateCompany, UpdateCompany), queries (GetCompanies, GetCompanyById), DTOs, and FluentValidation validators.

## Source of Truth

The implementation under test already exists in the workspace.

Relevant source location(s):

- src/backend/CareerPulse.Application/DTOs/Companies/
- src/backend/CareerPulse.Application/Features/Companies/

Relevant documentation, if applicable:

- AGENTS.md
- DOMAIN_MODEL.md
- ARCHITECTURE.md

Inspect the actual implementation before creating tests. The source code in the workspace is the primary source of truth.

## Test Project

Use the existing test project:

src/tests/CareerPulse.Application.Tests/

Before creating files, inspect the existing test structure and reuse its test infrastructure, helpers, fixtures, namespaces, and conventions where appropriate.

Do not create a duplicate test project or duplicate test infrastructure.

## Testing Task

Use `@dotnet-tester` to analyze Company CQRS Use Cases and create the appropriate unit tests for the existing implementation.

Relevant components include:

- DTOs: CompanyDto, CreateCompanyDto, UpdateCompanyDto
- Commands: CreateCompanyCommand & Handler & Validator, UpdateCompanyCommand & Handler & Validator
- Queries: GetCompaniesQuery & Handler, GetCompanyByIdQuery & Handler
- Domain Entity: Company

Determine the actual test coverage from the implementation rather than from this prompt.

Do not prescribe individual test cases unless a specific business requirement must be explicitly verified.

## Scope

Test only:

Company CQRS Use Cases (Create, Update, GetCompanies, GetCompanyById) under CareerPulse.Application.

Do not expand the task to unrelated features.

## Testing Type

This task is limited to unit tests.

Do NOT create integration tests, API controller tests, or frontend tests.

## Verification

Complete and verify the resulting test suite according to the existing `@dotnet-tester` testing workflow:

- Run: dotnet test "src/backend/CareerPulse.sln" -c Release
- Run: dotnet build "src/backend/CareerPulse.sln" -c Release

## Completion

Return the standard `@dotnet-tester` test report.

Stop after Company CQRS unit testing is fully completed and verified.
