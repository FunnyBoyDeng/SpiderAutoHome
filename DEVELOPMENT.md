# Development Guide

This document describes the current development status and project structure of SpiderAutoHome.

> **Important:** The repository currently contains a legacy .NET Core 2.0 codebase. .NET Core 2.0 is no longer supported. The instructions below primarily document the historical project structure while modernization work is in progress.

## Repository Structure

The solution contains three console application projects:

```text
SpiderAutoHome.sln
├── SpiderAutoHome/
│   └── SpiderAutoHome.csproj
├── SpiderAutoSkuData/
│   └── SpiderAutoSkuData.csproj
└── SpiderAutoLogo/
    └── SpiderAutoLogo.csproj
```

### SpiderAutoHome

Example project for automobile shop data extraction.

Current target framework:

```text
netcoreapp2.0
```

Legacy dependencies:

```text
DotnetSpider.Core      2.5.0
DotnetSpider.Extension 2.5.0
```

### SpiderAutoSkuData

Example project for automobile product/detail data extraction.

Current target framework:

```text
netcoreapp2.0
```

Legacy dependencies:

```text
DotnetSpider.Core      2.5.0
DotnetSpider.Extension 2.5.0
```

### SpiderAutoLogo

Example project for automobile brand and logo data extraction.

Current target framework:

```text
netcoreapp2.0
```

Legacy dependencies:

```text
DotnetSpider.Core      2.5.1
DotnetSpider.Extension 2.5.0
```

## Clone the Repository

```bash
git clone https://github.com/FunnyBoyDeng/SpiderAutoHome.git
cd SpiderAutoHome
```

## Legacy Development Environment

The original project was created with tooling from the .NET Core 2.0 era.

Because .NET Core 2.0 is end-of-life, developers should not assume that the current source code will build successfully with a modern .NET SDK without modification.

The repository is currently being reviewed before selecting a supported .NET target for the modernization branch.

For this reason, contributors should avoid making broad framework or dependency upgrades without first discussing the migration approach in an issue.

## Historical Build Commands

With a compatible legacy .NET Core environment, the solution used the standard .NET CLI workflow:

```bash
dotnet restore
dotnet build SpiderAutoHome.sln
```

Individual projects can also be referenced directly:

```bash
dotnet build SpiderAutoHome/SpiderAutoHome.csproj
dotnet build SpiderAutoSkuData/SpiderAutoSkuData.csproj
dotnet build SpiderAutoLogo/SpiderAutoLogo.csproj
```

These commands document the expected project structure. They are not currently guaranteed to work with modern .NET SDK releases.

## Running a Project

With a compatible environment, a project can be started using:

```bash
dotnet run --project SpiderAutoHome/SpiderAutoHome.csproj
```

or:

```bash
dotnet run --project SpiderAutoSkuData/SpiderAutoSkuData.csproj
```

or:

```bash
dotnet run --project SpiderAutoLogo/SpiderAutoLogo.csproj
```

Again, the current legacy code may require modernization before it runs successfully on a supported .NET SDK.

## Current Development Status

Maintenance resumed in 2026.

The current development process is divided into two stages.

### Stage 1 — Repository Maintenance

This stage focuses on:

* documentation;
* licensing;
* contribution guidelines;
* historical issue review;
* development environment documentation.

### Stage 2 — Code Modernization

The next stage will focus on:

* reviewing the current source and dependencies;
* selecting a supported .NET target;
* evaluating the current DotnetSpider API;
* migrating legacy dependencies;
* restoring successful builds;
* adding automated tests;
* adding CI.

Modernization progress is tracked in the repository Roadmap issue.

## Before Making Code Changes

Before modifying framework targets or DotnetSpider dependencies:

1. Check the current Roadmap issue.
2. Open or reference an issue describing the proposed change.
3. Keep migration changes focused and reviewable.
4. Record compatibility or behavior changes.
5. Do not describe untested migration work as production-ready.

See [CONTRIBUTING.md](CONTRIBUTING.md) for the general contribution workflow.

## Responsible Development

This project is intended for educational and research purposes.

Development and testing should use data and systems that contributors are authorized to access.

Do not add functionality specifically intended to bypass authentication, access controls, anti-abuse protections, rate limits, or other technical restrictions.

## Modernization Notes

The supported .NET target for the modernized version has not yet been selected.

Until dependency compatibility has been reviewed, the repository will continue to clearly distinguish between:

* the original .NET Core 2.0 implementation;
* future modernized implementation work.

This document will be updated as the modernization progresses.
