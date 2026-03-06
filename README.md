# QaaS.Common.Processors

Reusable .NET transaction processors for QaaS mocker workflows.

[![CI](https://img.shields.io/badge/CI-GitHub_Actions-2088FF)](./.github/workflows/ci.yml)
[![Docs](https://img.shields.io/badge/docs-qaas--docs-blue)](https://thesmoketeam.github.io/qaas-docs/)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)

## Contents
- [Overview](#overview)
- [Packages](#packages)
- [Functionalities](#functionalities)
- [Install and Upgrade](#install-and-upgrade)
- [Architecture](#architecture)
- [Documentation](#documentation)

## Overview
This repository contains one solution: [`QaaS.Common.Processors.sln`](./QaaS.Common.Processors.sln).

The solution includes:
- Runtime package project: [`QaaS.Common.Processors`](./QaaS.Common.Processors/)
- Test project: [`QaaS.Common.Processors.Tests`](./QaaS.Common.Processors.Tests/)

`QaaS.Common.Processors` provides reusable `BaseTransactionProcessor<TConfiguration>` implementations that can be plugged into QaaS execution pipelines.

## Packages
| Package | Latest Version | Total Downloads |
|---|---|---|
| [QaaS.Common.Processors](https://www.nuget.org/packages/QaaS.Common.Processors/) | [![NuGet](https://img.shields.io/nuget/v/QaaS.Common.Processors?logo=nuget)](https://www.nuget.org/packages/QaaS.Common.Processors/) | [![Downloads](https://img.shields.io/nuget/dt/QaaS.Common.Processors?logo=nuget)](https://www.nuget.org/packages/QaaS.Common.Processors/) |

## Functionalities
### [QaaS.Common.Processors](./QaaS.Common.Processors/)
- `ExampleProcessor`: returns a static UTF-8 response body with HTTP status `200`.
- `DummyTransactionProcessor`: converts request bytes to Base64, injects configured key/value pairs, and returns JSON metadata including request path parameters.
- `GrpcEchoProcessor`: resolves paired `*Request` / `*Response` types, echoes request `Message`, sets response `Code = 200`, and returns `byte[]` when `ToByteArray()` exists.
- `StatusCodeTransactionProcessor`: returns an empty body with status code from `StatusCodeConfiguration`.

### [QaaS.Common.Processors.Tests](./QaaS.Common.Processors.Tests/)
- NUnit tests for all processors and core edge cases.
- Validation of happy-path behavior and expected exception flows.

## Install and Upgrade
Install:

```bash
dotnet add package QaaS.Common.Processors
```

Upgrade:

```bash
dotnet add package QaaS.Common.Processors --version 1.0.0
dotnet restore
```

## Architecture
- Processor model: all runtime processors inherit from `BaseTransactionProcessor<TConfiguration>` from `QaaS.Framework.SDK`.
- Configuration boundary:
  - `NoConfiguration` is used by processors that do not require settings.
  - `DummyStubConfig` supplies required key/value fields for `DummyTransactionProcessor`.
  - `StatusCodeConfiguration` (from framework SDK) configures `StatusCodeTransactionProcessor`.
- Response patterns:
  - HTTP-oriented processors return `Data<object>` with `MetaData.Http` status and optional headers/body.
  - `GrpcEchoProcessor` resolves request/response pairs via reflection using `*Request` -> `*Response` naming.
  - If a response exposes `ToByteArray()`, `GrpcEchoProcessor` emits serialized `byte[]`; otherwise it returns the object directly.
- Test coverage of behavior:
  - One dedicated test class per processor validates normal flow and critical edge cases.
  - Test project depends on the runtime package project directly through `ProjectReference`.

Minimal usage example:

```csharp
var processor = new ExampleProcessor();
var response = processor.Process(ImmutableList<DataSource>.Empty, new Data<object>());
```

## Documentation
- Official docs: [thesmoketeam.github.io/qaas-docs](https://thesmoketeam.github.io/qaas-docs/)
- CI workflow: [`.github/workflows/ci.yml`](./.github/workflows/ci.yml)
- NuGet package: [QaaS.Common.Processors on NuGet](https://www.nuget.org/packages/QaaS.Common.Processors/)
