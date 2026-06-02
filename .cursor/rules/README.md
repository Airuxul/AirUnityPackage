# Unity package development rules

**Scope:** C# under `packages/**/*.cs` in this meta repo and its UPM submodules.

**Not in this folder:** Meta-repo workflow (clone, submodules, `config/registry.json`, doc governance) — see [`docs/AGENTS.md`](../../docs/AGENTS.md).

## Entry

| File | Role |
|------|------|
| [unity-package-develop.mdc](unity-package-develop.mdc) | Cursor rule: workflow, `unity-cmd` skill, mandatory `unity-compile-loop.ps1` |
| [C_SHARP_STANDARDS.md](C_SHARP_STANDARDS.md) | File layout, naming, per-package folder templates (§4), checklist (§9) |
| [PACKAGE_ARCHITECTURE.md](PACKAGE_ARCHITECTURE.md) | Layers, dependency graph, ownership |
| [PACKAGE_CONSTRAINTS.md](PACKAGE_CONSTRAINTS.md) | Package duties, v2 API, PR checklist |
| [PACKAGE_TAGS.md](PACKAGE_TAGS.md) | Feature tag dictionary; machine index: `config/package-tags.json` |

Read **C_SHARP_STANDARDS §4** + **PACKAGE_ARCHITECTURE** before adding files in an unfamiliar package.

## Living documents

These files describe **code boundaries**. They must stay aligned with `packages/` — update them in the same PR/task when boundaries change (see [unity-package-develop.mdc](unity-package-develop.mdc) § *Keep boundary docs in sync*).

| Trigger | Files |
|---------|--------|
| Ownership / dependency graph | `PACKAGE_ARCHITECTURE.md` |
| Duties, forbidden refs, v2 API | `PACKAGE_CONSTRAINTS.md` |
| Folder templates, naming layout | `C_SHARP_STANDARDS.md` |
| Tags / module slices | `PACKAGE_TAGS.md` + `config/package-tags.json` |
