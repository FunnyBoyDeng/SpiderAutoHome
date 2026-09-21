[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$repositoryRoot = Split-Path -Parent $PSScriptRoot
$missingLinks = [System.Collections.Generic.List[string]]::new()
$linkPattern = [regex]'!?(?:\[[^\]]*\])\((?<target>[^)]+)\)'

Get-ChildItem -LiteralPath $repositoryRoot -Recurse -Filter '*.md' -File |
    Where-Object { $_.FullName -notmatch '[\\/]\.(git)[\\/]' } |
    ForEach-Object {
        $document = $_
        $content = Get-Content -LiteralPath $document.FullName -Raw

        foreach ($match in $linkPattern.Matches($content)) {
            $target = $match.Groups['target'].Value.Trim()
            $targetWithoutFragment = ($target -split '#', 2)[0]

            if ([string]::IsNullOrWhiteSpace($targetWithoutFragment) -or
                $targetWithoutFragment -match '^(?:https?://|mailto:|data:)') {
                continue
            }

            $decodedTarget = [Uri]::UnescapeDataString($targetWithoutFragment)
            $resolvedPath = Join-Path $document.DirectoryName $decodedTarget

            if (-not (Test-Path -LiteralPath $resolvedPath)) {
                $relativeDocument = [IO.Path]::GetRelativePath(
                    $repositoryRoot,
                    $document.FullName)
                $missingLinks.Add("$relativeDocument -> $target")
            }
        }
    }

if ($missingLinks.Count -gt 0) {
    $missingLinks | ForEach-Object { Write-Error "Missing local link: $_" }
    exit 1
}

Write-Host 'All local Markdown links resolve.'
