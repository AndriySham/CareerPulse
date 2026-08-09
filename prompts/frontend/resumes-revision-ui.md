Read AGENTS.md, ADR.md, DOMAIN_MODEL.md, ARCHITECTURE.md, and MEMORY.md.

Task: Implement Resumes & Revisions frontend feature.

## Feature

Resume Profiles, Resume Revisions, and Version Snapshot UI

Implement the full frontend vertical slice required for resume profile and revision management under `src/frontend/src/`, connecting React 19, TypeScript, TanStack Query, and Axios to the existing backend REST API.

The implementation must follow the existing frontend architecture, conventions, and design system defined in AGENTS.md and ADR 005/009.

## Before Implementation

- Inspect the existing frontend structure under `src/frontend/src/`, including related pages, components, routes, API client, types, and existing UI patterns.
- Inspect the actual backend controller(s), domain entities, DTOs, validators, and revision/versioning logic related to resumes.
- Verify the actual relationship between ResumeProfile/Resume/ResumeRevision in the current implementation instead of assuming entity names from documentation.
- Inspect the existing implementation of ADR 005, especially Draft immutability and Copy-on-Write revision rules.
- Inspect existing UI components, `CustomSelect.tsx`, `Modal.tsx`, `ErrorAlert.tsx`, and design tokens.
- Determine which frontend components, API operations, and state are actually required for resume and revision management.
- Reuse existing components, hooks, types, utilities, modal primitives, design tokens, and patterns wherever applicable.
- Do not assume API routes, DTO shapes, revision fields, or frontend conventions when they can be determined from the workspace.

## Implementation

Implement the complete frontend vertical slice required for Resume Profiles & Revisions management.

Components may include, where applicable:

- TypeScript types/interfaces under the existing `src/frontend/src/types/` structure, matching the actual backend DTOs.
- API functions or TanStack Query hooks under the existing frontend API structure for operations actually exposed by the backend.
- Feature components under `src/frontend/src/components/resumes/`, such as:
  - Resume/profile cards;
  - Resume creation/editing forms;
  - Resume detail view;
  - Revision history/version view;
  - Draft revision creation controls.
- `ResumesPage.tsx` with:
  - appropriate creation/action controls;
  - search/filtering where appropriate;
  - resume/profile list;
  - loading, empty, and error states.
- Route integration only where required.

The UI must reflect the actual backend resume/revision workflow and ADR 005.

Do not create components or functionality that are not required by the actual backend API and existing frontend architecture.

### API & State

- Integrate the existing backend REST API through the existing Axios client.
- Use TanStack Query for server-state fetching, mutations, caching, and invalidation.
- Match frontend types and forms to the actual backend DTOs and validation rules.
- Reuse existing API/hooks and frontend data sources wherever available.
- Respect ResumeRevision immutability and Draft/Copy-on-Write rules defined by the backend and ADR 005.
- Do not allow the UI to modify an immutable revision.
- Handle loading, empty, success, and RFC 7807 ProblemDetails error states.
- Invalidate or update the relevant TanStack Query caches after successful mutations.

### UI

- Follow the existing App Shell, responsive layout, Dracula dark theme, Alabaster light theme, and design system.
- Reuse existing Shadcn UI components, Lucide icons, `CustomSelect.tsx`, and design tokens.
- Follow existing form, validation, dialog, and modal patterns.
- Represent revision/version information according to the actual backend model.
- Clearly distinguish editable Draft revisions from immutable revisions linked to Applications.
- Do not introduce new UI, routing, API, form, or state-management libraries.

## Scope

Implement only Resumes & Revisions frontend feature.

Do not expand the task into unrelated features such as AI resume import, AI tailoring, PDF generation/export, file-storage implementation, Analytics, or new backend functionality.

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

Stop after Resumes & Revisions frontend implementation is fully completed and verified.