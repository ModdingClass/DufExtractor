# DufExtractor

A Windows Explorer right-click tool that decompresses gzip-compressed DAZ Studio `.duf` files in place.

## Background

DAZ Studio `.duf` files are either plain JSON or gzip-compressed JSON (magic bytes `1F 8B`) — not zip/rar archives, even though renaming to `.zip` and opening in WinRAR happens to work, since WinRAR sniffs the real format regardless of extension. This tool skips that manual rename/WinRAR step and decompresses natively.

## How it works

1. Right-clicking a `.duf` file in Explorer shows a "Decompress DUF" entry (registered via `Install.ps1`).
2. The entry runs `DufExtractor.exe "<path>"`.
3. The exe reads the file header:
   - Gzip magic (`1F 8B`) -> decompresses via `GZipStream` to a temp file, then deletes the original and moves the temp file into place (same filename/extension).
   - Already starts with `{`/`[` (ignoring whitespace) -> treated as already-JSON, no-op.
   - Anything else -> reported as unrecognized.
4. Status (success/already-JSON/error) is shown via a custom self-dismissing toast popup (`Toast.cs`), not a blocking `MessageBox` — the user explicitly wants no dialogs requiring a click, including for errors.

## Project layout

- `Program.cs` — detection + in-place decompression logic, entry point.
- `Toast.cs` — borderless, auto-fading WinForms popup used for all status messages.
- `icon.ico` — embedded via `<ApplicationIcon>` in the csproj; also referenced by the registry `Icon` value so the context menu entry shows the same icon.
- `Install.ps1` / `Uninstall.ps1` — register/remove the context menu entry under `HKCU:\Software\Classes\SystemFileAssociations\.duf\shell\DecompressDuf`. This only adds a context-menu verb; it does not touch the default double-click/open association for `.duf` (so DAZ Studio's association is untouched), and needs no admin rights (HKCU only).
- `TestSample/EALeendaBD.duf` — a real user-provided compressed sample file. **Never overwrite or run destructive tests directly against it** — copy it to a scratch path first.

## Build & publish

```
dotnet build
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o publish
```

`publish/` (self-contained single-file exe, ~110MB) and `bin/`/`obj/` are gitignored build output — `Install.ps1` expects `publish\DufExtractor.exe` to exist (run `-ExePath` to point elsewhere).

## Conventions

- Status feedback must always be non-blocking/self-dismissing (toast), never a modal dialog requiring a click — applies to error states too.
- Registry changes should stay scoped to `HKEY_CURRENT_USER` — no admin rights, no machine-wide changes.
