# Release process

Releases package the three educational console samples after a locked restore, Release build, offline tests, and checksum generation.

## Prepare

1. Confirm the default branch verification and CodeQL workflows are green.
2. Review open security and dependency alerts.
3. Move relevant entries from `CHANGELOG.md` under a version and date.
4. Verify README status, environment variables, and known live-source limitations.
5. Run locally:

   ```bash
   dotnet restore SpiderAutoHome.Modern.slnx --locked-mode
   dotnet build SpiderAutoHome.Modern.slnx --configuration Release --no-restore
   dotnet run --project SpiderAutoHome.Tests/SpiderAutoHome.Tests.csproj --configuration Release --no-build
   dotnet list SpiderAutoHome.Modern.slnx package --vulnerable --include-transitive --no-restore
   ```

## Package a candidate

Run the **Package release candidate** workflow manually with a version label. Download the resulting artifact and verify its `SHA256SUMS` file.

## Publish

After the candidate is approved, create a signed semantic-version tag such as `v1.0.0` on the reviewed commit and push the tag. The same workflow packages the tag. Create the GitHub release from that tag, attach the verified bundle, and use the matching changelog section as release notes.

Release publication remains an explicit maintainer action. Do not publish from an unreviewed working tree or when required checks are failing.
