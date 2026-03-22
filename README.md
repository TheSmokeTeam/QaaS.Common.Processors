# QaaS.Common.Processors

Reusable .NET transaction processors for QaaS mocker workflows.

[![CI](https://github.com/TheSmokeTeam/QaaS.Common.Processors/actions/workflows/ci.yml/badge.svg)](https://github.com/TheSmokeTeam/QaaS.Common.Processors/actions/workflows/ci.yml)
[![Line Coverage](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/eldarush/b162a10f72beb3d3562978765ecc4d6c/raw/line-coverage-badge.json)](https://github.com/TheSmokeTeam/QaaS.Common.Processors/actions/workflows/ci.yml)
[![Branch Coverage](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/eldarush/b162a10f72beb3d3562978765ecc4d6c/raw/branch-coverage-badge.json)](https://github.com/TheSmokeTeam/QaaS.Common.Processors/actions/workflows/ci.yml)
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
- `StaticResponseProcessor`: returns a configurable UTF-8 body, status code, content type, and additional response headers.
- `RequestEchoProcessor`: returns a JSON echo of the incoming body, request headers, path parameters, and request URI.
- `PassThroughProcessor`: returns the incoming payload unchanged while applying response status and headers, with optional metadata preservation.
- `StatusCodeTransactionProcessor`: returns an empty body with status code from `StatusCodeConfiguration`.

### [QaaS.Common.Processors.Tests](./QaaS.Common.Processors.Tests/)
- NUnit tests for all processors and core edge cases.
- Validation of response metadata, payload handling, and passthrough behavior.

## Quick Start
Install package:

```bash
dotnet add package QaaS.Common.Processors
```

Update package:

```bash
dotnet add package QaaS.Common.Processors --version 1.0.0
dotnet restore
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
