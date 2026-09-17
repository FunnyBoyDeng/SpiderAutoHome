# DotnetSpider Compatibility Assessment

## Status

Compatibility assessment completed before implementation.

This document evaluates the migration path from the DotnetSpider 2.5.x APIs currently used by SpiderAutoHome to the current DotnetSpider 5.x ecosystem.

No package upgrade or source migration is considered complete by this document.

## Baseline

The historical SpiderAutoHome solution currently uses:

```text
.NET Core 2.0
DotnetSpider.Core 2.5.0 / 2.5.1
DotnetSpider.Extension 2.5.0
```

The current stable DotnetSpider NuGet package evaluated for modernization is:

```text
DotnetSpider 5.1.7
```

The modernized SpiderAutoHome projects are planned to target:

```text
net10.0
```

The current DotnetSpider architecture is substantially different from the 2.5.x architecture, so the migration requires source changes rather than only changing package versions.

---

## 1. Package Model

### Legacy

The current projects reference separate packages:

```xml
<PackageReference Include="DotnetSpider.Core" Version="2.5.0" />
<PackageReference Include="DotnetSpider.Extension" Version="2.5.0" />
```

`SpiderAutoLogo` uses:

```xml
<PackageReference Include="DotnetSpider.Core" Version="2.5.1" />
<PackageReference Include="DotnetSpider.Extension" Version="2.5.0" />
```

### Modern

The current package model is centered on:

```xml
<PackageReference Include="DotnetSpider" Version="5.1.7" />
```

The old Core/Extension split should therefore not be carried directly into the modernized project files.

Additional storage or infrastructure packages should only be introduced if they are actually needed.

---

## 2. Spider Creation

### Legacy

The existing projects construct a spider directly:

```csharp
var spider = Spider.Create(
    site,
    new QueueDuplicateRemovedScheduler(),
    new AutoHomeProcessor());

spider
    .AddStartRequests(requests)
    .AddPipeline(new AutoHomePipe());

spider.Run();
```

### Modern

The current architecture expects the application spider to inherit from `Spider`:

```csharp
public class AutoHomeSpider(
    IOptions<SpiderOptions> options,
    DependenceServices services,
    ILogger<Spider> logger)
    : Spider(options, services, logger)
{
}
```

Startup is performed through the host builder:

```csharp
var builder = Builder.CreateDefaultBuilder<AutoHomeSpider>();
await builder.Build().RunAsync();
```

Initialization is performed inside:

```csharp
protected override async Task InitializeAsync(
    CancellationToken stoppingToken = default)
{
}
```

### Migration decision

`Spider.Create(...)` will be replaced with a dedicated `Spider` subclass and the current host-builder model.

---

## 3. Scheduler

### Legacy

The original projects explicitly create:

```csharp
new QueueDuplicateRemovedScheduler()
```

### Modern

`Builder.CreateDefaultBuilder<TSpider>()` configures the default queue-based scheduler with duplicate removal.

### Migration decision

The initial migration should use the default scheduler supplied by `CreateDefaultBuilder`.

A custom scheduler should only be introduced if a real requirement appears later.

This means the old explicit:

```csharp
new QueueDuplicateRemovedScheduler()
```

does not need a direct replacement in the initial migration.

---

## 4. Site Configuration

### Legacy

The old API uses:

```csharp
var site = new Site
{
    CycleRetryTimes = 1,
    SleepTime = 200,
    Headers = ...
};
```

### Modern

The old `Site` configuration model is no longer the primary configuration model.

Spider-level behavior is configured using `SpiderOptions`.

Relevant properties include:

```text
RetriedTimes
Speed
Depth
Batch
RequestedQueueCount
EmptySleepTime
```

### Migration mapping

Approximate mappings are:

```text
Site.CycleRetryTimes
    -> SpiderOptions.RetriedTimes
```

The old:

```text
Site.SleepTime = 200
```

represented approximately a 200 ms delay between requests.

The modern API uses a rate-oriented option:

```text
SpiderOptions.Speed
```

For example:

```text
Speed = 5
```

represents approximately five requests per second.

This is not an exact semantic replacement and should be validated during testing.

---

## 5. ThreadNum

### Legacy

The current projects use:

```csharp
spider.ThreadNum = 1;
```

### Modern

No direct `ThreadNum` equivalent is used by the current Spider API.

Scheduling and request throughput are now controlled through the hosting, scheduler, batch, downloader, and rate-limiting architecture.

### Migration decision

Do not mechanically replace:

```text
ThreadNum = 1
```

with:

```text
Batch = 1
```

because these settings do not have identical meanings.

The initial migration will use conservative request-rate settings and validate runtime behavior separately.

---

## 6. Starting Requests

### Legacy

```csharp
.AddStartRequests(resList.ToArray())
```

