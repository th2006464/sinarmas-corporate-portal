Param(
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64"
)

$ErrorActionPreference = "Stop"
$root = $PSScriptRoot
$publishDir = Join-Path $root "publish"
$releaseDir = Join-Path $root "release"

Write-Host "=== 企业门户发布脚本 (框架依赖部署) ==="

# 1. Publish Portal (framework-dependent, server has .NET 8)
Write-Host "[1/3] Publishing Portal (框架依赖) ..."
dotnet publish -c $Configuration -r $Runtime --self-contained false -o $publishDir
if ($LASTEXITCODE -ne 0) { throw "dotnet publish failed" }

# 2. Clean release (preserve logs and readme)
Write-Host "[2/3] Cleaning release directory ..."
if (Test-Path $releaseDir) {
    Get-ChildItem $releaseDir -Exclude "logs","使用说明.txt" | Remove-Item -Recurse -Force
} else {
    New-Item -ItemType Directory -Path $releaseDir | Out-Null
}

# 3. Copy published files to release
Write-Host "[3/3] Copying to release ..."
Copy-Item (Join-Path $publishDir "*") -Destination $releaseDir -Recurse -Force

# Remove unnecessary files for server deployment
@("createdump.exe", "*.pdb") | ForEach-Object {
    Get-ChildItem $releaseDir -File -Filter $_ -ErrorAction SilentlyContinue | Remove-Item -Force
}

# Remove nested duplicates
if (Test-Path "$releaseDir\release") { Remove-Item "$releaseDir\release" -Recurse -Force }
if (Test-Path "$releaseDir\publish") { Remove-Item "$releaseDir\publish" -Recurse -Force }

# Ensure required directories
@("logs", "data") | ForEach-Object {
    $d = Join-Path $releaseDir $_
    if (-not (Test-Path $d)) { New-Item -ItemType Directory -Path $d | Out-Null }
}

# Ensure data/news.json exists in release
$srcData = Join-Path $root "data\news.json"
$dstData = Join-Path $releaseDir "data\news.json"
if (-not (Test-Path $dstData) -and (Test-Path $srcData)) {
    Copy-Item $srcData -Destination $dstData -Force
}

# Clean up publish intermediate
Remove-Item $publishDir -Recurse -Force -ErrorAction SilentlyContinue

Write-Host "=== Publish complete ==="
Write-Host "    部署路径: $releaseDir"
