Read AGENTS.md, ADR.md, DOMAIN_MODEL.md, ARCHITECTURE.md, and MEMORY.md.

Task: Implement Application & Kanban Pipeline backend feature.

## Feature

Application Management, Kanban Pipeline, and Status Machine Workflow

Implement the complete vertical backend slice for job applications linking Vacancy, ResumeRevision, and Company. Enforce status transition rules defined by ApplicationStatusMachine.

The implementation must follow the existing project architecture and conventions.

Before implementation:
- inspect the domain model (Application.cs, ApplicationStatusMachine.cs, Vacancy.cs, ResumeRevision.cs);
- inspect existing implementations of similar features (e.g. CompaniesController and VacanciesController);
- determine which components are actually required for this feature;
- reuse existing abstractions and infrastructure where applicable.

## Implementation

Implement the complete backend vertical slice required for Application & Kanban Pipeline.

Components may include, where applicable:

- DTOs: ApplicationDto (Id, VacancyId, VacancyTitle, CompanyName, ResumeRevisionId, Status, AppliedAt, CreatedAt), SubmitApplicationDto, ChangeApplicationStatusDto
- Domain changes, only if required
- Commands and Command Handlers: SubmitApplicationCommand & Handler (creates Application entity linking Vacancy and ResumeRevision), ChangeApplicationStatusCommand & Handler (mutates status via ApplicationStatusMachine.TransitionTo())
- Queries and Query Handlers: GetApplicationsQuery & Handler (supporting optional status and VacancyId filters with AsNoTracking()), GetApplicationByIdQuery & Handler (with AsNoTracking())
- FluentValidation validators: SubmitApplicationCommandValidator, ChangeApplicationStatusCommandValidator
- REST API Controller: ApplicationsController.cs under CareerPulse.Api/Controllers/
- OpenAPI metadata: ProducesResponseType annotations for success and applicable RFC 7807 ProblemDetails status codes

Do not create components that are not required by the feature.

### API

Expose the required REST endpoints for Application & Kanban Pipeline according to the existing API conventions:

- POST /api/applications -> SubmitApplicationCommand -> 201 Created (via CreatedAtAction)
- PUT /api/applications/{id:guid}/status -> ChangeApplicationStatusCommand -> 200 OK
- GET /api/applications -> GetApplicationsQuery -> 200 OK
- GET /api/applications/{id:guid} -> GetApplicationByIdQuery -> 200 OK | 404 NotFound

Use:
- MediatR for application requests;
- existing global error handling (GlobalExceptionMiddleware);
- existing RFC 7807 / ProblemDetails conventions;
- existing OpenAPI conventions.

## Scope

Implement only Application & Kanban Pipeline backend feature.

Do not expand the task into unrelated features.

Do not introduce a new architectural pattern when an existing project pattern already applies.

Testing is a separate task.
Do not create or modify test files as part of this feature implementation.
Only run existing tests to verify that the solution remains green.

## Verification

Run:
- dotnet build "src/backend/CareerPulse.sln" -c Release

Verify that the implementation integrates correctly with the existing solution.

## Completion Report

Report:

- files created or modified;
- implemented functionality and endpoints;
- verification result;
- architectural or implementation issues discovered;
- recommended Git commit message.

Stop after Application & Kanban Pipeline backend implementation is fully completed and verified.
