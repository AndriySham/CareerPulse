Read AGENTS.md, ADR.md, DOMAIN_MODEL.md, ARCHITECTURE.md, and MEMORY.md.

Task: Implement Applications & Kanban Pipeline frontend feature.

## Feature

Applications Management, Kanban Board Pipeline, and Status Machine Workflow UI

Implement the full frontend vertical slice for job application tracking and Kanban workflow under `src/frontend/src/`, connecting React 19, TypeScript, TanStack Query, and Axios to the existing backend REST API.

The implementation must follow the existing frontend architecture, conventions, and design system defined in AGENTS.md and ADR 009.

## Before Implementation

- Inspect the existing frontend structure under `src/frontend/src/`, including related pages, components, routes, API client, types, and existing UI patterns.
- Inspect the actual `ApplicationsController`, `Application` entity, `ApplicationStatus` and `ApplicationStatusMachine` implementation under `src/backend/`.
- Inspect the actual backend DTOs, validators, API routes, and supported application operations.
- Inspect existing Company, Vacancy, and ResumeRevision frontend API/hooks and reuse them where available.
- Determine which frontend components, API operations, and state are actually required for Applications & Kanban management.
- Reuse existing components, hooks, types, utilities, modal primitives, error handling, design tokens, and patterns wherever applicable.
- Do not assume API routes, DTO shapes, status values, or frontend conventions when they can be determined from the workspace.

## Implementation

Implement the complete frontend vertical slice required for Applications & Kanban Pipeline.

Components may include, where applicable:

- TypeScript types/interfaces under the existing `src/frontend/src/types/` structure, matching the actual backend DTOs and status model.
- API functions or TanStack Query hooks under the existing frontend API structure for the application operations actually exposed by the backend.
- Feature components under `src/frontend/src/components/applications/`, such as:
  - Kanban board and status columns;
  - Application cards;
  - Application submission form/modal;
  - Status transition controls where required.
- `ApplicationsPage.tsx` with:
  - application header/actions;
  - appropriate application filters supported by the backend;
  - Kanban pipeline based on the actual `ApplicationStatusMachine`;
  - loading, empty, and error states.
- Route integration only where required.

Do not create components or functionality that are not required by the actual backend API and existing frontend architecture.

### API & State

- Integrate the existing backend REST API through the existing Axios client.
- Use TanStack Query for server-state fetching, mutations, caching, and invalidation.
- Match frontend types and forms to the actual backend DTOs and validation rules.
- Reuse existing Company, Vacancy, and ResumeRevision data sources where available.
- Do not implement unrelated Company, Vacancy, Resume, or Analytics functionality as part of this feature.
- Handle loading, empty, success, and RFC 7807 ProblemDetails error states, including invalid status transitions.
- Invalidate or update the relevant TanStack Query caches after successful mutations.

### UI

- Follow the existing App Shell, responsive layout, Dracula dark theme, Alabaster light theme, and design system.
- Reuse existing Shadcn UI components, Lucide icons, design tokens, forms, dialogs, and modal patterns.
- Represent the actual application statuses defined by the backend status machine.
- Provide an intuitive way to move applications between valid statuses without bypassing backend validation.
- Do not introduce new UI, routing, API, form, or state-management libraries.

## Scope

Implement only Applications & Kanban Pipeline frontend feature.

Do not expand the task into unrelated features such as Analytics, Interview Management, Resume Management, or new backend functionality.

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

Stop after Applications & Kanban Pipeline frontend implementation is fully completed and verified.