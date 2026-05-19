# project_specs.md — QaaS.Common.Processors

9 pre-built `ITransactionProcessor` implementations consumed by
`QaaS.Mocker` stubs.

## Categories

- **Static responses (2):** `StaticResponseProcessor`,
  `StatusCodeTransactionProcessor`.
- **Request inspection (2):** `RequestEchoProcessor`,
  `PassThroughProcessor`.
- **Error / status (1):** `ProblemDetailsProcessor` (RFC 7807).
- **Transformation (2):** `TextTransformProcessor`,
  `JsonEnvelopeProcessor`.
- **Conditional / data-driven (2):** `ConditionalResponseProcessor`,
  `DataSourceResponseProcessor`.

## Public surface

- `BaseTransactionProcessor<TConfig>` inheritance root.
- `ProcessorResponseFactory` for response polymorphism.

## Build, packaging, CI

- Target: `.NET 10.0`. NuGet: `QaaS.Common.Processors`.
- CI: standard pipeline + strict NuGet metadata validation (453 lines):
  `PackageReadmeFile = README.md`, README linked with
  `Include="..\README.md"`, `Pack=true`, `PackagePath="/"`,
  `Link=README.md`.

## References

- Live docs: <https://docs.qaas.online/processors/>
