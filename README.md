# Air Unity Packages

[简体中文](README.zh-CN.md)

**Last Updated:** 2026-06-04 · **Scope:** user documentation (English)

Unity UPM **meta repository**: submodule pointers under `packages/`, install tooling, and agent governance under `docs/`.

## Repository layout

```text
CustomPackages/
├── packages/              # Git submodules (UPM source)
├── config/                # registry & tags (tooling; see docs/AGENTS.md)
├── tools/                 # install, doc validation, Unity compile helpers
├── docs/                  # agent documentation (English)
├── TODO.zh-CN.md          # cross-package optimization backlog (Chinese)
├── init-submodules.bat
├── install-to-unity.bat
├── README.md              # this file (English)
└── README.zh-CN.md        # Chinese user docs
```

## Quick start

```bat
cd C:\Project\GameDemo\CustomPackages
init-submodules.bat
install-to-unity.bat C:\Project\GameDemo
```

`install-to-unity.bat` adds packages marked `installDefault` in `config/registry.json` to the Unity project `Packages/manifest.json` as `file:` dependencies. Existing entries are not overwritten.

## Commands

| Command | Purpose |
|---------|---------|
| `init-submodules.bat` | `git submodule sync` + `update --init --recursive` for `packages/` |
| `install-to-unity.bat [UnityProjectPath]` | Install default packages into Unity manifest |

## Manual manifest

If you prefer hand-editing Unity dependencies, see [config/manifest.example.json](config/manifest.example.json) for `file:` path examples (adjust paths for your machine).

## Tools (contributors)

| Script | Purpose |
|--------|---------|
| [tools/install-packages.ps1](tools/install-packages.ps1) | Core logic used by `install-to-unity.bat` |
| [tools/validate-docs.ps1](tools/validate-docs.ps1) | Pre-commit doc parity checks |
| [tools/install-git-hooks.ps1](tools/install-git-hooks.ps1) | One-time: enable `.githooks/pre-commit` |
| [tools/unity-compile-loop.ps1](tools/unity-compile-loop.ps1) | Unity compile + console check via `unity-cmd` |

Details: [docs/TOOLS.md](docs/TOOLS.md).

## Troubleshooting

| Issue | What to try |
|-------|-------------|
| Submodule update failed | Re-run `init-submodules.bat`; check `.gitmodules`, network, and Git credentials |
| Package folder missing | Submodules not initialized — run `init-submodules.bat` |
| `install-to-unity.bat` failed | Pass a valid Unity project root (contains `Assets/` and `Packages/manifest.json`) |
| Doc commit blocked by hook | Update `README.md` and `README.zh-CN.md` together when changing user docs; see [docs/DOC_GOVERNANCE.md](docs/DOC_GOVERNANCE.md) |

## Optimization backlog

Cross-package follow-ups (existing features only): [TODO.zh-CN.md](TODO.zh-CN.md) (Chinese). Agent coordinator: [docs/TODO_ROADMAP.md](docs/TODO_ROADMAP.md). Per-package lists live in each submodule as `TODO.zh-CN.md` / `docs/TODO.md`.

## Agent documentation

Agent-facing rules and technical references live under [docs/](docs/) (English). Entry point: [docs/AGENTS.md](docs/AGENTS.md).

## User docs language policy

- English: this file (`README.md`)
- Chinese: [README.zh-CN.md](README.zh-CN.md)
- Any user-facing doc change must update **both** files in sync
