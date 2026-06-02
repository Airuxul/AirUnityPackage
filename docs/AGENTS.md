# AGENTS

**Last Updated:** 2026-06-02 · **Owner:** meta-repo · **Scope:** canonical agent entrypoint (English)

## Canonical Agent Spec

This file is the canonical entrypoint for agent-side documentation rules in this repository.
If any other file conflicts with this file, this file wins.

## Scope

| Track | Location | Language |
|-------|----------|----------|
| User docs | `README.md`, `README.zh-CN.md` | English + Chinese (must stay in sync) |
| Agent markdown | `docs/*.md` | English — **meta repo only** (clone, registry, doc governance) |
| Package C# development | `.cursor/rules/*.md`, `unity-package-develop.mdc` | Standards for `packages/**/*.cs` — **not** under `docs/` |
| Agent data | `config/*.json` | JSON (machine-readable) |

## Agent data files (`config/`)

| File | Purpose |
|------|---------|
| `config/registry.json` | UPM package index, `installDefault`, submodule paths — **source of truth** for install |
| `config/package-tags.json` | Cross-package feature tag index |
| `config/manifest.example.json` | Example `file:` dependency paths for manual Unity manifest edits |

`tools/install-packages.ps1` reads `config/registry.json`. Do not duplicate registry tables in user README unless install workflow changes.

## Per-package documentation (submodules)

Each UPM **Git submodule** under `packages/` is its own repository and must include:

| File | Purpose |
|------|---------|
| `README.md` | English user docs |
| `README.zh-CN.md` | Chinese user docs (keep in sync with English) |
| `docs/AGENTS.md` | Canonical agent entry for **that repository** |
| `docs/DOC_GOVERNANCE.md` | Doc workflow for that repository (links to meta standards) |
| `docs/CHANGELOG_AGENT.md` | Agent-side changelog for that repository |
| `*/DESIGN.md` | Optional module design when README is too long |

**Cursor skills** (`doc-read-index`, `doc-generate-update`) exist **only** in this meta repo (`.cursor/skills/`). Do **not** copy skills into package submodules. Package doc updates are driven from the meta repo using those skills.

Do not add `config/README.md` or `packages/README.md` in the meta repo. Avoid duplicate `QUICKSTART.md` when steps fit in README.

## Document index

| Document | Content |
|----------|---------|
| [AGENTS.md](AGENTS.md) | This file — rules and index |
| [DOC_GOVERNANCE.md](DOC_GOVERNANCE.md) | Doc workflow, hooks, metadata |
| [STRUCTURE.md](STRUCTURE.md) | Directory layout |
| [WORKFLOW.md](WORKFLOW.md) | Clone, install, submodule, doc updates |
| [TOOLS.md](TOOLS.md) | PowerShell tools reference |
| [PACKAGES.md](PACKAGES.md) | Submodule index (human-readable; sync with registry) |
| [CHANGELOG_AGENT.md](CHANGELOG_AGENT.md) | Agent-side change log |

**Unity package C# development** (layers, constraints, C# layout, tags): [`.cursor/rules/README.md`](../.cursor/rules/README.md) — do not duplicate in `docs/`.

## Cursor skills

| Skill | Use |
|-------|-----|
| `doc-read-index` | Read-only inventory, gaps, language parity |
| `doc-generate-update` | Apply doc changes with dual-track rules |
| `unity-cmd` | Unity Editor/Player automation via `unity-cmd` CLI |

Project path: `.cursor/skills/<name>/SKILL.md` (all skills live here only; not under `packages/*/`)

## Required reads before any doc update

1. `docs/AGENTS.md` (this file)
2. `docs/DOC_GOVERNANCE.md`
3. `README.md`
4. `README.zh-CN.md`
5. `docs/DOC_GOVERNANCE_OVERRIDE.md` (optional)
6. `config/registry.json` / `config/package-tags.json` when packages or tags change

## Language policy

- New agent markdown in `docs/` should be English.
- User docs must maintain English (`README.md`) and Chinese (`README.zh-CN.md`) together.

## Dual-track update rule

When documentation changes:

- User-facing changes → update both README files **and** relevant `docs/*.md` (unless exempt).
- Agent-only `docs/*.md` changes → update user README only if user-visible behavior changes.
- `config/*.json` changes → update `docs/` when behavior or indexes change; README only if user workflow changes.

## Exemption rule

Mechanical fixes (typos, link anchors) may skip dual-track updates if the commit message states the exemption reason.
