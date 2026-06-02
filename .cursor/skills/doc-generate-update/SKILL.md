---
name: doc-generate-update
description: Generate and update documentation using dual-track governance. Use when README or docs change requests require synchronized updates to root user docs and docs agent docs.
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

## Update workflow

1. Identify impact scope:
   - user-only, agent-only, or dual
2. Update root user docs:
   - keep `README.md` and `README.zh-CN.md` aligned
3. Update agent docs under `docs/` in English; update `config/registry.json` / `config/package-tags.json` when package or tag data changes; keep `docs/PACKAGES.md` in sync with registry when package list changes (do not copy registry into user README unless workflow changes)
4. For `unity-cmd` automation rules, edit `.cursor/skills/unity-cmd/` only (not `packages/unity-cli/docs/unity-cmd-skill/`)
5. Update `docs/CHANGELOG_AGENT.md` for non-trivial agent doc changes
6. Produce summary with:
   - `Scope`
   - `UserImpact`
   - `AgentImpact`
   - `Validation`

## Rules

- Do not skip user language parity.
- Do not skip agent/user dual-track sync unless explicitly exempt.
- If required files are missing, stop and output a missing-file report.
