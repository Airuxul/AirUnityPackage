# 包职责与依赖约束（v2）

## 分层

```text
com.air.game-core              纯 C#，noEngineReferences
    ↓
com.air.unity-game-core        GameRuntime + EventBus + 资源 + 定时器 + UnityObjectPool
    ↓
com.air.unity-ui               UIFramework + UIManager + UIScopedEvents（可选）

独立域：gameplay-tag | unity-behavior-tree → node-graph-processor | unity-timeline-kit | unity-connector
```

## 各包职责

| 包 | 职责 | 禁止 |
| --- | --- | --- |
| `com.air.game-core` | Pool、FSM、Procedure、GoF Command、GF Entity、JSON 契约 | Unity、Newtonsoft、CLI 协议 |
| `com.air.unity-game-core` | `GameRuntime`、`EventBus`、async `IResManager`、`UnityEntityManager`、`ProcedureManager`、`JsonHost` 注册 | UI、CLI 命令 |
| `com.air.unity-ui` | `UIFramework`、`UIManager`、`UIScopedEvents`、`GameEntry` | 实现 EventBus / 资源加载 |
| 域包 | 各自领域 | 引用 unity-game-core（除非确有需求） |

## v2 API 约定

1. **无全局单例门面：** 游戏入口持有 `GameRuntime` 实例；UI 通过 `UIFramework.Install(runtime)` 安装。
2. **事件 API：** `EventBus.On` / `Emit` / `Off`；UI 组件用 `UIScopedEvents` + `IUIScopedEvents` 扩展方法。
3. **一键启动：** `GameEntry.CreateWithUI()` 返回 `(Runtime, UI)`。
4. **命名空间：** `Air.<Feature>`；生成代码 `Air.UI.Generated`。

## PR 自检

- [ ] 代码在正确包内（见 [ARCHITECTURE.md](ARCHITECTURE.md)）
- [ ] 目录与命名符合 [C_SHARP_STANDARDS.md](C_SHARP_STANDARDS.md)
- [ ] Runtime 无 `UnityEditor`
- [ ] 依赖单向
- [ ] 未向 UnityGameCore 塞入与基建无关的工具类
