---
name: doc-generate-update
description: Generate and update documentation using dual-track governance. Use when README, docs, or TODO backlog changes require synchronized user (Chinese) and agent (English) updates.
disable-model-invocation: true
---

# Doc Generate Update

## Purpose

Apply documentation changes with enforced dual-track consistency. Skills run from the **AirUnityPackage meta repo only**; when updating a package submodule, edit files under `packages/<name>/` but do not add `.cursor/skills/` inside the submodule.

## Prerequisite

Run `doc-read-index` logic first (same required-read chain) and use its output as input.

## Required read order

1. `docs/AGENTS.md`
2. `docs/DOC_GOVERNANCE.md`
3. `README.md`
4. `README.zh-CN.md`
5. `docs/DOC_GOVERNANCE_OVERRIDE.md` (if present)
6. When TODO is in scope: package `docs/TODO.md` (English) and root `TODO.zh-CN.md` (Chinese), or meta `docs/TODO_ROADMAP.md` + root `TODO.zh-CN.md`

## TODO dual-track (mandatory when backlog changes)

| Track | Path | Language | Audience |
|-------|------|----------|----------|
| **User** | `<repo-root>/TODO.zh-CN.md` (same directory as `README.md`) | **Chinese only** | Humans, product/planning |
| **Agent** | `docs/TODO.md` (package submodule) or `docs/TODO_ROADMAP.md` (meta repo) | **English only** | Agents, cross-package coordination |

### Meta repo (AirUnityPackage)

| File | Role |
|------|------|
| `TODO.zh-CN.md` | User-facing integrated backlog (Chinese) |
| `docs/TODO_ROADMAP.md` | Agent-facing integrated backlog (English); boundary rules, P0 order |

### Package submodule

| File | Role |
|------|------|
| `TODO.zh-CN.md` | User-facing package backlog (Chinese); same folder as `README.md` / `README.zh-CN.md` |
| `docs/TODO.md` | Agent-facing package backlog (English); IDs, boundaries, “do not assign here” |

### TODO update rules

1. **Same IDs** in `TODO.zh-CN.md` and `docs/TODO.md` (e.g. `GC-01`, `UGC-01`). Titles/descriptions are translated, not renumbered.
2. **User track** may omit agent-only columns (e.g. long “out of scope” tables) but must keep a short **边界** section pointing to owning packages.
3. **Agent track** is the source for cross-package order; when P0 order changes, update **both** meta `docs/TODO_ROADMAP.md` and root `TODO.zh-CN.md`.
4. **Do not** put Chinese in `docs/TODO.md` or English user TODO at repo root (no `TODO.md` next to README — use `TODO.zh-CN.md` only for users).
5. **Do not** duplicate full TODO tables into `README.md`; link from README to `TODO.zh-CN.md` when the package advertises a backlog (optional one-line link).
6. Bump **Last Updated** on every touched TODO file.
7. After TODO edits in a package, append `docs/CHANGELOG_AGENT.md` in that repo when the change is non-trivial.

### TODO impact scope

| Change | Update |
|--------|--------|
| New/closed item in one package | That package’s `TODO.zh-CN.md` + `docs/TODO.md`; meta rollup if P0/P1 order or boundaries change |
| Cross-package priority shift | Meta `docs/TODO_ROADMAP.md` + `TODO.zh-CN.md` |
| User-visible feature completed | User `TODO.zh-CN.md` (mark done or remove row); agent `docs/TODO.md`; README only if behavior/docs promise changed |

## Update workflow

1. Identify impact scope:
   - user-only, agent-only, dual, or **todo-dual** (see above)
2. Update root user docs:
   - keep `README.md` and `README.zh-CN.md` aligned
3. Update agent docs under `docs/` in English; update `config/registry.json` / `config/package-tags.json` when package or tag data changes; keep `docs/PACKAGES.md` in sync with registry when package list changes (do not copy registry into user README unless workflow changes)
4. If TODO backlog changed: apply **TODO dual-track** section
5. For `unity-cmd` automation rules, edit `.cursor/skills/unity-cmd/` only (not `packages/unity-cli/docs/unity-cmd-skill/`)
6. Update `docs/CHANGELOG_AGENT.md` for non-trivial agent doc changes
7. Produce summary with:
   - `Scope`
   - `UserImpact`
   - `AgentImpact`
   - `TodoImpact` (if any)
   - `Validation`

## Rules

- Do not skip user language parity for README pair.
- Do not skip agent/user dual-track sync for README ↔ `docs/*.md` unless explicitly exempt.
- Do not skip TODO dual-track when backlog items are added, removed, reprioritized, or reworded for users.
- If required files are missing, stop and output a missing-file report.
