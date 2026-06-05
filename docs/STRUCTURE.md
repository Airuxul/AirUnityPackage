# Repository Structure

**Last Updated:** 2026-06-03 · **Owner:** meta-repo · **Scope:** agent documentation (English)

```text
CustomPackages/
├── packages/                 # Git submodules only (UPM sources)
├── config/
│   ├── registry.json         # Package index, installDefault, repo URLs
│   ├── package-tags.json     # Feature tag index
│   └── manifest.example.json # Example file: paths for Unity manifest
├── tools/
│   ├── install-packages.ps1
│   ├── validate-docs.ps1
│   ├── install-git-hooks.ps1
│   └── unity-compile-loop.ps1
├── docs/                     # Meta-repo agent markdown (English); TODO_ROADMAP.md coordinates package docs/TODO.md
├── .cursor/rules/            # Unity package C# development (not docs/)
├── .cursor/skills/           # doc-read-index, doc-generate-update, unity-cmd
├── .githooks/pre-commit
├── init-submodules.bat
├── install-to-unity.bat
├── README.md                 # User docs (English)
└── README.zh-CN.md           # User docs (Chinese)
```

## Path rules

- **User docs:** repository root `README.md` / `README.zh-CN.md` only.
- **Agent markdown:** `docs/` — meta repo workflow, registry, doc governance only.
- **Package C# rules:** `.cursor/rules/` — see [README](../.cursor/rules/README.md); applied via `unity-package-develop.mdc` for `packages/**/*.cs`.
- **Machine-readable agent data:** `config/*.json` (not duplicated in user README unless workflow changes).

See also [WORKFLOW.md](WORKFLOW.md), [PACKAGES.md](PACKAGES.md).
