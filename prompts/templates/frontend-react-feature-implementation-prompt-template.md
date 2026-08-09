Read AGENTS.md, ADR.md, DOMAIN_MODEL.md, ARCHITECTURE.md, and MEMORY.md.

Task: Implement [FEATURE_NAME] frontend feature.

## Feature

[SHORT_DESCRIPTION_OF_THE_FRONTEND_FEATURE]

The implementation must follow the existing frontend architecture, conventions, and design system defined in AGENTS.md and ADR 009.

## Before Implementation

- Inspect the existing frontend structure under `src/frontend/src/`, including related pages, components, routes, API hooks, types, and state management.
- Inspect the corresponding backend controllers and DTOs under `src/backend/CareerPulse.Api/Controllers/` and related application-layer code.
- Determine which frontend components are actually required for this feature.
- Reuse existing components, hooks, types, utilities, design tokens, and patterns wherever applicable.

## Implementation

Implement the complete frontend vertical slice required for [FEATURE_NAME].

Components may include, where applicable:

- TypeScript types/interfaces
- API functions or TanStack Query hooks
- Feature components
- Forms and dialogs/modals
- Page views
- Route integration

Do not create components that are not required by the feature.

### API & State

- Integrate the existing backend REST API through the existing Axios client.
- Use TanStack Query for server-state fetching, mutations, caching, and invalidation.
- Handle loading, empty, success, and RFC 7807 ProblemDetails error states.
- Follow existing mutation and cache-invalidation patterns.

### UI

- Follow the existing App Shell, responsive layout, light/dark theme, and design system.
- Reuse existing Shadcn UI components, Lucide icons, and design tokens.
- Follow existing form, validation, dialog, and modal patterns.
- Do not introduce new UI, routing, API, or state-management libraries.

## Scope

Implement only [FEATURE_NAME].

Do not expand the task into unrelated features.
Do not introduce a new architectural pattern when an existing project pattern already applies.

## Testing Boundary

Testing is a separate task and is explicitly OUT OF SCOPE.

Do NOT:

- create or modify test files;
- add test projects;
- create test components or test utilities;
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

Stop after [FEATURE_NAME] frontend implementation is fully completed and verified.