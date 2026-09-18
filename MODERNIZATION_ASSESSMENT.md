# SpiderAutoHome Modernization Assessment

> **Historical planning record (2026):** This assessment captured the repository before implementation. All three samples have since moved to .NET 10, gained offline tests, and entered CI. See [README.md](README.md) for current status.

This document records the initial technical assessment of the legacy SpiderAutoHome solution before framework or dependency upgrades are made.

The purpose of this assessment was to separate factual review from migration decisions before the first migration was implemented and tested.

## 1. Current Solution Structure

The repository contains one Visual Studio solution with three console application projects:

```text
SpiderAutoHome.sln
├── SpiderAutoHome/
│   ├── AutoHomeSpider.cs
│   ├── Program.cs
│   └── SpiderAutoHome.csproj
├── SpiderAutoSkuData/
│   ├── AutoCarConfig.cs
│   ├── AutoCarParam.cs
│   ├── Program.cs
│   └── SpiderAutoSkuData.csproj
└── SpiderAutoLogo/
    ├── Program.cs
    └── SpiderAutoLogo.csproj
```

The codebase is relatively small and does not currently contain:

* automated test projects;
* CI configuration;
* centralized package management;
* shared application libraries;
* dependency injection setup;
* configuration files for endpoints or runtime settings.

This makes the repository a reasonable candidate for incremental modernization.

## 2. Current Framework

All three projects currently target:

```text
netcoreapp2.0
```

.NET Core 2.0 is end-of-life and should be treated only as the historical framework for the original implementation.

The modernization should move to a currently supported .NET release, but the final target will be selected only after dependency compatibility has been evaluated.

## 3. Current DotnetSpider Dependencies

### SpiderAutoHome

```text
DotnetSpider.Core      2.5.0
DotnetSpider.Extension 2.5.0
```

### SpiderAutoSkuData

```text
DotnetSpider.Core      2.5.0
DotnetSpider.Extension 2.5.0
```

### SpiderAutoLogo

```text
DotnetSpider.Core      2.5.1
DotnetSpider.Extension 2.5.0
```

There is therefore already a small version inconsistency in the legacy solution: `SpiderAutoLogo` uses DotnetSpider.Core 2.5.1 while the Extension package remains at 2.5.0.

The current DotnetSpider ecosystem has moved well beyond the 2.5.x API generation. A direct package-version replacement should not be assumed to compile without source changes.

## 4. DotnetSpider API Usage

The legacy source relies heavily on DotnetSpider 2.x APIs, including concepts such as:

```text
Site
Request
Spider.Create
QueueDuplicateRemovedScheduler
BasePageProcessor
BasePipeline
Page
ResultItems
SpiderEntity
EntitySpider
RegionAndPatternTargetUrlsExtractor
```

These APIs form the main migration boundary.

Most application-specific logic is relatively simple:

1. create requests;
2. configure a spider;
3. select content using XPath;
4. transform extracted values;
5. write results to the console or local files.

Therefore, migration risk is expected to come primarily from framework and DotnetSpider API changes rather than from complex business logic.

## 5. SpiderAutoHome Assessment

`SpiderAutoHome` demonstrates shop/product-list extraction.

The current implementation includes:

* a configured `Site`;
* manually constructed POST requests;
* XPath extraction;
* a custom page processor;
* a custom pipeline;
* a simple entity model.

The project also contains `AutoHomeSpider`, an `EntitySpider` subclass with an empty initialization method.

This class should be reviewed during migration to determine whether it is still required.

### Observed modernization concerns

* Legacy DotnetSpider API usage
* Hard-coded target endpoints
* Hard-coded browser headers
* Historical commented cookie/session information
* Parsing logic coupled directly to the external page structure
* No automated parser tests

Historical cookie/session examples should be removed or sanitized as part of repository cleanup even if they are no longer valid.

## 6. SpiderAutoSkuData Assessment

`SpiderAutoSkuData` demonstrates multi-step extraction of automobile detail and configuration information.

