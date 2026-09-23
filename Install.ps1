<#
.SYNOPSIS
    Registers the "Decompress DUF" right-click context menu entry for .duf files.

.DESCRIPTION
    Adds a shell verb under HKCU\Software\Classes\SystemFileAssociations\.duf\shell
    This only ADDS a context-menu command; it does not change the default
    double-click / open action for .duf files (so DAZ Studio's association is untouched).
    Runs entirely under HKEY_CURRENT_USER, so no admin rights are required.

.PARAMETER ExePath
    Path to the published DufExtractor.exe. Defaults to .\publish\DufExtractor.exe
    next to this script.
#>
param(
    [string]$ExePath = (Join-Path $PSScriptRoot "publish\DufExtractor.exe")
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path $ExePath)) {
    throw "DufExtractor.exe not found at: $ExePath`nBuild/publish it first, or pass -ExePath."
}
$ExePath = (Resolve-Path $ExePath).Path

$verbKey = "HKCU:\Software\Classes\SystemFileAssociations\.duf\shell\DecompressDuf"
$cmdKey  = Join-Path $verbKey "command"

New-Item -Path $verbKey -Force | Out-Null
Set-ItemProperty -Path $verbKey -Name "(default)" -Value "Decompress DUF"
Set-ItemProperty -Path $verbKey -Name "Icon" -Value "`"$ExePath`",0"

New-Item -Path $cmdKey -Force | Out-Null
Set-ItemProperty -Path $cmdKey -Name "(default)" -Value "`"$ExePath`" `"%1`""

Write-Host "Registered 'Decompress DUF' context menu entry for .duf files."
Write-Host "  Exe:  $ExePath"
Write-Host "  Key:  $verbKey"
Write-Host ""
Write-Host "Right-click a .duf file in Explorer to see the new entry."
Write-Host "If it doesn't show up immediately, restart Explorer or sign out/in."
