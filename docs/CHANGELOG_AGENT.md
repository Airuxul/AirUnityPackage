# Agent Changelog

## 2026-06-03

- TODO dual-track: user `TODO.zh-CN.md` (repo root, Chinese) ↔ agent `docs/TODO.md` / `docs/TODO_ROADMAP.md` (English); `doc-generate-update` and `doc-read-index` skills updated
- Meta user rollup: root `TODO.zh-CN.md`; per-package `TODO.zh-CN.md` (8 packages) with synced IDs
- Multi-agent pass: per-package `docs/TODO.md` (8 packages) for existing-feature optimization backlog with boundary tables
- Meta coordinator: `docs/TODO_ROADMAP.md` — integrated P0 order, P1 themes, anti-patterns
- `docs/AGENTS.md`: index entries for `TODO_ROADMAP.md` and per-package `docs/TODO.md` requirement
- Package `docs/AGENTS.md` tables link local `TODO.md`

## 2026-06-02

- Added canonical agent entrypoint: `docs/AGENTS.md`
- Added governance baseline: `docs/DOC_GOVERNANCE.md`
- User docs: `README.md` (English) + `README.zh-CN.md` (Chinese); removed `README.en.md`
- Introduced dual-skill model: `doc-read-index`, `doc-generate-update`
- Added `tools/validate-docs.ps1` and `.githooks/pre-commit` (enable via `tools/install-git-hooks.ps1`)
- Restored `config/` with `registry.json`, `package-tags.json`, `manifest.example.json`; install script uses JSON again
- Expanded user READMEs (layout, tools, troubleshooting, manual manifest)
- Added agent docs: `WORKFLOW.md`, `STRUCTURE.md`, `TOOLS.md`; refreshed `AGENTS.md` document index
- Package docs pass: standardized `packages/*/README.md` (+ zh-CN where needed), removed `com.air.gameplay-tag/QUICKSTART.md`, dropped orphan `Core/Commands/DESIGN.md` under unity-cli connector
- Per-submodule Git repos: parallel agents added `docs/AGENTS.md`, `docs/DOC_GOVERNANCE.md`, `docs/CHANGELOG_AGENT.md` in all 8 packages; skills remain meta-only (removed package-local `doc-*` skills under `com.air.game-core`)
- Split `docs/` vs `.cursor/rules/`: package dev standards (`PACKAGE_ARCHITECTURE`, `PACKAGE_CONSTRAINTS`, `PACKAGE_TAGS`, `C_SHARP_STANDARDS`) live under `.cursor/rules/`; `docs/` stubs redirect; meta `docs/` index no longer lists dev rules
- `unity-package-develop.mdc`: require updating boundary docs in `.cursor/rules/` (and `config/package-tags.json`) when code boundaries change
- Moved `unity-cmd` skill to `.cursor/skills/unity-cmd/SKILL.md` (English only; removed references/guide)
