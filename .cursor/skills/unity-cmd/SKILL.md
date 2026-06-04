---
name: unity-cmd
description: Operates Unity through unity-cmd and com.air.unity-connector over HTTP. Use when automating the Unity Editor, Editor Play Mode, or a development player, refreshing the command catalog, or editing unity-cli connector code.
---

# unity-cmd

`unity-cmd` (Node CLI) talks to **com.air.unity-connector** in Unity over loopback HTTP. The live command catalog comes from Unity `POST /list` — **never invent commands or flags**.

## Prerequisites (stop if violated)

| # | Rule |
|---|------|
| 1 | Profiles `editor` and `editor-play` must exist (create if missing). |
| 2 | Target profile must respond to `ping`. |
| 3 | Run `list` first; match user intent to `commands[]`. If no match, name ~2 related commands and stop. |
| 4 | If catalog is stale or miss: `list --refresh-catalog`, then match again; still no match → stop. |
| 5 | On `ping` failure, remote command failure, or failed screenshot check → stop. |

**Stop** means: no profile hopping, no invented CLI flags, no guessing catalog entries.

## Workflow

```text
Ensure profiles → list (+ match) → ping → run command → verify (e.g. console / screenshot)
```

1. Pick **profile** from task (see table below).
2. `unity-cmd --profile <name> list` — use returned `commands` and `params` only.
3. `unity-cmd --profile <name> ping`
4. Execute matched command with flags from catalog.
5. CONN-10: `compile`/`play`/`stop` finish in **one POST** (no HTTP 202). On failure: `wait` → retry.
6. Integration from `unity-cmd/`: set `UNITY_CMD_WORKSPACE` to the Unity project root (matches `instances` `projectPath` for `wait`).

Catalog cache: `~/.unity-cmd/cache/catalog-<host>_<port>.json` (CLI-managed). Refresh with `list --refresh-catalog`. Do not maintain command tables in chat or markdown.

## Profiles and scope

| Profile | Port (default) | Use for |
|---------|----------------|---------|
| `editor` | 6547 | `compile`, `play`, `stop`, `console`, `screenshot`, `profiler`, `state`, `menu`, `exec`, … |
| `editor-play` | 6794 | Runtime channel while Editor is in Play (`echo`, etc.) |
| `package-play` | 6795 | Development Build player runtime |

| Command scope | Allowed on `editor` | Allowed on `editor-play` / `package-play` |
|---------------|---------------------|-------------------------------------------|
| `editor` | Yes | No |
| `runtime` | No | Yes |
| `any` | Yes | Yes |

- `play` and `stop` → profile **`editor` only**.
- Game view screenshot while playing → still profile **`editor`**.
- Runtime `echo` during Play → **`editor-play`** (not `editor`).

Create profiles once:

```bash
unity-cmd profile create editor --host 127.0.0.1 --port 6547 --host-kind editor --no-verify
unity-cmd profile create editor-play --host 127.0.0.1 --port 6794 --host-kind editor_play --no-verify
unity-cmd profile create package-play --host 127.0.0.1 --port 6795 --host-kind player --no-verify
```

## Common commands (names only — flags from `list`)

`ping`, `state`, `echo`, `compile` (`recompile`, `reload`), `play`, `stop`, `refresh`, `console`, `menu`, `screenshot`, `exec`, `profiler`, `manage`, `reserialize`.

After connector C# changes: `unity-cmd --profile editor compile` (default timeout **20000** ms).

## Error codes (CLI JSON)

| Code | Action |
|------|--------|
| `NO_PROFILE` | Set `--profile` or `UNITY_CMD_PROFILE` |
| `NO_INSTANCE` | Open Unity / check port and profile |
| `SCOPE_MISMATCH` | Wrong profile for command scope |
| `CONNECTION_FAILED` | Unity HTTP not reachable |
| `CATALOG_FETCH_FAILED` | Fix Unity / connector, retry `list` |
| `SERVER_BUSY` | Wait; one command at a time per host |
| `DOMAIN_RELOADING` | Retry after domain reload |
| `COMMAND_FAILED` | Read `hint`; run `console` on editor profile |

## Examples

**Compile check after C# edits (meta repo):**

```powershell
.\tools\unity-compile-loop.ps1 -IncludeWarnings -ConsoleLines 400
```

Or manually:

```bash
unity-cmd --profile editor list
unity-cmd --profile editor ping
unity-cmd --profile editor compile --timeout 20000
unity-cmd --profile editor console --type error,warning --lines 30
```

**Play + screenshot:**

```bash
unity-cmd --profile editor ping
unity-cmd --profile editor play
unity-cmd --profile editor screenshot --view game --output_path Screenshots/play.png
unity-cmd --profile editor stop
```

## Security

- Default bind: loopback only. Use `UNITY_CMD_BIND=lan` only on trusted networks.
- Optional token: `UNITY_CMD_TOKEN` / header `X-Unity-Cmd-Token`.
- `exec`, `menu`, `manage` can change project state — confirm with user first.

## More detail

Package docs: `packages/unity-cli/docs/AGENTS.md`, `ARCHITECTURE.md`, `com.air.unity-connector/docs/IMPLEMENTATION.md`.
