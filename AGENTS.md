# AGENTS.md

## Scope

These instructions apply to the entire repository.

## Build and test

Use the modernized test project as the verified entry point:

```bash
dotnet restore SpiderAutoHome.Modern.slnx --locked-mode
dotnet build SpiderAutoHome.Modern.slnx --configuration Release --no-restore
dotnet run --project SpiderAutoHome.Tests/SpiderAutoHome.Tests.csproj --configuration Release --no-build
```

Do not use a live third-party website as a required CI dependency. Add sanitized, minimal, in-memory fixtures for parser changes.

## Project boundaries

- All three samples and `SpiderAutoHome.Tests` target .NET 10 and belong to the verified solution.
- Logo downloads must remain opt-in and disabled in tests and CI.
- Keep framework migration separate from live-site compatibility claims.

## Change guidelines

- Preserve the educational structure and explain non-obvious compatibility choices.
- Never add credentials, cookies, tokens, personal data, or access-control bypass logic.
- Keep request rates conservative and make network behavior explicit.
- Update README, DEVELOPMENT, CHANGELOG, and tests when behavior or support status changes.
- Run the NuGet vulnerability audit and leave the default branch buildable.
