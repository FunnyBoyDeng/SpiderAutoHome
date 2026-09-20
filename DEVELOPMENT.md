# Development guide

This guide describes the verified development workflow for all three modernized samples.

## Toolchain

- .NET SDK 10.0 (selected by the repository's `global.json`)
- DotnetSpider 5.1.7
- xUnit v3 with Microsoft Testing Platform

Check the active SDK:

```bash
dotnet --info
```

## Verified workflow

Restore the modernized application and its tests:

```bash
dotnet restore SpiderAutoHome.Modern.slnx --locked-mode
```

Build in the same configuration used by CI:

```bash
dotnet build SpiderAutoHome.Modern.slnx \
  --configuration Release \
  --no-restore
```

Run the offline test executable:

```bash
dotnet run --project SpiderAutoHome.Tests/SpiderAutoHome.Tests.csproj \
  --configuration Release \
  --no-build
```

The repository configures Microsoft Testing Platform in `global.json`; `dotnet test` behavior can vary by SDK/test-runner integration, so CI deliberately invokes the test executable.

## Dependency audit

NuGet audit is enabled for direct and transitive dependencies. Findings with low, moderate, high, or critical severity are promoted to build errors through `Directory.Build.props`.

To inspect the resolved graph:

```bash
dotnet list SpiderAutoHome.Modern.slnx package --include-transitive
dotnet list SpiderAutoHome.Modern.slnx package --vulnerable --include-transitive
```

`MessagePack` is pinned to a patched 2.5.x version because DotnetSpider 5.1.7 otherwise resolves an older vulnerable transitive version. Remove the explicit pin after the upstream dependency is updated and the audit remains clean.

## Project boundaries

### Modernized and verified

- `SpiderAutoHome/`
- `SpiderAutoSkuData/`
- `SpiderAutoLogo/`
- `SpiderAutoHome.Tests/`

All projects target .NET 10, use DotnetSpider 5.1.7 where applicable, and are included in the current CI build.

## Testing principles

1. Put representative HTML or JSON in deterministic fixtures.
2. Keep normal CI independent from live third-party services.
3. Test empty, partial, and changed markup as well as the happy path.
4. Keep request construction separate from parsing when new features are added.
5. Never commit credentials, cookies, tokens, or personal data in fixtures.

## Running the live sample

```bash
dotnet run --project SpiderAutoHome/SpiderAutoHome.csproj
```

This command contacts the configured endpoint. Before using it, confirm that the endpoint is still appropriate, review current access rules, and keep request volume conservative. External HTML and APIs can change independently of this repository, so a successful offline build does not guarantee live-site compatibility.

The SKU sample accepts an optional `SPIDERAUTOHOME_SKU_URL` environment variable. Its parser and request factory are tested with in-memory HTML and JSON fixtures.

The logo sample accepts `SPIDERAUTOHOME_LOGO_URL`. Asset downloads require `SPIDERAUTOHOME_DOWNLOAD_LOGOS=true`; `SPIDERAUTOHOME_LOGO_DIR` selects the output directory. Downloads stay off during tests and CI.

Downloads use a temporary file followed by an atomic move and enforce a 10 MiB per-file limit. CI runs the same locked build and offline suite on Windows and Linux to catch platform-specific URL and filesystem behavior.

## Pull-request checklist

- Link the relevant issue for non-trivial work.
- Keep the change focused and explain compatibility decisions.
- Add or update offline tests for parser behavior.
- Run the verified restore/build/test workflow.
- Update documentation when status or behavior changes.
- Review `dotnet list package --vulnerable --include-transitive`.
- Run `./scripts/Test-MarkdownLinks.ps1` after changing documentation links.

See [CONTRIBUTING.md](CONTRIBUTING.md) for the contribution process and [SECURITY.md](SECURITY.md) for private vulnerability reporting.
