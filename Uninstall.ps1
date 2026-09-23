<#
.SYNOPSIS
    Removes the "Decompress DUF" right-click context menu entry for .duf files.
#>

$ErrorActionPreference = "Stop"

$verbKey = "HKCU:\Software\Classes\SystemFileAssociations\.duf\shell\DecompressDuf"

if (Test-Path $verbKey) {
    Remove-Item -Path $verbKey -Recurse -Force
    Write-Host "Removed context menu entry: $verbKey"
} else {
    Write-Host "Nothing to remove; key not found: $verbKey"
}
