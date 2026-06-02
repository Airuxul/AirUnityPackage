# 功能 Tag（跨包拆分索引）

各 UPM 包内的模块按 **Tag** 标记归属域。后续可将同一 Tag 的代码迁到独立包，或按 Tag 生成依赖图。

**约定：**

- Tag 为稳定标识符（`kebab-case`），写在 README 模块表、源文件顶部 `// @tag <id>`，或 `config/package-tags.json`。
- 一个文件夹/类型可带多个 Tag；拆包时以**主 Tag** 为准。
- `com.air.unity-connector` 内 **CLI 专属** 逻辑（含 `CliParam`）使用 `cli-*` Tag，不进入 L1 基建。

---

## Tag 词典

| Tag | 说明 | 当前包 |
|-----|------|--------|
| `core-foundation` | 单例、纯 C# 对象池 | `com.air.game-core` |
| `core-fsm` | 有限状态机 / 分层状态机 | `com.air.game-core` |
| `core-command` | GoF 命令 / 撤销栈（`ICommand`, `CommandHistory`） | `com.air.game-core` |
| `core-entity` | `EntityId`、`EntityWorld`、`IComponent`、`IEntityTickable`、`IEntitySystem` | `com.air.game-core` |
| `unity-entity` | `EntityWorldHost`、`UnityEntityView`、`EntityWorldUpdater`、内置组件/系统 | `com.air.unity-game-core` |
| `unity-scene` | `ISceneFlow`、`SceneFlow` | `com.air.unity-game-core` |
| `unity-save` | `ISaveService`、`FileSaveService` | `com.air.unity-game-core` |
| `unity-audio` | `IAudioService`、`AudioService` | `com.air.unity-game-core` |
| `ui-navigation` | `UIPanelNavigator`、面板栈 | `com.air.unity-ui` |
| `cli-command` | 远程命令发现/执行、`CommandState`、`CommandNames` 与具体命令实现 | `com.air.unity-connector` |
| `cli-http` | `HttpServer`、`CommandPipeline`、`CommandHttpHelper`、连接器路由 | `com.air.unity-connector` |
| `cli-network` | 连接器主机类型、鉴权、监听配置 | `com.air.unity-connector` |
| `unity-input` | Unity 输入采样、`InputPipeline` → `CommandHistory` | `com.air.unity-game-core` |
| `unity-runtime` | `GameRuntime` / `IGameRuntime` | `com.air.unity-game-core` |
| `unity-event` | `EventBus` | `com.air.unity-game-core` |
| `unity-resource` | 资源加载、AssetBundle | `com.air.unity-game-core` |
| `unity-timer` | 定时器 | `com.air.unity-game-core` |
| `unity-pool` | Unity 对象池 | `com.air.unity-game-core` |
| `unity-serialization` | `IJsonSerializer`、`JsonSerialization`、Newtonsoft 实现与启动注册 | `com.air.unity-game-core` |
| `cli-param` | `CliParam*` 参数绑定（仅 unity-cmd 使用） | `com.air.unity-connector` |
| `cli-http-host` | `ConnectorBuild`、`ConnectorHealthMetadata`、Editor/PlayMode HTTP 宿主 | `com.air.unity-connector` |
| `cli-policy` | 完成策略（编译、进 Play 等） | `com.air.unity-connector` |
| `ui-framework` | UI 面板、状态、事件 | `com.air.unity-ui` |
| `ui-navigation` | `UIPanelNavigator`、面板栈 | `com.air.unity-ui` |
| `domain-gameplay-tag` | Gameplay Tag | `com.air.gameplay-tag` |
| `domain-behavior-tree` | 行为树 | `com.air.unity-behavior-tree` |
| `domain-timeline` | Timeline 扩展 | `com.air.unity-timeline-kit` |

---

## 按包索引（主 Tag）

### `com.air.game-core`

| 路径 | Tag |
|------|-----|
| `Pool/` | `core-foundation` |
| `Singleton.cs` | `core-foundation` |
| `StateMachine/` | `core-fsm` |
| `Command/` | `core-command` |
| `Entity/` | `core-entity` |

### `com.air.unity-game-core`

| 路径 | Tag |
|------|-----|
| `Runtime/Core/` | `unity-runtime` |
| `Runtime/Event/` | `unity-event` |
| `Runtime/Resource/` | `unity-resource` |
| `Runtime/Time/` | `unity-timer` |
| `Runtime/Pool/` | `unity-pool` |
| `Runtime/Serialization/` | `unity-serialization` |
| `Runtime/Input/` | `unity-input` |
| `Runtime/Entity/` | `unity-entity` |
| `Runtime/Scene/` | `unity-scene` |
| `Runtime/Save/` | `unity-save` |
| `Runtime/Audio/` | `unity-audio` |
| `Editor/AssetDependency/` | `unity-editor-tool`（编辑器工具，非 Runtime API） |

### `com.air.unity-connector`

| 路径 | Tag |
|------|-----|
| `Runtime/Params/CliParam*.cs` | `cli-param` |
| `Runtime/Params/*.cs`（业务参数 DTO） | `cli-param` |
| `Editor/Params/` | `cli-param` |
| `Runtime/ConnectorBuild.cs` | `cli-http-host` |
| `Runtime/Connector/Command/`, `Runtime/Connector/CommandState/` | `cli-command` |
| `Runtime/Connector/Http/`, `Runtime/Connector/Network/` | `cli-http`, `cli-network` |
| `Runtime/Commands/CommandNames.cs` | `cli-command` |
| `Runtime/Commands/`, `Editor/Commands/` | `cli-command` |
| `Runtime/Http/`, `Editor/Http/` | `cli-http-host` |
| `Editor/Completion/` | `cli-policy` |
| `Runtime/Compatibility/ConnectorJson.cs` | `cli-param`（兼容垫片，依赖 `unity-serialization`） |

---

## 拆包示例（未来）

| 目标包 | 迁入 Tag |
|--------|----------|
| `com.air.command-core`（假想） | `cli-command`, `cli-http`, `unity-serialization` |
| `com.air.unity-cli`（已存在 connector） | `cli-*` 全部 |

依赖方向保持不变：`cli-*` → `unity-serialization` → `core-*`。
