# QaaS.Common.Processors

Reusable .NET transaction processors for QaaS mocker workflows.

[![CI](https://img.shields.io/badge/CI-GitHub_Actions-2088FF)](./.github/workflows/ci.yml)
[![Docs](https://img.shields.io/badge/docs-qaas--docs-blue)](https://thesmoketeam.github.io/qaas-docs/)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)

## Contents
- [Overview](#overview)
- [Packages](#packages)
- [Functionalities](#functionalities)
- [Quick Start](#quick-start)
- [Build and Test](#build-and-test)
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

## Quick Start
Install package:

```bash
dotnet add package QaaS.Common.Processors
```

Minimal usage:

```csharp
var processor = new ExampleProcessor();
var response = processor.Process(ImmutableList<DataSource>.Empty, new Data<object>());
```

Configuration-based processor example:

```csharp
var processor = new StatusCodeTransactionProcessor
{
    Configuration = new StatusCodeConfiguration { StatusCode = 418 }
};

var response = processor.Process(ImmutableList<DataSource>.Empty, new Data<object>());
```

## Build and Test
```bash
dotnet restore QaaS.Common.Processors.sln
dotnet build QaaS.Common.Processors.sln -c Release --no-restore
dotnet test QaaS.Common.Processors.sln -c Release --no-build
```

## Documentation
- Official docs: [thesmoketeam.github.io/qaas-docs](https://thesmoketeam.github.io/qaas-docs/)
- CI workflow: [`.github/workflows/ci.yml`](./.github/workflows/ci.yml)
- NuGet package: [QaaS.Common.Processors on NuGet](https://www.nuget.org/packages/QaaS.Common.Processors/)