### Modern

Starting requests are added asynchronously from `InitializeAsync`:

```csharp
await AddRequestsAsync(request);
```

or:

```csharp
await AddRequestsAsync(requests);
```

### Migration decision

All existing start-request generation logic will move into the spider's `InitializeAsync` method.

---

## 7. Request URL

### Legacy

```csharp
Request request = new Request();
request.Url = "...";
```

### Modern

Requests can be created directly:

```csharp
var request = new Request("https://example.com");
```

The URL is represented by:

```text
Request.RequestUri
```

### Migration decision

Prefer the constructor form when creating new requests.

---

## 8. HTTP Method

### Legacy

```csharp
request.Method = System.Net.Http.HttpMethod.Post;
```

### Modern

`Request.Method` is represented as a string:

```csharp
request.Method = "POST";
```

### Migration decision

Existing GET/POST request creation must be updated accordingly.

---

## 9. POST Body

### Legacy

The current SpiderAutoHome project uses:

```csharp
request.PostBody = "...";
```

### Modern

The modern request model uses:

```text
Request.Content
```

For form data, the current API pattern is based on content objects such as `ByteArrayContent`.

Conceptually:

```csharp
var content = new ByteArrayContent(
    Encoding.UTF8.GetBytes(formData));

content.Headers.Add(
    "Content-Type",
    "application/x-www-form-urlencoded");

request.Content = content;
```

### Important compatibility warning

The stable 5.1.7 code predates an upstream fix to POST content-header handling.

Upstream later corrected the handling of content headers in `Request.ToHttpRequestMessage`.

This matters directly to SpiderAutoHome because its first sample uses an HTTP POST request with:

```text
Content-Type: application/x-www-form-urlencoded
```

Therefore, the POST sample must receive dedicated validation during migration.

The modernization must not claim live POST compatibility merely because the project compiles.

A dependency strategy for this issue must be selected before declaring the POST sample fully migrated.

---

## 10. Request Headers

### Legacy

Headers are configured globally through:

```csharp
Site.Headers
```

### Modern

Headers are available through:

```text
Request.Headers
```

The Spider base class also exposes:

```csharp
ConfigureRequest(Request request)
```

which can be overridden when common request configuration is required.

### Migration decision

Common headers should be applied centrally where practical rather than duplicated across every initial request.

Historical headers should also be reviewed instead of copied blindly.

In particular:

* obsolete browser User-Agent values should be reviewed;
* hard-coded Referer values should be reviewed;
* commented historical Cookie/session values should not be restored.

---

## 11. BasePageProcessor

### Legacy

The projects use classes such as:

```csharp
private class AutoHomeProcessor : BasePageProcessor
{
    protected override void Handle(Page page)
    {
    }
}
```

### Modern

Custom parsing is implemented using:

```csharp
DataParser
```

with:

```csharp
protected override Task ParseAsync(
    DataFlowContext context)
{
}
```

### Migration mapping

```text
BasePageProcessor
    -> DataParser

Handle(Page page)
    -> ParseAsync(DataFlowContext context)
```

This is one of the main source-level migration tasks.

---

## 12. Page and DataFlowContext

### Legacy

Processors receive:

```text
Page
```

### Modern

Parsers receive:

```text
DataFlowContext
```

Important members include:

```text
context.Request
context.Response
context.Selectable
context.Data
context.AddData(...)
context.GetData(...)
context.AddFollowRequests(...)
context.CreateNewRequest(...)
```

### Migration decision

Code using `Page` will be rewritten around `DataFlowContext`.

---

## 13. HTML Selection

### Legacy

Examples include:

```csharp
page.Selectable
    .XPath("...")
    .GetValue();
```

and:

```csharp
page.Selectable
    .XPath("...")
    .Nodes();
```

### Modern

The selector model is still available, but values are normally accessed using:

```csharp
context.Selectable
    .XPath("...")
    ?.Value;
```

`Nodes()` is still available for collections.

### Migration mapping

Typical transformation:

```text
.GetValue()
    -> .Value
```

The existing XPath expressions can initially be preserved for parser migration, but whether those XPath expressions still match the current external websites must be tested separately.

---

## 14. AddResultItem

### Legacy

```csharp
page.AddResultItem("CarList", list);
```

### Modern

```csharp
context.AddData("CarList", list);
```

Data can later be accessed using:

```csharp
context.GetData("CarList");
```

### Migration mapping

```text
Page.AddResultItem(...)
    -> DataFlowContext.AddData(...)
```

---

## 15. Dynamic Target Requests

### Legacy

SpiderAutoSkuData uses:

```csharp
page.AddTargetRequest(url);
```

### Modern

A parser can add follow-up requests through:

```csharp
context.AddFollowRequests(...);
```

A new request based on the current request can also be produced using:

