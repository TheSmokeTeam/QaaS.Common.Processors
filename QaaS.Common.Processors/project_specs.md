# project_specs.md — QaaS.Common.Processors (package project)

Single package project; processors grouped by category folder.

## Folders

- `Static/` — `StaticResponseProcessor`, `StatusCodeTransactionProcessor`.
- `RequestInspection/` — `RequestEchoProcessor`, `PassThroughProcessor`.
- `Error/` — `ProblemDetailsProcessor`.
- `Transformation/` — `TextTransformProcessor`, `JsonEnvelopeProcessor`.
- `Conditional/` — `ConditionalResponseProcessor`,
  `DataSourceResponseProcessor`.
- `ConfigurationObjects/` — config records.

## Forbidden

- Adding non-processor hook implementations.
- Maintaining state across requests; processors must be re-entrant.
