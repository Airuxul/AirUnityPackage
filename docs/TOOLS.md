# Tools Reference

**Last Updated:** 2026-06-02 · **Owner:** meta-repo · **Scope:** agent documentation (English)

Scripts live under `tools/`. Root `*.bat` files are thin wrappers.

## install-packages.ps1

Adds missing `installDefault` packages from `config/registry.json` to Unity `Packages/manifest.json` as `file:` dependencies.

```powershell
.\tools\install-packages.ps1 -MetaRoot . -UnityProject C:\Project\GameDemo
```

Or use `install-to-unity.bat [UnityProjectPath]`.

## validate-docs.ps1

Used by `.githooks/pre-commit` when staged files include documentation:

- Requires `README.md` and `README.zh-CN.md` to change together when either changes.
- Requires `docs/*.md` to change together with user README when dual-track applies.
- Changes under `config/` alone do not require README updates.

## install-git-hooks.ps1

```powershell
.\tools\install-git-hooks.ps1
```

Sets `git config core.hooksPath .githooks` for this repository.

## unity-compile-loop.ps1

Runs `unity-cmd` compile + console check (default profile `editor`). Required when editing `packages/**/*.cs` (see [.cursor/rules/README.md](../.cursor/rules/README.md)).

```powershell
.\tools\unity-compile-loop.ps1 -IncludeWarnings -ConsoleLines 400
```

Requires `packages/unity-cli/unity-cmd` submodule to be present.
