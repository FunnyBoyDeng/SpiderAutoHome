# SpiderAutoHome

[简体中文](README.zh-CN.md) | English

[![Verify SpiderAutoHome](https://github.com/FunnyBoyDeng/SpiderAutoHome/actions/workflows/verify-spiderautohome.yml/badge.svg)](https://github.com/FunnyBoyDeng/SpiderAutoHome/actions/workflows/verify-spiderautohome.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)
[![GitHub stars](https://img.shields.io/github/stars/FunnyBoyDeng/SpiderAutoHome)](https://github.com/FunnyBoyDeng/SpiderAutoHome/stargazers)

SpiderAutoHome is an educational C# repository that shows three practical web-data extraction patterns with [DotnetSpider](https://github.com/dotnetcore/DotnetSpider): paginated form requests, multi-step detail/API extraction, and brand asset discovery.

The project began in 2018 as a Chinese tutorial series. Active maintenance resumed in 2026 to preserve those examples, make them reproducible on a supported .NET toolchain, and provide a small real-world migration reference from DotnetSpider 2.x to 5.x.

> All three samples now build on .NET 10 and DotnetSpider 5.1.7. Their parsers, request construction, JSON models, URL normalization, and filename handling are covered by offline tests. Live third-party markup and API contracts may differ from the stored educational examples.

## Why this repository is useful

- Preserves a complete Chinese-language DotnetSpider tutorial series and its source.
- Demonstrates the architectural migration from `Site` / `BasePageProcessor` / `BasePipeline` to the modern host, `Spider`, `DataParser`, and data-flow APIs.
- Keeps parser tests offline and deterministic, so CI never depends on a third-party website.
- Documents compatibility decisions and known limitations rather than hiding legacy constraints.
- Provides focused examples that are smaller than a production crawler and easier for new contributors to study.

## Repository status

| Project | Purpose | Runtime | Status |
| --- | --- | --- | --- |
| `SpiderAutoHome` | Paginated shop/list parsing | .NET 10 / DotnetSpider 5.1.7 | Builds and is covered by offline parser tests |
| `SpiderAutoHome.Tests` | Deterministic parser fixtures | .NET 10 / xUnit v3 | Runs in GitHub Actions |
| `SpiderAutoSkuData` | Multi-step product detail/API extraction | .NET 10 / DotnetSpider 5.1.7 | Builds; typed JSON and follow-request tests |
| `SpiderAutoLogo` | Brand/logo extraction and opt-in asset downloads | .NET 10 / DotnetSpider 5.1.7 | Builds; parsing, URL, and filename tests |

The modernization roadmap is tracked in [GitHub issue #3](https://github.com/FunnyBoyDeng/SpiderAutoHome/issues/3). Historical assessment documents remain in the repository so that migration decisions are auditable.

## Quick start

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Git

### Build and test the modernized sample

```bash
git clone https://github.com/FunnyBoyDeng/SpiderAutoHome.git
cd SpiderAutoHome
dotnet restore SpiderAutoHome.Modern.slnx --locked-mode
dotnet build SpiderAutoHome.Modern.slnx --configuration Release --no-restore
dotnet run --project SpiderAutoHome.Tests/SpiderAutoHome.Tests.csproj --configuration Release --no-build
```

The test suite uses in-memory HTML fixtures and makes no live scraping requests.

Both `SpiderAutoHome.Modern.slnx` and the Visual Studio `SpiderAutoHome.sln` build all three samples and the test project.

### Run the sample

```bash
dotnet run --project SpiderAutoHome/SpiderAutoHome.csproj
```

Run the multi-step SKU sample with its historical default detail URL, or supply another permitted detail page through the environment:

```bash
SPIDERAUTOHOME_SKU_URL=https://example.com/permitted-detail \
  dotnet run --project SpiderAutoSkuData/SpiderAutoSkuData.csproj
```

Logo downloads are disabled by default. Enable them explicitly and optionally choose an output directory:

```bash
SPIDERAUTOHOME_DOWNLOAD_LOGOS=true \
SPIDERAUTOHOME_LOGO_DIR=./img \
  dotnet run --project SpiderAutoLogo/SpiderAutoLogo.csproj
```

Running the sample performs network requests to its configured data source. Review the endpoint, current site rules, request rate, and your intended use before running it. For learning or contribution work, prefer the offline tests.

## Tutorial series

1. [Automobile shop data extraction](https://www.cnblogs.com/FunnyBoy/p/8453338.html) — `SpiderAutoHome`
2. [Automobile product detail extraction](https://www.cnblogs.com/FunnyBoy/p/9029937.html) — `SpiderAutoSkuData`
3. [Automobile brand and logo extraction](https://www.cnblogs.com/FunnyBoy/p/9097377.html) — `SpiderAutoLogo`

An original [demonstration video](https://www.bilibili.com/video/av24022630/) is also available.

## Engineering and maintenance

- [Development guide](DEVELOPMENT.md)
- [Contributing guide](CONTRIBUTING.md)
- [Security policy](SECURITY.md)
- [Maintainers and response expectations](MAINTAINERS.md)
- [Release process](RELEASING.md)
- [Modernization assessment](MODERNIZATION_ASSESSMENT.md)
- [Target framework decision](TARGET_FRAMEWORK_DECISION.md)
- [DotnetSpider compatibility mapping](DOTNETSPIDER_COMPATIBILITY.md)
- [Maintainer automation plan](docs/MAINTAINER_AUTOMATION.md)

CI restores, builds, runs offline tests, and treats known NuGet vulnerability findings as errors. CodeQL scans the supported C# projects, and Dependabot monitors NuGet packages and GitHub Actions.

## Contributing

Issues and pull requests are welcome, especially for fixture-based tests, documentation, dependency maintenance, source-drift reports, and accessibility of the tutorial material. Please read [CONTRIBUTING.md](CONTRIBUTING.md) before opening a substantial change.

## Responsible use

This repository teaches data-extraction architecture; it does not grant access to any third-party system. Use only data and systems you are permitted to access. Respect applicable terms, robots policies, privacy requirements, rate limits, and data-use restrictions. Do not add credentials, personal data, access-control bypasses, or anti-abuse evasion techniques to examples or fixtures.

## License

[MIT](LICENSE) © 2018–2026 FunnyBoyDeng.
