# Codex for Open Source application notes

This file keeps application statements accurate, reproducible, and consistent with the public repository. Re-check live metrics immediately before submitting because stars, forks, and project status can change.

## Evidence snapshot — 2026-09-18

- Public repository: <https://github.com/FunnyBoyDeng/SpiderAutoHome>
- License: MIT
- Role: repository creator and primary maintainer
- GitHub usage: 20 stars, 2 forks, 3 watchers/subscribers
- History: original tutorials and source published in 2018; active maintenance resumed in 2026
- Current maintenance evidence: issue triage, a reviewed modernization pull request, successful GitHub Actions, offline tests, dependency auditing, Dependabot, contribution and security policies
- Concrete backlog: expand fixtures, automate source-drift triage, improve contributor review, and prepare the first documented modern release

Do not describe the repository as broadly adopted or production-ready. Its strongest case is durable Chinese-language educational value, historical continuity, a clear technical migration, and concrete ongoing maintainer work.

## Form draft

### Role

Primary maintainer.

### Why does this repository qualify? — 401/500 characters

> SpiderAutoHome preserves a three-part Chinese DotnetSpider tutorial and source first published in 2018. The public repository has 20 GitHub stars and 2 forks and is now a tested migration reference from .NET Core 2.0/DotnetSpider 2.x to .NET 10/5.x. As primary maintainer, I triage issues, review changes, maintain CI and security controls, and have modernized all three samples with 24 offline tests.

### How will you use API credits? — 456/500 characters

> I will use API credits for maintainer automation: PR review checklists, issue classification and offline reproduction plans, dependency/security analysis, fixture-based regression tests for source drift, documentation synchronization, and release-note drafting. Every output will be maintainer-reviewed and recorded through normal issues, pull requests, tests, and commits. Credentials, private security reports, and live third-party data will be excluded.

### Anything else? — 478/500 characters

> This is a small but durable repository with Chinese-language ecosystem value. Maintenance resumed in 2026 after its original 2018 tutorial release. All three samples now build on .NET 10 with 24 offline xUnit tests on Windows and Linux, locked dependencies, GitHub Actions, CodeQL, bilingual documentation, and opt-in asset downloads. Success will be measured by review turnaround, fixture coverage, clean dependency audits, contributor activity, and documented modern releases.

## Before submitting or following up

1. Confirm the repository and GitHub profile are public.
2. Re-check the metrics and character counts.
3. Link the roadmap issue and the latest successful Actions run when a free-text channel permits links.
4. Use the same email address as the ChatGPT account and provide the correct OpenAI organization ID.
5. Keep future claims tied to public issues, pull requests, releases, and workflow runs.

## GitHub settings follow-up

After these files are merged:

- change the short repository description from `SpiderAutoHome,DotnetSpider` to `Modernized C#/.NET examples for learning DotnetSpider through a three-part Chinese tutorial series`;
- add topics such as `csharp`, `dotnet`, `dotnetspider`, `web-scraping`, `tutorial`, and `chinese`;
- update roadmap issue #3 so completed tests, CI, migration, documentation, and security checks are checked off;
- delete the merged modernization branch;
- enable private vulnerability reporting and require the verification workflow on the default branch;
- create a milestone for the first modern release, then publish a tag only after its documented scope is verified.

The official program reviews repository usage, ecosystem importance, and evidence of active maintenance. It accepts applications on a rolling basis and explicitly invites applicants to explain ecosystem importance when a project does not fit neatly into other criteria: <https://openai.com/form/codex-for-oss/>.
