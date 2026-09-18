# Security policy

## Supported code

Security updates currently target all three modernized .NET 10 samples and their shared test project. They are educational examples rather than supported production components.

## Reporting a vulnerability

Please use GitHub's **Report a vulnerability** feature on the repository Security tab so details are kept private while they are assessed. Include:

- the affected file, dependency, or workflow;
- impact and realistic attack conditions;
- reproduction steps or a minimal proof of concept;
- a suggested fix, if available.

Do not include secrets, personal data, or live third-party data in a report. The primary maintainer aims to acknowledge a complete report within seven days, provide an initial assessment within fourteen days, and coordinate disclosure after a fix is available.

For ordinary bugs, outdated selectors, or documentation problems, use the public issue tracker.

## Dependency handling

NuGet audit covers direct and transitive packages during restore and build. Known vulnerability warnings are treated as errors. Dependabot monitors NuGet and GitHub Actions dependencies.