```csharp
context.CreateNewRequest(new Uri(url));
```

### Migration decision

SpiderAutoSkuData's two follow-up API requests will be migrated to explicit follow requests through `DataFlowContext`.

---

## 16. TargetUrlsExtractor

### Legacy

SpiderAutoSkuData uses:

```csharp
RegionAndPatternTargetUrlsExtractor
```

to determine which processor should handle a URL.

### Modern

`DataParser` supports request validation through:

```csharp
AddRequiredValidator(...)
```

For example, a parser can register a regular-expression validator for the requests it handles.

### Migration mapping

The existing URL-routing logic can therefore be represented by dedicated `DataParser` classes with request validators.

The three existing logical processing stages can remain separated:

```text
GetSkuProcessor
GetBasicInfoProcessor
GetExtInfoProcessor
```

but become modern `DataParser` implementations.

---

## 17. BasePipeline

### Legacy

The projects define pipelines such as:

```csharp
private class AutoHomePipe : BasePipeline
```

and process:

```text
ResultItems
```

### Modern

The processing architecture uses data flows.

Built-in flows such as:

```text
ConsoleStorage
```

can be registered using:

```csharp
AddDataFlow<ConsoleStorage>();
```

Custom post-processing can be implemented as a `DataFlowBase` implementation if required.

### Migration mapping

```text
BasePipeline
    -> DataFlow / DataFlowBase
```

The old `ResultItems` pattern should not be preserved merely to imitate the legacy architecture.

---

## 18. Console Output

The existing projects primarily use pipelines to print results to the console.

The first migration should prefer the simplest appropriate solution:

* use a small custom DataFlow when formatted output is required;
* use `ConsoleStorage` where the built-in behavior is sufficient.

The goal is to preserve educational clarity rather than reproduce obsolete abstractions.

---

## 19. SpiderEntity

SpiderAutoHome currently declares:

```csharp
class AutoHomeShopListEntity : SpiderEntity
```

but the model is only used as a simple data container and overrides `ToString()`.

### Migration decision

This model does not need to inherit from a framework entity type.

It can initially become a plain C# model:

```csharp
class AutoHomeShopListEntity
{
}
```

If the project later adopts DotnetSpider's entity parsing/storage features, `EntityBase<T>` can be evaluated separately.

---

## 20. Legacy AutoHomeSpider Class

The repository contains:

```text
SpiderAutoHome/AutoHomeSpider.cs
```

which derives from the old `EntitySpider` API.

The class currently contains no real initialization logic and is not referenced by the active `Program.cs` workflow.

### Migration decision

Do not migrate this class mechanically.

During implementation it should either:

1. be removed as unused legacy code; or
2. be replaced by the actual modern `Spider` implementation if the filename is retained.

Removing unused code is preferred over preserving an obsolete abstraction.

---

## 21. Newtonsoft.Json

SpiderAutoSkuData directly uses:

```csharp
JsonConvert.DeserializeObject<T>()
```

The legacy project does not explicitly declare `Newtonsoft.Json` in its project file.

The modern DotnetSpider package currently depends on Newtonsoft.Json, which may make the namespace available transitively.

However, SpiderAutoSkuData directly depends on the API.

### Migration decision

If `Newtonsoft.Json` remains in use, declare it explicitly in `SpiderAutoSkuData.csproj`.

Application code should not rely unintentionally on another package's transitive dependency.

A later cleanup may evaluate migration to `System.Text.Json`, but that should be a separate change.

---

## 22. SpiderAutoLogo HTTP Download

The image download implementation is largely independent from DotnetSpider.

Current issues include:

* creating `HttpClient` repeatedly;
* blocking asynchronous operations with `.Result`;
* empty exception handling;
* no status-code validation.

### Migration decision

Do not mix all of these improvements into the first DotnetSpider API conversion.

First restore a successful build.

Then modernize the image-download implementation as a separate reviewable change.

---

## 23. Build Compatibility vs Website Compatibility

Two different success criteria must remain separate.

### Build compatibility

The project:

* restores successfully;
* compiles on `net10.0`;
* uses the selected DotnetSpider dependency;
* passes automated tests.

### Website compatibility

The project:

* can still access valid authorized endpoints;
* receives expected response structures;
* matches current HTML using its selectors;
* correctly parses current data.

A successful build does not prove website compatibility.

Historical third-party endpoints should not block the initial framework modernization.

---

## 24. Stable Package vs Upstream Master

As of September 2026, the latest stable NuGet package evaluated is:

```text
DotnetSpider 5.1.7
```

The upstream repository has continued to receive changes after that package release.

The upstream `master` branch has also moved its sample project to `.NET 10`.

However, unreleased upstream source should not automatically replace a stable dependency in SpiderAutoHome.

### Important exception

