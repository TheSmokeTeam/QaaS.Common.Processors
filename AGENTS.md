# AGENTS.md — QaaS.Common.Processors

Guidance for AI agents working in this repository.

## What this repo is

The stock **`ITransactionProcessor` hook library** for the QaaS platform: 9 reusable processors that run inside **QaaS.Mocker** workflows to shape transaction requests and forge mock server responses. Processors inherit `BaseTransactionProcessor<TConfig>` (from `QaaS.Framework.SDK`) and are discovered at runtime by Framework assembly scanning (order: `QaaS.*` → `Common.*` → user assemblies). Tier-1; ships as the `QaaS.Common.Processors` NuGet package.

## Shipped processors

| Processor | Category | Behavior |
|---|---|---|
| StaticResponseProcessor | Static | configured hardcoded body, status, content type, headers |
| StatusCodeTransactionProcessor | Static | empty response with configured HTTP status |
| RequestEchoProcessor | Inspection | JSON document echoing request body, path params, headers, URI |
| PassThroughProcessor | Inspection | forwards request untouched, overlays configured headers/status |
| ProblemDetailsProcessor | Error | RFC 7807 error envelope (status, title, instance, details, extensions) |
| TextTransformProcessor | Transform | trim + string-replacement rules + prefix/suffix wrapping |
| JsonEnvelopeProcessor | Transform | wraps body in customizable JSON envelope (query, path, headers) |
| ConditionalResponseProcessor | Conditional | first-matching header/path rule executes; falls back otherwise |
| DataSourceResponseProcessor | Conditional | resolves a QaaS data source; yields first/last/exact-index records |

## Build & test

```powershell
dotnet restore
dotnet build -m --no-restore
dotnet test --no-build          # NUnit, QaaS.Common.Processors.Tests
```

## Critical gotchas

- **Naming is contract**: processor class names are referenced verbatim in Mocker YAML (`Processor: StaticResponseProcessor`) and baked into the mocker-family JSON schema (via QaaS.JsonSchemaExtensions → QaaS.PackageMirror) — renames break every consumer and the docs.
- Configuration class properties feed the published schema — renames break user editor validation.
- These run inside Mocker's request path: keep processors **stateless and fast**; behavior changes affect every stubbed endpoint users have configured.
- Versioning is tag-driven (`VersionPrefix 0.0.0` + stable `X.X.X` Git tags in CI). Don't hand-edit versions.
- CI (windows-latest) measures and reports line coverage via dotnet-coverage; ≥70% is the target but is not enforced as a hard build gate.

## Process

Non-trivial changes follow the QaaS harness pipeline: plan → contract → implement → adversarial evaluation (rubric: correctness, completeness, craft, robustness — each ≥7/10). Write failing NUnit tests first; never mock the class under test. Conventional commits.
