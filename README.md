# DufExtractor

A Windows Explorer right-click tool that decompresses gzip-compressed DAZ Studio `.duf` files in place.

DAZ Studio `.duf` files are either plain JSON or gzip-compressed JSON — not zip/rar archives, even though renaming one to `.zip` and opening it in WinRAR happens to work, since WinRAR sniffs the real format regardless of extension. DufExtractor skips that manual rename/extract step: right-click a `.duf` file, click **Decompress DUF**, done.

- Detects gzip vs. already-plain-JSON automatically — running it on an already-decompressed file is a safe no-op.
- Decompresses in place (same filename, same `.duf` extension).
- Status shown via a small self-dismissing toast popup — no dialogs to click through.
- Registered entirely under `HKEY_CURRENT_USER` — no admin rights required, and it doesn't touch DAZ Studio's default double-click/open association for `.duf` files.

## Install (prebuilt release)

1. Download the latest `DufExtractor-*-win-x64.zip` from [Releases](../../releases), extract it anywhere.
2. Right-click `Install.ps1` → **Run with PowerShell** (or run it from a PowerShell prompt).
3. Right-click any `.duf` file in Explorer → **Decompress DUF**.

To remove the context menu entry, run `Uninstall.ps1` the same way.

## Build from source

Requires the [.NET 9 SDK](https://dotnet.microsoft.com/download).

```
dotnet build
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o publish
.\Install.ps1
```

`Install.ps1` looks for `DufExtractor.exe` next to itself first, then in `.\publish\`.

## How it works

- Reads the file header: gzip magic bytes (`1F 8B`) means compressed, `{`/`[` means already JSON.
- Gzip files are decompressed via `GZipStream` to a temp file, then the original is replaced.
- The context menu entry is a shell verb under `HKCU:\Software\Classes\SystemFileAssociations\.duf\shell\DecompressDuf`, pointing at the exe.