The POST content-header bug is particularly relevant because an upstream fix exists after the latest stable NuGet release.

Before the actual dependency upgrade, the project must explicitly choose how to handle this gap.

Possible approaches should be evaluated separately rather than silently depending on unreleased source.

---

## 25. Migration Mapping Summary

| Legacy 2.5.x                          | Modern 5.x direction                                    |
| ------------------------------------- | ------------------------------------------------------- |
| `DotnetSpider.Core`                   | `DotnetSpider`                                          |
| `DotnetSpider.Extension`              | `DotnetSpider` / modern APIs                            |
| `Spider.Create(...)`                  | `Spider` subclass + `Builder.CreateDefaultBuilder<T>()` |
| `Site`                                | `SpiderOptions` + request configuration                 |
| `CycleRetryTimes`                     | `RetriedTimes`                                          |
| `SleepTime`                           | `Speed`-based rate control                              |
| `ThreadNum`                           | No direct one-to-one replacement                        |
| `QueueDuplicateRemovedScheduler`      | Default builder scheduler                               |
| `AddStartRequests`                    | `AddRequestsAsync`                                      |
| `Request.Url`                         | `Request(...)` / `RequestUri`                           |
| `HttpMethod.Post`                     | `"POST"`                                                |
| `PostBody`                            | `Request.Content`                                       |
| `BasePageProcessor`                   | `DataParser`                                            |
| `Page`                                | `DataFlowContext`                                       |
| `Handle(Page)`                        | `ParseAsync(DataFlowContext)`                           |
| `GetValue()`                          | `.Value`                                                |
| `AddResultItem`                       | `AddData`                                               |
| `AddTargetRequest`                    | `AddFollowRequests`                                     |
| `RegionAndPatternTargetUrlsExtractor` | `AddRequiredValidator`                                  |
| `BasePipeline`                        | `DataFlowBase` / storage flow                           |
| `ResultItems`                         | `DataFlowContext.Data`                                  |
| `SpiderEntity`                        | Plain model or `EntityBase<T>` when needed              |
| `spider.Run()`                        | `builder.Build().RunAsync()`                            |

---

## 26. Project-by-Project Migration Strategy

### SpiderAutoHome

Primary changes:

1. Replace `Spider.Create` with a `Spider` subclass.
2. Replace `Site` with `SpiderOptions` and request configuration.
3. Replace `AutoHomeProcessor` with `DataParser`.
4. Replace `AutoHomePipe` with a modern data flow.
5. Replace POST body handling with `Request.Content`.
6. Explicitly validate the POST content-header compatibility issue.
7. Convert `AutoHomeShopListEntity` to a plain model.
8. Remove or repurpose the unused legacy `AutoHomeSpider.cs`.

This should be the **first project migrated** because it is small and exposes the important POST compatibility issue early.

### SpiderAutoSkuData

Primary changes:

1. Create a modern Spider subclass.
2. Convert all three processors to `DataParser`.
3. Replace processor URL extractors with request validators.
4. Replace `AddTargetRequest` with follow requests.
5. Explicitly reference Newtonsoft.Json if retained.
6. Validate JSON models using fixture-based tests.

This should be migrated after the first project establishes the basic architecture.

### SpiderAutoLogo

Primary changes:

1. Create a modern Spider subclass.
2. Convert `GetLogoInfoProcessor` to `DataParser`.
3. Replace the output pipeline with a modern data flow.
4. Preserve the existing image-download behavior initially if necessary for a small migration.
5. Modernize HttpClient and error handling separately.

---

## 27. Recommended Next Implementation Sequence

The compatibility analysis supports the following implementation sequence:

1. Create a dedicated modernization branch.
2. Migrate **SpiderAutoHome only** to `net10.0`.
3. Replace its old package references with the selected modern dependency strategy.
4. Convert its Spider, request, parser, and data-flow code.
5. Build the project without depending on the live website.
6. Resolve the stable-package POST compatibility decision.
7. Add parser tests with stored fixtures.
8. Migrate SpiderAutoSkuData.
9. Migrate SpiderAutoLogo.
10. Build the complete solution.
11. Add GitHub Actions.

Keeping the first implementation limited to one sample project will make migration errors easier to identify and review.

---

## Conclusion

SpiderAutoHome is compatible with the current DotnetSpider architecture at the **conceptual workflow level**, but it is **not source-compatible** with DotnetSpider 5.x.

The fundamental workflow remains recognizable:

```text
request
→ download
→ parse
→ create follow requests
→ process/store results
```

but most of the framework-facing implementation must be rewritten around:

```text
Spider
Builder
SpiderOptions
Request
DataParser
DataFlowContext
DataFlow
```

The next step should be implementation planning for the first `SpiderAutoHome` migration rather than changing all three projects simultaneously.
