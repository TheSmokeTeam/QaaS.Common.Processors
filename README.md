# QaaS.Common.Processors

Deprecated repository placeholder for the retired `QaaS.Common.Processors` built-in processor package.

[![CI](https://github.com/TheSmokeTeam/QaaS.Common.Processors/actions/workflows/ci.yml/badge.svg)](https://github.com/TheSmokeTeam/QaaS.Common.Processors/actions/workflows/ci.yml)
[![Line Coverage](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/eldarush/b162a10f72beb3d3562978765ecc4d6c/raw/line-coverage-badge.json)](https://github.com/TheSmokeTeam/QaaS.Common.Processors/actions/workflows/ci.yml)
[![Branch Coverage](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/eldarush/b162a10f72beb3d3562978765ecc4d6c/raw/branch-coverage-badge.json)](https://github.com/TheSmokeTeam/QaaS.Common.Processors/actions/workflows/ci.yml)
[![Docs](https://img.shields.io/badge/docs-qaas--docs-blue)](https://thesmoketeam.github.io/qaas-docs/)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)

## Status

`QaaS.Common.Processors` no longer ships reusable transaction processors and no longer publishes a NuGet package.

The remaining assembly only exposes retirement metadata and migration guidance through `ProcessorRetirementCatalog`. It exists to lock the repo into its retired state, document the canonical framework status hook, and keep the future backlog explicit under test.

The canonical built-in status hook is now:

`QaaS.Framework.SDK.Hooks.BaseHooks.StatusCodeTransactionProcessor`

Use the fully qualified type name in mocker YAML when you need the built-in status hook, because `QaaS.Mocker` also contains an internal runtime-only `StatusCodeTransactionProcessor` for default 404/500 fallback stubs.

`QaaS.Mocker` internal fallback behavior was intentionally left unchanged by this retirement.

## Migration

Replace package usage with one of these patterns:

1. For a simple built-in status response, use the framework hook directly in YAML:

```yaml
Stubs:
  - Name: HealthStub
    Processor: QaaS.Framework.SDK.Hooks.BaseHooks.StatusCodeTransactionProcessor
    ProcessorConfiguration:
      StatusCode: 200
```

2. For any other behavior, author a local processor in the mocker project by inheriting from `BaseTransactionProcessor<TConfig>` in `QaaS.Framework.SDK`.

There is currently no successor shared processor package. Reusable processors must be authored locally until a replacement package is created.

## Backlog

The following processor candidates were identified from `QaaS.Runner` and `QaaS.Mocker` usage patterns. They are roadmap items only and are not implemented in this repository:

1. `StaticResponseProcessor`
2. `TransactionFromDataSources`
3. `RequestEchoProcessor`
4. `PassThroughProcessor`
5. `JsonTemplateProcessor`
6. `ConditionalResponseProcessor`
7. `SequenceProcessor`
8. `LatencyInjectionProcessor`
9. `ProblemDetailsProcessor`
10. `TextTransformProcessor`

## Build

The remaining solution is kept only for build, coverage, and deprecation validation. CI enforces 90%+ line and branch coverage over the retirement metadata and migration logic:

```bash
dotnet restore QaaS.Common.Processors.sln
dotnet build QaaS.Common.Processors.sln -c Release --no-restore
dotnet test QaaS.Common.Processors.sln -c Release --no-build
```

## Documentation

- Official docs: [thesmoketeam.github.io/qaas-docs](https://thesmoketeam.github.io/qaas-docs/)
- CI workflow: [`.github/workflows/ci.yml`](./.github/workflows/ci.yml)
