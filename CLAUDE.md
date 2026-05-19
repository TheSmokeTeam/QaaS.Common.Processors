# CLAUDE.md — QaaS.Common.Processors

> Operating manual; see `project_specs.md`. Live docs:
> <https://docs.qaas.online/processors/>.

## Mission

Pre-built `ITransactionProcessor` implementations for `QaaS.Mocker`:
static responses, request echoes, error envelopes, transformations,
conditional routing, data-source-driven responses.

## Build / Test

```bash
dotnet build QaaS.Common.Processors.sln --nologo -clp:ErrorsOnly
dotnet test  QaaS.Common.Processors.sln --nologo --no-build
csharpier format <changed-files>
```

## Shipped processors

- **Static (2):** `StaticResponseProcessor` (UTF-8 body + status code +
  headers), `StatusCodeTransactionProcessor` (status only).
- **Request inspection (2):** `RequestEchoProcessor` (echo as JSON),
  `PassThroughProcessor`.
- **Error / status (1):** `ProblemDetailsProcessor` (RFC 7807).
- **Transformation (2):** `TextTransformProcessor`,
  `JsonEnvelopeProcessor`.
- **Conditional / data-driven (2):** `ConditionalResponseProcessor`
  (first match wins, fallback), `DataSourceResponseProcessor`
  (first/last/index selection with fallback).

Total: 9 processors.

## Conventions

- Every processor inherits `BaseTransactionProcessor<TConfig>`.
- Status codes constrained by `[Range(100, 599)]`.
- Response headers: case-insensitive dictionary
  (`StringComparer.OrdinalIgnoreCase`).
- Configurations are records with DataAnnotations.

## Forbidden

1. Return null `Body` without intent — handle explicitly.
2. Mutate `DataSource` list (immutable contract).
3. Override `Content-Type` silently — log when changed.
4. Reorder conditional rules — first match wins; preserve user order.
5. Transform binary as text without checking ContentType.
6. Nest JSON envelopes infinitely.
7. Serialise non-Newtonsoft-compatible objects.

## Must-verify

1. `dotnet build` / `dotnet test` green.
2. Framework SDK 1.4.2 (`csproj:21`).
3. Status code range 100-599.
4. RFC 7807 problem details serialise as JSON.
5. CI green (with strict NuGet metadata validation: PackagePath="/",
   `..\README.md` linked).

## Recent

- PR #16 (`feature/docs-claude`) — CLAUDE.md drop.
- Latest: framework 1.4.2.
