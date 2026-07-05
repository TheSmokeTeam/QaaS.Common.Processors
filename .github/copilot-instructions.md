# Copilot instructions — QaaS.Common.Processors

Read `AGENTS.md` at the repo root first — it tables all 9 processors and the Mocker/schema naming contracts.

Essentials:
- net10.0; NUnit tests; build `dotnet build -m`, test `dotnet test --no-build`.
- Processors implement `ITransactionProcessor` via `BaseTransactionProcessor<TConfig>`, run inside QaaS.Mocker request handling — keep them stateless and fast.
- Class and configuration-property names are referenced verbatim in user YAML and published mocker-family JSON schemas — renames are ecosystem-breaking.
- Versions are tag-driven (`VersionPrefix 0.0.0` + stable Git tags); CI measures line coverage on windows-latest (≥70% target, not a hard gate).
- Conventional commits; tests first.