It uses multiple processors and dynamically adds additional target requests.

The project also uses:

```csharp
Newtonsoft.Json
```

for deserialization.

However, `Newtonsoft.Json` is not explicitly declared in the project's `.csproj`.

The legacy build may therefore be relying on a transitive dependency supplied by another package.

Modernization should make every directly used package dependency explicit rather than relying unintentionally on transitive dependencies.

### Observed modernization concerns

* Legacy DotnetSpider processors and URL extractors
* Dynamic target-request API compatibility
* Implicit/transitive Newtonsoft.Json dependency
* Hard-coded historical endpoints
* JSON response contracts may have changed
* No deserialization tests
* No handling for malformed or changed responses

## 7. SpiderAutoLogo Assessment

`SpiderAutoLogo` extracts automobile brand/logo information and saves image files locally.

Besides DotnetSpider, the code directly uses:

```text
HttpClient
HttpRequestMessage
System.IO
```

### Observed modernization concerns

The current file download implementation:

* creates a new `HttpClient` for downloads;
* synchronously waits on asynchronous operations using `.Result`;
* manually disposes the client;
* uses an empty `catch` block;
* writes directly to an `img` directory;
* has no explicit HTTP status validation;
* has no download tests.

This code should eventually be converted to an asynchronous implementation with explicit error handling.

The migration does not need to combine this cleanup with the first framework upgrade. Keeping changes small and reviewable is preferred.

## 8. External Website Dependency

A successful build does not necessarily mean that the examples still work against the original websites.

The repository contains URLs and HTML selectors based on website structures from the original 2018 implementation.

Modernization therefore has two separate concerns:

### Build compatibility

Can the projects compile and run on a supported .NET and DotnetSpider stack?

### Data-source compatibility

Do the historical URLs, endpoints, response formats, and XPath selectors still match the external websites?

These concerns should be tested independently.

The initial modernization should prioritize a reproducible build before attempting to restore every historical data-source integration.

## 9. Testing Status

There are currently no automated tests.

Useful future test boundaries include:

* HTML/XPath parsing with stored fixture data;
* JSON deserialization;
* request construction;
* model transformation;
* URL handling;
* image-path generation.

Tests should avoid depending exclusively on live third-party websites because live HTML and APIs can change independently of this repository.

## 10. CI Status

The repository currently has no automated build or test workflow.

GitHub Actions should be added after a supported .NET target is selected and the solution can build reproducibly.

A future CI workflow should at minimum:

1. restore packages;
2. build the solution;
3. run automated tests;
4. fail on build or test errors.

Dependency and security checks can then be added separately.

## 11. Migration Risk Summary

### Lower-risk areas

* Small codebase
* Three simple console applications
* Limited internal architecture
* Simple data models
* Mostly straightforward extraction workflows

### Medium-risk areas

* Hard-coded endpoints and selectors
* Historical JSON response structures
* File downloading implementation
* Implicit package dependency usage
* Lack of tests

### Higher-risk areas

* .NET Core 2.0 to modern .NET migration
* DotnetSpider 2.5.x to current DotnetSpider API migration
* External website/API changes since 2018

## 12. Recommended Migration Sequence

No implementation is performed by this document.

The recommended sequence for future work is:

1. Complete the current dependency and source assessment.
2. Select a supported .NET target.
3. Evaluate current DotnetSpider API compatibility.
4. Create a dedicated migration branch or pull request.
5. Modernize one sample project first.
6. Restore a reproducible build.
7. Add basic automated tests.
8. Add GitHub Actions CI.
9. Migrate the remaining sample projects.
10. Review live-site compatibility separately.

## Conclusion

The repository is small enough to modernize incrementally.

The main technical challenge is not the amount of application code. It is the large version gap between the original .NET Core 2.0 / DotnetSpider 2.5.x environment and the current .NET and DotnetSpider ecosystem.

For this reason, framework selection and DotnetSpider compatibility should be evaluated before package versions or source APIs are changed.
