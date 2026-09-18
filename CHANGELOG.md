# Changelog

Notable user-facing changes are recorded here. The project follows semantic versioning for future tagged releases.

## Unreleased

### Added

- Offline xUnit parser coverage for complete, partial, and unrelated markup.
- Offline coverage for SKU extraction, follow-request construction, typed JSON responses, and malformed data.
- Offline coverage for logo parsing, URL normalization, request validation, and portable filenames.
- NuGet audit enforcement, CodeQL analysis, and Dependabot configuration.
- A verified release-candidate packaging workflow with SHA-256 checksums.
- Security, maintainer, contribution, and repository automation documentation.

### Changed

- Modernized the primary `SpiderAutoHome` sample to .NET 10 and DotnetSpider 5.1.7.
- Modernized `SpiderAutoSkuData` to .NET 10, DotnetSpider 5.1.7, and `System.Text.Json`.
- Modernized `SpiderAutoLogo` to .NET 10 with safe, explicitly enabled asynchronous downloads.
- Updated development instructions to match the verified CI workflow.

### Security

- Pinned the transitive MessagePack dependency to a patched 2.5.x release.

## Historical code (2018)

The original three-part tutorial implementation targeted .NET Core 2.0 and DotnetSpider 2.5.x. Git history remains the authoritative record for changes before the first tagged modern release.
