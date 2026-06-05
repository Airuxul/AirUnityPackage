---
name: doc-read-index
description: Read and index repository documentation state. Use when the user asks to inspect docs, evaluate consistency, detect missing docs, or prepare doc updates before writing.
disable-model-invocation: true
---

# Doc Read Index

## Purpose

Read documentation inputs and produce a structured status report without writing files. Operates from the **meta repo**; package submodules have their own `docs/AGENTS.md` but no per-package Cursor skills.

## Required read order

1. `docs/AGENTS.md`
2. `docs/DOC_GOVERNANCE.md`
3. `README.md`
4. `README.zh-CN.md`
5. `docs/DOC_GOVERNANCE_OVERRIDE.md` (if present)
6. `config/registry.json` and `config/package-tags.json` when indexing packages or tags
7. `.cursor/skills/unity-cmd/` when Unity CLI automation is in scope
8. As needed: `docs/STRUCTURE.md`, `docs/WORKFLOW.md`, `docs/TOOLS.md`, `docs/PACKAGES.md`
9. **TODO tracks** (when indexing backlog):
   - Meta: `TODO.zh-CN.md` (user), `docs/TODO_ROADMAP.md` (agent)
   - Per package in `config/registry.json`: `<package-root>/TODO.zh-CN.md`, `docs/TODO.md`

## Output format

Return sections in this order:

1. `Inventory`
2. `Gaps`
3. `LanguageParity`
4. `TodoParity` (ID alignment between `TODO.zh-CN.md` and `docs/TODO.md`; meta rollup vs packages)
5. `SuggestedActions`

## TODO parity checks (report only)

| Check | Pass criteria |
|-------|----------------|
| User TODO exists | Each indexed package has `TODO.zh-CN.md` next to `README.md` |
| Agent TODO exists | Each indexed package has `docs/TODO.md` (English) |
| ID sync | Same IDs and priorities in both files per package |
| Meta rollup | `docs/TODO_ROADMAP.md` P0 table matches meta `TODO.zh-CN.md` integrated P0 |
| Language | No Chinese inside `docs/TODO.md` / `docs/TODO_ROADMAP.md`; no English user TODO at root except `README.md` |

## Rules

- Do not modify any file.
- If any required file is missing, stop and report the missing file list.
- Treat `docs/AGENTS.md` as canonical if conflicts are found.
