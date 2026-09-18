# Maintainer automation plan

SpiderAutoHome is a compact project, but its 2018-to-current migration creates recurring maintainer work: dependency review, issue triage, fixture maintenance, release notes, and compatibility documentation. This plan describes where coding-agent and API credits would produce auditable open-source outcomes.

## Current maintenance load

- Review dependency updates across .NET, DotnetSpider, test tooling, and GitHub Actions.
- Reproduce parser reports against sanitized fixtures rather than unstable live pages.
- Keep all three modernized samples aligned with current dependencies without losing their teaching value.
- Keep English documentation synchronized with the original Chinese tutorial context.
- Review pull requests for security, data handling, test coverage, and compatibility claims.

## Planned automations

### 1. Pull-request review assistant

For each pull request, summarize the architectural change, identify affected tutorial concepts, check for missing fixtures and documentation, and flag network or secret-handling risks. The output remains advisory and maintainer-reviewed.

**Measure:** review turnaround, number of missing-test or compatibility issues caught before merge, and maintainer time saved.

### 2. Issue triage and reproduction planning

Classify new issues as parser behavior, dependency, documentation, live-source drift, or migration work. Draft a minimal offline reproduction checklist and identify the likely project owner.

**Measure:** time to first useful response and percentage of issues with reproducible fixture-based steps.

### 3. Dependency and security update analysis

Explain transitive changes, connect NuGet audit findings to the resolved dependency graph, propose the smallest compatible update, and generate focused regression tests.

**Measure:** age of actionable dependency alerts and percentage resolved with a verified test.

### 4. Source-drift maintenance

Turn sanitized source-drift reports into focused fixture updates before changing selectors, request construction, JSON models, asset handling, or storage.

**Measure:** time from reproducible drift report to tested fix, offline coverage, and successful full-solution builds.

### 5. Release preparation

Draft changelog entries from merged pull requests, identify breaking behavior, verify documentation links, and produce a maintainer checklist. Tagging and publishing remain explicit maintainer actions.

**Measure:** reproducible release checklist completion and time from milestone completion to release notes.

## Data and review boundaries

- Use public repository content and sanitized fixtures.
- Exclude credentials, private reports, cookies, and personal data from model inputs.
- Keep a human maintainer responsible for merges, releases, security disclosure, and external communication.
- Record material agent-generated changes in ordinary commits and pull requests so contributors can review them.

## Six-month outcomes

1. Keep all three samples green while expanding representative offline fixtures.
2. Establish a repeatable PR review and issue-triage checklist.
3. Keep known dependency vulnerability findings at zero on the supported projects.
4. Publish the first documented modern release after the supported scope is ready.
5. Report automation outcomes in roadmap issues without inflating usage or adoption claims.
