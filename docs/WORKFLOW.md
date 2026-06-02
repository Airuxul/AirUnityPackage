# Meta Repository Workflow

**Last Updated:** 2026-06-02 · **Owner:** meta-repo · **Scope:** agent documentation (English)

## Roles

| Area | Responsibility |
|------|----------------|
| Meta repo (`CustomPackages`) | Submodule pointers, `config/registry.json`, install scripts, governance docs |
| `packages/<name>` | UPM source; commit / tag / push in each submodule repository |

## Clone

```bash
git clone --recurse-submodules https://github.com/Airuxul/AirUnityPackage.git CustomPackages
cd CustomPackages
```

Or clone then run `init-submodules.bat`.

## Install into Unity

```bat
install-to-unity.bat C:\Path\To\UnityProject
```

Reads `config/registry.json` for `installDefault` packages. Does not overwrite existing manifest entries.

## Change a package and publish

1. Work inside the submodule under `packages/<folder>`.
2. Commit, tag, and push in that package repository.
3. In the meta repo, update the submodule pointer commit.
4. Commit the meta repo pointer change.

Local development uses `file:` dependencies via `install-to-unity.bat`.

## Documentation updates

1. Read [AGENTS.md](AGENTS.md) and [DOC_GOVERNANCE.md](DOC_GOVERNANCE.md).
2. Use Cursor skills `doc-read-index` then `doc-generate-update` when applicable.
3. Keep `README.md` and `README.zh-CN.md` in sync for user-facing changes.
4. Record agent-side summaries in [CHANGELOG_AGENT.md](CHANGELOG_AGENT.md).

Enable hooks once per clone:

```powershell
.\tools\install-git-hooks.ps1
```
