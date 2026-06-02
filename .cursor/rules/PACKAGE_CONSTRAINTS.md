# Package responsibilities and dependencies (v2)

**Last Updated:** 2026-06-02 · **Scope:** Unity UPM package development (Chinese)

## Layers

```text
com.air.game-core              Pure C#, noEngineReferences
    ↓
com.air.unity-game-core        GameRuntime + EventBus + resources + timers + UnityObjectPool
    ↓
com.air.unity-ui               UIFramework + UIManager + UIScopedEvents (optional)

Independent domains: gameplay-tag | unity-behavior-tree → node-graph-processor | unity-timeline-kit | unity-connector
```

## Per-package duties

| Package | Responsibility | Forbidden |
|---------|----------------|-----------|
| `com.air.game-core` | Pool, FSM, Procedure, GoF Command, GF Entity, JSON contracts | Unity, Newtonsoft, CLI protocol |
| `com.air.unity-game-core` | `GameRuntime`, `EventBus`, async `IResManager`, `UnityEntityManager`, `ProcedureManager`, JsonHost registration | UI, CLI commands |
| `com.air.unity-ui` | `UIFramework`, `UIManager`, `UIScopedEvents`, `GameEntry` | Implementing EventBus / resource loading |
| Domain packages | Own domain | Reference unity-game-core unless truly required |

## v2 API conventions

1. **No global singleton facade:** Game entry holds a `GameRuntime` instance; UI installs via `UIFramework.Install(runtime)`.
2. **Events:** `EventBus.On` / `Emit` / `Off`; UI components use `UIScopedEvents` + `IUIScopedEvents` extensions.
3. **One-shot bootstrap:** `GameEntry.CreateWithUI()` returns `(Runtime, UI)`.
4. **Namespaces:** `Air.<Feature>`; generated code `Air.UI.Generated`.

## PR self-check

- [ ] Code lives in the correct package (see [PACKAGE_ARCHITECTURE.md](PACKAGE_ARCHITECTURE.md))
- [ ] Folders and naming match [C_SHARP_STANDARDS.md](C_SHARP_STANDARDS.md)
- [ ] Runtime assemblies have no `UnityEditor`
- [ ] Dependencies are one-way
- [ ] No unrelated utility types added to UnityGameCore
