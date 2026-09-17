# Target Framework Decision

## Status

Accepted for the modernization work.

## Decision

The modernized SpiderAutoHome projects will target:

```text
net10.0
```

The existing `netcoreapp2.0` implementation remains the historical baseline until the migration is implemented and validated.

This document records the target-framework decision only. It does not mean that the source code or dependencies have already been successfully migrated.

## Context

SpiderAutoHome was originally created in 2018 and currently contains three console application projects targeting:

```text
netcoreapp2.0
```

The repository is being modernized in 2026.

A supported target framework must therefore be selected before updating DotnetSpider or modifying the application source.

The primary candidates considered were:

* .NET 8
* .NET 10

## Why .NET 10 Was Selected

### 1. Long-term support window

.NET 10 is the current Long Term Support release.

At the time of this decision in September 2026, .NET 10 is in active support and is scheduled to remain supported until November 2028.

This gives the modernized educational project a substantially longer maintenance window.

### 2. .NET 8 is near end of support

.NET 8 is also an LTS release, but its support lifecycle is already close to completion in 2026.

Selecting .NET 8 now would introduce another framework migration shortly after completing the current modernization.

For a newly modernized repository, this would add unnecessary maintenance work.

### 3. Current DotnetSpider package compatibility

The current stable DotnetSpider package targets .NET 8.

NuGet package compatibility information indicates that the package can also be consumed by applications targeting newer compatible .NET versions, including .NET 10.

Therefore, the DotnetSpider package target framework alone does not require SpiderAutoHome to target .NET 8.

API compatibility still needs to be evaluated separately.

### 4. Educational project lifecycle

SpiderAutoHome is intended to remain useful as an educational open-source example.

Using the current LTS framework provides contributors with a modern development environment and avoids documenting a target framework that is already approaching end-of-support.

## Why .NET 8 Was Not Selected

.NET 8 would provide a potentially smaller conceptual jump because the current DotnetSpider package directly targets `net8.0`.

However, that advantage is outweighed by its remaining support window.

The modernization effort involves substantial changes from:

```text
.NET Core 2.0
DotnetSpider 2.5.x
```

to the current ecosystem.

Completing that work against a framework that is about to leave support would provide limited long-term value.

## Why .NET 11 Was Not Selected

At the time of this decision, .NET 11 is not the current stable LTS baseline for the project.

The modernization should target a stable LTS release rather than a preview or release-candidate framework.

The target can be reviewed in the future as part of normal maintenance.

## Package Compatibility Is a Separate Decision

Selecting `net10.0` does not mean the existing DotnetSpider 2.5.x references can simply be upgraded without source changes.

The legacy project relies on older DotnetSpider APIs such as:

```text
Spider.Create
Site
Request
BasePageProcessor
BasePipeline
QueueDuplicateRemovedScheduler
EntitySpider
RegionAndPatternTargetUrlsExtractor
```

The next modernization step is therefore to evaluate how these APIs map to the current DotnetSpider ecosystem.

Only after that assessment should package references or application source be modified.

## Migration Strategy

The planned sequence is:

1. Keep the current `master` source as the documented legacy baseline.
2. Evaluate current DotnetSpider APIs and migration requirements.
3. Prepare the implementation as a focused modernization change.
4. Change project targets from `netcoreapp2.0` to `net10.0`.
5. Update or replace legacy DotnetSpider package references.
6. Resolve compilation errors caused by API changes.
7. Build all three sample projects.
8. Add automated tests.
9. Add CI.
10. Update development documentation with verified commands.

## Validation Requirement

The target-framework migration will only be considered complete after all three projects build successfully on .NET 10:

```text
SpiderAutoHome
SpiderAutoSkuData
SpiderAutoLogo
```

Live compatibility with the historical third-party websites will be evaluated separately from build compatibility.

## Conclusion

`net10.0` is the selected target framework for the SpiderAutoHome modernization.

The decision prioritizes:

* a current LTS release;
* a longer support lifecycle;
* compatibility with the current .NET ecosystem;
* reduced need for anothe
