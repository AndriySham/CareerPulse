Read AGENTS.md, ADR.md, DOMAIN_MODEL.md, ARCHITECTURE.md, and MEMORY.md.

Task: Implement [FEATURE_NAME] backend feature.

## Feature

[SHORT_DESCRIPTION_OF_THE_FEATURE]

The implementation must follow the existing project architecture and conventions.

Before implementation:
- inspect the domain model and existing implementations of similar features;
- determine which components are actually required for this feature;
- reuse existing abstractions and infrastructure where applicable.

## Implementation

Implement the complete backend vertical slice required for [FEATURE_NAME].

Components may include, where applicable:

- DTOs
- Domain changes, only if required
- Commands and Command Handlers
- Queries and Query Handlers
- FluentValidation validators
- REST API Controller
- OpenAPI metadata

Do not create components that are not required by the feature.

### API

Expose the required REST endpoints for [FEATURE_NAME] according to the existing API conventions.

Use:
- MediatR for application requests;
- existing global error handling;
- existing RFC 7807 / ProblemDetails conventions;
- existing OpenAPI conventions.

## Scope

Implement only [FEATURE_NAME].

Do not expand the task into unrelated features.

Do not introduce a new architectural pattern when an existing project pattern already applies.

Testing is a separate task.
Do not create or modify test files as part of this feature implementation.
Only run existing tests to verify that the solution remains green.

## Verification

Verify that the implementation integrates correctly with the existing solution.

## Completion Report

Report:

- files created or modified;
- implemented functionality and endpoints;
- verification result;
- architectural or implementation issues discovered;
- recommended Git commit message.

Stop after [FEATURE_NAME] backend implementation is fully completed and verified.