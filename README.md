# QaaS.Common.Processors

[![CI](https://img.shields.io/github/actions/workflow/status/eldarush/QaaS.Common.Processors/ci.yml?label=CI)](./.github/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/QaaS.Common.Processors?label=NuGet)](https://www.nuget.org/packages/QaaS.Common.Processors)
[![NuGet Downloads](https://img.shields.io/nuget/dt/QaaS.Common.Processors?label=NuGet%20Downloads)](https://www.nuget.org/packages/QaaS.Common.Processors)
[![Coverage QaaS.Common.Processors](https://img.shields.io/badge/Coverage%20%28QaaS.Common.Processors%29-96.49%25-brightgreen)](#test-coverage)
[![Coverage QaaS.Common.Processors.Tests](https://img.shields.io/badge/Coverage%20%28QaaS.Common.Processors.Tests%29-99.19%25-brightgreen)](#test-coverage)
[![Docs](https://img.shields.io/badge/docs-qaas--docs-blue)](https://thesmoketeam.github.io/qaas-docs/)

Reusable transaction processors for QaaS mocker workflows.

- Language: C# 14
- Target Framework: .NET 10 (`net10.0`)
- Package: `QaaS.Common.Processors`
- Official docs: [QaaS Documentation](https://thesmoketeam.github.io/qaas-docs/)

## Table of Contents

- [What Is Included](#what-is-included)
- [Packages and Badges](#packages-and-badges)
- [Processor Catalog](#processor-catalog)
- [Installation](#installation)
- [Usage](#usage)
- [Test Coverage](#test-coverage)
- [Build, Test, and Pack](#build-test-and-pack)
- [Release Flow](#release-flow)

## What Is Included

This solution contains one runtime package project and one test project.

| Project | Type | Purpose |
| --- | --- | --- |
| `QaaS.Common.Processors` | Class library / NuGet package | Provides ready-to-use `BaseTransactionProcessor<TConfig>` implementations. |
| `QaaS.Common.Processors.Tests` | NUnit test project | Verifies runtime behavior and edge cases for each processor. |

## Packages and Badges

| Package | Latest Version | Total Downloads | NuGet |
| --- | --- | --- | --- |
| `QaaS.Common.Processors` | ![NuGet](https://img.shields.io/nuget/v/QaaS.Common.Processors?label=) | ![Downloads](https://img.shields.io/nuget/dt/QaaS.Common.Processors?label=) | [nuget.org/packages/QaaS.Common.Processors](https://www.nuget.org/packages/QaaS.Common.Processors) |

## Processor Catalog

| Processor | Configuration | Behavior |
| --- | --- | --- |
| `ExampleProcessor` | `NoConfiguration` | Returns static UTF-8 payload (`Hello world! This is an example :)`) and HTTP `200`. |
| `DummyTransactionProcessor` | `DummyStubConfig` | Accepts byte[] request body, returns JSON with configured key/value, base64 payload, and request path params. |
| `GrpcEchoProcessor` | `NoConfiguration` | Reflects request/response types that follow `*Request` and `*Response` naming, echoes `Message`, sets `Code = 200`, supports `ToByteArray()` response serialization. |
| `StatusCodeTransactionProcessor` | `StatusCodeConfiguration` | Returns empty body with configured HTTP status code. |

## Installation

```bash
dotnet add package QaaS.Common.Processors
```

## Usage

### ExampleProcessor

```csharp
var processor = new ExampleProcessor();
var result = processor.Process(ImmutableList<DataSource>.Empty, new Data<object>());
```

### DummyTransactionProcessor

```csharp
var processor = new DummyTransactionProcessor
{
    Configuration = new DummyStubConfig
    {
        DummyKey = "KeyA",
        DummyValue = "ValueA"
    }
};

var result = processor.Process(ImmutableList<DataSource>.Empty, new Data<object>
{
    Body = Encoding.UTF8.GetBytes("abc")
});
```

### GrpcEchoProcessor

```csharp
var processor = new GrpcEchoProcessor();
var result = processor.Process(
    ImmutableList<DataSource>.Empty,
    new Data<object> { Body = new EchoRequest { Message = "hello" } });
```

### StatusCodeTransactionProcessor

```csharp
var processor = new StatusCodeTransactionProcessor
{
    Configuration = new StatusCodeConfiguration { StatusCode = 418 }
};

var result = processor.Process(ImmutableList<DataSource>.Empty, new Data<object>());
```

## Test Coverage

Coverage snapshot was generated on `2026-03-06` via `XPlat Code Coverage` (Cobertura format).

| Project | Line Coverage | Badge |
| --- | --- | --- |
| `QaaS.Common.Processors` | `96.49%` | ![Coverage QaaS.Common.Processors](https://img.shields.io/badge/Coverage%20%28QaaS.Common.Processors%29-96.49%25-brightgreen) |
| `QaaS.Common.Processors.Tests` | `99.19%` | ![Coverage QaaS.Common.Processors.Tests](https://img.shields.io/badge/Coverage%20%28QaaS.Common.Processors.Tests%29-99.19%25-brightgreen) |

To regenerate:

```bash
dotnet test QaaS.Common.Processors.sln --configuration Release --collect:"XPlat Code Coverage" --results-directory TestResults -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.IncludeTestAssembly=true
```

## Build, Test, and Pack

```bash
dotnet restore QaaS.Common.Processors.sln
dotnet build QaaS.Common.Processors.sln --configuration Release --no-restore
dotnet test QaaS.Common.Processors.sln --configuration Release --no-restore
dotnet pack QaaS.Common.Processors/QaaS.Common.Processors.csproj --configuration Release --no-build
```

## Release Flow

- CI workflow: [`.github/workflows/ci.yml`](./.github/workflows/ci.yml)
- NuGet publish is triggered on git tags that match SemVer validation in CI.
- Package readme inside NuGet is sourced from this root `README.md`.
