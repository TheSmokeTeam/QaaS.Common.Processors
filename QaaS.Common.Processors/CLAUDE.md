# CLAUDE.md — QaaS.Common.Processors (library)

> Project-level operating manual. See repo root `CLAUDE.md` and
> `project_specs.md`.

## Purpose

Pre-built `ITransactionProcessor` implementations for `QaaS.Mocker`:
static responses, request echoes, RFC 7807 error envelopes, text /
JSON transformations, conditional routing, data-source-driven responses.

## Source files (9 processors)

- `StaticResponseProcessor.cs` — UTF-8 body + status + headers.
- `StatusCodeTransactionProcessor.cs` — status only.
- `RequestEchoProcessor.cs` — echo as JSON.
- `PassThroughProcessor.cs`.
- `ProblemDetailsProcessor.cs` — RFC 7807.
- `TextTransformProcessor.cs`, `JsonEnvelopeProcessor.cs`.
- `ConditionalResponseProcessor.cs` — first-match-wins + fallback.
- `DataSourceResponseProcessor.cs` — first / last / index selection
  with fallback.
- `ProcessorResponseFactory.cs` — shared response construction.
- `ConfigurationObjects/` — record configs with DataAnnotations.

## Conventions

- Every processor inherits `BaseTransactionProcessor<TConfig>`.
- Status codes constrained by `[Range(100, 599)]`.
- Response headers stored in a case-insensitive dictionary
  (`StringComparer.OrdinalIgnoreCase`).
- Configurations are records with DataAnnotations validation.
- Conditional rules evaluate in user-declared order; never reorder.

## Forbidden

1. Returning a null `Body` without intent — handle explicitly.
2. Mutating the `DataSource` list (immutable contract).
3. Silently overriding `Content-Type` — log on change.
4. Reordering conditional rules — first match wins.
5. Transforming binary bodies as text without checking `ContentType`.
6. Nesting JSON envelopes infinitely (cap depth).
7. Serialising types incompatible with the wire serialiser in use.

## Build

```bash
dotnet build ../QaaS.Common.Processors.sln --nologo -clp:ErrorsOnly
csharpier format <changed-files>
```

Framework SDK 1.4.2 alignment via `Directory.Build.props`. CI enforces
strict NuGet metadata (`PackagePath="/"`, README linked).
