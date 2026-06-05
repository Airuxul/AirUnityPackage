# Documentation Governance

**Last Updated:** 2026-06-03 · **Owner:** meta-repo · **Scope:** agent documentation (English)

## Purpose

Define a stable, enforceable documentation workflow for both users and agents.

## Document layout

| Track | Paths |
|-------|--------|
| User | `README.md` (EN), `README.zh-CN.md` (ZH), `TODO.zh-CN.md` (ZH backlog, repo root) |
| Agent markdown | `docs/*.md` (English), including `docs/TODO.md` / `docs/TODO_ROADMAP.md` |
| Agent data | `config/registry.json`, `config/package-tags.json`, `config/manifest.example.json` |
| Skills | `.cursor/skills/doc-read-index`, `.cursor/skills/doc-generate-update`, `.cursor/skills/unity-cmd` |
| Package C# rules | `.cursor/rules/` (outside `docs/` dual-track; see [README](../.cursor/rules/README.md)) |

## Metadata (recommended)

Each managed markdown file should include near the top:

- **Last Updated** (date)
- **Owner** (team or `meta-repo`)
- **Scope** (user vs agent)

## Update workflow

1. Read [AGENTS.md](AGENTS.md) and this file.
2. Run `doc-read-index` logic when unsure what is out of date.
3. Identify impact: user / agent markdown / `config/` JSON.
4. Apply updates (bilingual user docs when user-visible).
5. Run `.\tools\validate-docs.ps1` or commit via hook-enabled clone.
6. Append summary to [CHANGELOG_AGENT.md](CHANGELOG_AGENT.md).

## Validation contract

`tools/validate-docs.ps1` (via `.githooks/pre-commit`):

| Rule | Detail |
|------|--------|
| User language parity | `README.md` and `README.zh-CN.md` staged together |
| Dual-track (markdown) | User README changes require `docs/*.md` change, and vice versa |
| Config-only | `config/**` changes do not trigger README pairing |
| Required files | `docs/AGENTS.md`, `docs/DOC_GOVERNANCE.md`, both READMEs exist |

### Hook setup (one-time per clone)

```powershell
.\tools\install-git-hooks.ps1
```

## Scope split: `docs/` vs `.cursor/rules/`

| Area | Location |
|------|----------|
| Meta repo (this repository) | `docs/*.md`, `config/*.json`, root README |
| Unity UPM package C# development | `.cursor/rules/*.md`, `unity-package-develop.mdc` |

Changes under `.cursor/rules/` do not trigger the README ↔ `docs/*.md` dual-track rule unless user-visible install or workflow text changes.

## Legacy Chinese in `docs/`

[PACKAGES.md](PACKAGES.md) may remain Chinese. Prefer English for **new** meta-repo agent documents.
