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

## Output format

Return sections in this order:

1. `Inventory`
2. `Gaps`
3. `LanguageParity`
4. `SuggestedActions`

## Rules

- Do not modify any file.
- If any required file is missing, stop and report the missing file list.
- Treat `docs/AGENTS.md` as canonical if conflicts are found.
