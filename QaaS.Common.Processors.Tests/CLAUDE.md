# CLAUDE.md — QaaS.Common.Processors.Tests

> Test operating manual. See repo root `CLAUDE.md`.

## Purpose

One test class per processor, covering happy-path, status range
boundaries, header case-insensitivity, conditional ordering, and
data-source selection semantics.

## Layout

- `StaticResponseProcessorTests.cs`,
  `StatusCodeTransactionProcessorTests.cs`,
  `RequestEchoProcessorTests.cs`, `PassThroughProcessorTests.cs`,
  `ProblemDetailsProcessorTests.cs`,
  `TextTransformProcessorTests.cs`, `JsonEnvelopeProcessorTests.cs`,
  `ConditionalResponseProcessorTests.cs`,
  `DataSourceResponseProcessorTests.cs`.
- `Globals.cs` — minimal shared `Context` with
  `Logger = NullLogger.Instance` and an empty `ConfigurationBuilder`
  root (see `Globals.cs:8-12`).
- `AssemblyInfo.cs` — assembly-level test attributes.

## Conventions

- **NUnit**. No Moq required — processors take simple records;
  prefer hand-built `Transaction` / `Context` instances.
- `Globals.Context` is the canonical `Context` for unit tests; reuse it
  rather than newing up another `NullLogger` chain.
- Assert headers using `OrdinalIgnoreCase` lookups — processors store
  them that way.
- For conditional / data-source tests, exercise: first match, no
  match → fallback, exact selector boundaries.
- Status code tests must hit the `[Range(100, 599)]` boundaries.

## Forbidden

1. `[Test(Ignore=...)]` / `[Explicit]` to dodge a red test — diagnose.
2. Reordering rules in `ConditionalResponseProcessorTests` to make a
   broken implementation pass.
3. Asserting header lookups with case-sensitive comparers.
4. Real network I/O.
5. Leaking shared mutable state between tests.

## Run

```bash
dotnet test ../QaaS.Common.Processors.sln --nologo --no-build
```
