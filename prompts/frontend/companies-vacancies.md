
Read AGENTS.md, ADR.md, DOMAIN_MODEL.md, ARCHITECTURE.md, and MEMORY.md.

Task: Implement Companies and Vacancies frontend feature.

## Feature

Companies and Vacancies Management UI (List Views, Search Filters, Detail Cards, and Modal Dialog Forms)

Implement the full frontend vertical slice for managing Companies and Vacancies under `src/frontend/`, connecting React 19, TypeScript, TanStack Query, and Axios to the existing backend REST API.

The implementation must follow the existing frontend architecture, conventions, and design system defined in AGENTS.md and ADR 009.

## Before Implementation

- Inspect the existing frontend structure under `src/frontend/src/`, including related pages, components, routes, API client, types, and existing UI patterns.
- Inspect the corresponding backend controllers, DTOs, validators, and API routes for Companies and Vacancies.
- Determine which frontend components, API operations, and state are actually required by the existing implementation.
- Reuse existing components, hooks, types, utilities, design tokens, and patterns wherever applicable.
- Do not assume API routes, DTO shapes, or frontend conventions when they can be determined from the workspace.

## Implementation

Implement the complete frontend vertical slice required for Companies and Vacancies.

Components may include, where applicable:

- TypeScript types/interfaces under `src/frontend/src/types/`
- API functions or TanStack Query hooks under the existing frontend API structure
- Feature components under `src/frontend/src/components/companies/` and `src/frontend/src/components/vacancies/`
- Companies and Vacancies page views
- Route integration only where required

The implementation should support the business operations exposed by the existing backend API. Do not implement unsupported operations such as deletion unless they already exist in the backend and are required by the feature.

### API & State

- Integrate the existing backend REST API through the existing Axios client.
- Use TanStack Query for server-state fetching, mutations, caching, and invalidation.
- Match frontend types and forms to the actual backend DTOs and validation rules.
- Handle loading, empty, success, and RFC 7807 ProblemDetails error states.
- Invalidate or update the relevant TanStack Query caches after successful mutations.
- Keep API communication outside page and presentation components.

### UI

- Follow the existing App Shell, responsive layout, Dracula dark theme, Alabaster light theme, and design system.
- Reuse existing Shadcn UI components, Lucide icons, design tokens, forms, dialogs, and modal patterns.
- Implement search and filtering according to the existing API capabilities and frontend architecture. Prefer the simplest appropriate approach based on the actual data and API.
- Do not introduce new UI, routing, API, form, or state-management libraries.

## Scope

Implement only Companies and Vacancies frontend feature.

Do not expand the task into unrelated features.
Do not introduce a new architectural pattern when an existing project pattern already applies.

## Testing Boundary

Testing is a separate task and is explicitly OUT OF SCOPE.

Do NOT:

- create or modify test files;
- add test projects;
- create test utilities;
- generate unit, integration, or E2E tests;
- delegate testing work to `@react-tester` or another testing subagent.

You may only run the existing frontend test suite if appropriate, without modifying test-related files.

## Verification

Run:

- `cd src/frontend && npm run build`
- `cd src/frontend && npm run lint`

Verify that the frontend builds successfully and linting passes.

## Completion Report

Report:

- files created or modified;
- implemented functionality, components, and routes;
- API integration;
- verification result;
- architectural or implementation issues discovered;
- recommended Git commit message.

Stop after Companies and Vacancies frontend implementation is fully completed and verified.