# C# 代码规范与目录结构

**适用范围：** `packages/` 下所有 UPM 子模块中的 C# 源码。  
**优先级：** 与 [CONSTRAINTS.md](CONSTRAINTS.md)、[ARCHITECTURE.md](ARCHITECTURE.md) 同级；修改代码前必须同时遵守。

**目标：** 结构清晰、按文件夹分域、便于 AI 与人工在正确位置增删改。

---

## 1. 文件与类型

| 规则 | 说明 |
|------|------|
| 一文件一类型 | 每个 `.cs` 仅包含一个主要 `class` / `struct` / `interface` / `enum`（`partial` 除外） |
| 文件名 | 与类型名一致：`GameRuntime.cs` → `class GameRuntime` |
| 文件长度 | 单文件建议 ≤ 400 行；超出则按职责拆文件或子文件夹 |
| 编码与换行 | UTF-8 无 BOM、LF；缩进 4 空格（见仓库 `.editorconfig`） |
| `.meta` | 与源文件同目录；新增 `.cs` 后由 Unity 生成，勿手写 GUID |

---

## 2. 命名约定

| 元素 | 风格 | 示例 |
|------|------|------|
| 命名空间 | PascalCase，与程序集/功能一致 | `Air.UI`、`Air.UnityGameCore.Runtime.Event` |
| 类 / 结构 / 枚举 | PascalCase | `UIFramework`、`GameplayTag` |
| 接口 | `I` 前缀 | `IGameRuntime`、`IResManager` |
| 公开成员 | PascalCase | `CreateWithUI()` |
| 私有字段 | `_camelCase` | `_currentState` |
| 静态只读 / 常量 | PascalCase 或 `k` 前缀（项目内统一即可） | `UIEvents.PanelShown` |
| 泛型参数 | `T` / `TKey` | `ObjectPool<T>` |
| 异步方法 | `Async` 后缀 | `LoadAssetAsync` |

**禁止：** 匈牙利命名（`strName`）、无意义缩写（`Mgr` 仅当类型名已为 `UIManager` 等约定名时可用）。

---

## 3. UPM 包根目录（强制）

每个包仓库根目录固定为：

```text
<package-root>/
├── package.json
├── README.md
├── Runtime/                    # 玩家构建可引用（或纯 C# 包的全部代码）
│   ├── Resources/              # 可选：运行时 Resources.Load 资源
│   └── *.asmdef
├── Editor/                     # 仅编辑器；可选
│   ├── Resources/              # 可选：编辑器 USS/UXML/图标等
│   └── *.asmdef
├── Tests/                      # 可选：Edit Mode / Play Mode 测试
│   └── Editor/                 # 常见：仅编辑器测试 + Tests.Editor.asmdef
├── Samples/                    # 可选示例
└── ...
```

| 目录 | 内容 |
|------|------|
| `Runtime/` | 运行时代码、Runtime 程序集定义 |
| `Runtime/Resources/` | 需 `Resources.Load` 的运行时资源（贴图、Prefab、ScriptableObject 等） |
| `Editor/` | `UnityEditor`、Inspector、生成器、分析窗口 |
| `Editor/Resources/` | 编辑器专用资源（USS、UXML、图标；不进玩家包体时可放此处） |
| `Tests/` | 测试程序集；`Tests/Editor/*.asmdef` 通常 `includePlatforms: ["Editor"]`，`optionalUnityReferences: ["TestAssemblies"]` |
| `Samples/` | 示例场景/脚本，不进入核心 API |

**禁止：**

- 在包根目录散落 `.cs`（除极小的纯 C# 包见 §4.1）
- 在 `Runtime/` 使用 `UnityEditor`
- 在 `Editor/` 放玩家循环必需逻辑
- 用 `Scripts/`、`Code/` 等模糊目录代替 `Runtime/` / `Editor/`
- **保留无任何文件的空文件夹**（无 `.cs`、无资源、无 `.meta` 的目录应删除）

---

## 4. 按包类型的文件夹模板

### 4.1 纯 C# 包 — `com.air.game-core`

无 `Runtime/` 包裹亦可；若使用根目录 + 功能文件夹：

```text
com.air.game-core/
├── package.json
├── com.air.GameCore.asmdef      # noEngineReferences: true
├── Pool/
├── StateMachine/
│   └── Layered/
├── Command/                     # GoF: ICommand, CommandHistory, CompositeCommand
├── Entity/                      # EntityId, EntityWorld, IComponent
└── Singleton.cs
```

- **命名空间：** `Air.GameCore`、`Air.GameCore.Command`、`Air.GameCore.StateMachine`、`Air.GameCore.Entity`
- **禁止：** `UnityEngine`、HTTP、JSON、CLI、输入采样
- **Tag 索引：** [PACKAGE_TAGS.md](PACKAGE_TAGS.md)
- **禁止：** 任何 `using UnityEngine`

### 4.1.1 Connector — `com.air.unity-connector`（CLI 专属）

```text
Runtime/Connector/Command/       # tag: cli-command — 发现、执行、CommandState
Runtime/Connector/Http/          # tag: cli-http — CommandPipeline、路由
Runtime/Connector/Network/       # tag: cli-network
Runtime/Params/                  # tag: cli-param — CliParamAttribute, CliParamBinder
Editor/Params/                   # tag: cli-param — 命令参数 DTO
Runtime/Commands/, Editor/Commands/  # tag: cli-command — CommandNames、具体命令
Runtime/Http/, Editor/Http/      # tag: cli-http-host
```

- **命名空间：** `UnityCliConnector.Params`、`UnityCliConnector.Command`、`UnityCliConnector.Http`、`UnityCliConnector.Commands`
- **禁止：** 将 `CliParam*` 放入 `com.air.unity-game-core`（仅 connector 使用）

### 4.2 Unity 基建 — `com.air.unity-game-core`

```text
Runtime/
├── Core/                        # 入口与门面：GameRuntime, IGameRuntime
├── Event/                       # EventBus
├── Resource/                    # IResManager, loaders
├── Time/                        # Timer, TimerService, ITimerService（无静态 TimerManager）
│   └── TimerImpl/               # 具体实现类
├── Pool/                        # IPool, UnityObjectPool, GameObjectPool, PoolRegistry
├── Entity/                      # EntityWorldHost, UnityEntityView
├── Scene/                       # ISceneFlow, SceneFlow
├── Save/                        # ISaveService, FileSaveService
├── Audio/                       # IAudioService, AudioService
├── Coroutine/                   # WaitFor 等
├── Serialization/               # tag: unity-serialization
├── Input/                       # tag: unity-input — GameInputSystem, UnityLegacyKeyboardSource
├── Utils/                       # 无状态工具（PlayerLoopUtils）
└── com.air.UnityGameCore.Runtime.asmdef

Editor/
├── AssetDependency/             # 编辑器工具成组放子目录
├── Extensions/
├── Utils/
├── Timer/
└── com.air.UnityGameCore.Editor.asmdef
```

- **命名空间：** `Air.UnityGameCore.Runtime.<Folder>`、`Air.UnityGameCore.Editor.<Folder>`
- **新增模块：** 先建文件夹，再建类型；不要把无关工具塞进 `Utils/`

### 4.3 UI 包 — `com.air.unity-ui`

```text
Runtime/
├── GameEntry.cs                 # 包级入口可放 Runtime 根
├── UIFramework.cs
├── UIScopedEvents.cs
├── UIEvents.cs
├── Extensions/
├── UI/
│   ├── UIManager.cs
│   ├── UIPanel.cs
│   ├── State/
│   │   └── Actions/             # 按机制分子目录
│   └── Trigger/
│       └── Actions/
└── com.air.UnityUI.Runtime.asmdef

Editor/
├── UI/                          # 与 Runtime/UI 对应
│   └── Templates/               # 生成模板 (.txt)
└── com.air.UnityUI.Editor.asmdef
```

- **命名空间：** `Air.UI`、`Air.UI.Editor`（编辑器）
- **生成代码：** 仅输出到 `Air.UI.Generated`（由生成器约定，勿与手写 API 混目录）

### 4.4 域包 — behavior-tree / gameplay-tag / timeline-kit

按**领域概念**分文件夹，编辑器与运行时分离：

```text
# 行为树示例
Runtime/
├── Nodes/
│   ├── Base/
│   ├── Control/
│   ├── Action/
│   └── Decorator/
├── NodeParamData/
└── BehaviorTreeRunner.cs

Editor/
├── Nodes/                       # 与 Runtime/Nodes 镜像
├── Views/
└── Resources/                   # USS/USS 等样式
```

- 域包 **默认不依赖** `com.air.unity-game-core`
- 节点类：Editor 用 `BT*` / `*Node`，Runtime 用 `RuntimeBT*`

### 4.5 第三方/图框架 — `node-graph-processor`

保持上游目录习惯；仅在做 Air 定制时遵循本规范命名，避免大范围重排目录。

---

## 5. 程序集 (asmdef)

| 规则 | 说明 |
|------|------|
| 名称 | 与文件一致：`com.air.UnityUI.Runtime.asmdef` → `"name": "com.air.UnityUI.Runtime"` |
| `rootNamespace` | 与主命名空间前缀一致 |
| `references` | 只引用必要程序集；禁止循环引用 |
| Editor asmdef | `includePlatforms`: `["Editor"]` |
| Tests asmdef | 放 `Tests/Editor/`；`includePlatforms`: `["Editor"]`；`optionalUnityReferences`: `["TestAssemblies"]`；`autoReferenced`: `false` |
| 跨包引用 | 仅通过 `package.json` 声明的 UPM 依赖，再在 asmdef 中引用程序集名 |

修改 `package.json` 依赖后，必须同步检查 asmdef `references`。

---

## 6. 分层与依赖（与 CONSTRAINTS 一致）

```text
com.air.game-core
    → com.air.unity-game-core
        → com.air.unity-ui

独立：gameplay-tag | unity-behavior-tree → node-graph-processor | unity-timeline-kit | unity-connector
```

| 要做 | 不要做 |
|------|--------|
| UI 通过 `IGameRuntime` / `EventBus` 与基建交互 | 在 UI 包再实现一套 EventBus |
| 业务入口持有 `GameRuntime` 实例 | 新增全局 `XXXFacade.Instance` |
| 域逻辑放在对应域包 | 把玩法代码塞进 `unity-game-core` |

---

## 7. 代码风格要点

```csharp
// ✅ 类型与文件、命名空间清晰
namespace Air.UnityGameCore.Runtime.Event
{
    public sealed class EventBus { }
}

// ✅ 使用 protected Initialize，避免 State ↔ Machine 循环构造
public sealed class GameStateMachine : StateMachine
{
    public GameStateMachine() => Initialize(new IdleState());
}

// ❌ Runtime 中引用 Editor
#if UNITY_EDITOR  // 仅当极少数共享常量可接受；默认应拆到 Editor 程序集
#endif
```

- 优先 `sealed` 除非设计为继承基类
- 公开 API 加 XML 文档注释（`///`），至少对 public 类与方法
- `using` 顺序：System → Unity → 其他 Air 包 → 本程序集
- 不使用 `#region` 掩盖过长类；应拆文件

---

## 8. 新增功能时的放置决策

按顺序自问：

1. **是否依赖 Unity 引擎？** 否 → `game-core`；是 → 继续  
2. **是否 UI？** 是 → `unity-ui`；否 → 继续  
3. **是否通用运行时基建（事件/资源/定时器/池）？** 是 → `unity-game-core`  
4. **是否独立领域（Tag/行为树/Timeline/CLI）？** 是 → 对应域包  

然后在对应包的 **`Runtime/<Feature>/` 或 `Editor/<Feature>/`** 下新增文件，**不要**放在包根或 `Misc/`。

---

## 9. AI / PR 修改自检清单

修改 `packages/**/*.cs` 完成后必须核对：

- [ ] 新文件位于 [§4](#4-按包类型的文件夹模板) 规定的目录下
- [ ] 命名空间与文件夹、asmdef 一致
- [ ] 未违反 [CONSTRAINTS.md](CONSTRAINTS.md) 分层
- [ ] `Runtime` 无 `UnityEditor` 引用
- [ ] 一文件一类型，文件名正确
- [ ] `package.json` / asmdef 依赖已同步（若新增跨包引用）
- [ ] 无空文件夹（§3）；`Tests/`、`Resources/` 仅在确有内容时使用
- [ ] 文档为 UTF-8（见 User Rule `docs-utf8`）

---

## 10. 相关文档

| 文档 | 内容 |
|------|------|
| [CONSTRAINTS.md](CONSTRAINTS.md) | 包职责、v2 API |
| [ARCHITECTURE.md](ARCHITECTURE.md) | 依赖图、归属速查 |
| [PACKAGES.md](PACKAGES.md) | 子模块索引 |
| [WORKFLOW.md](WORKFLOW.md) | Submodule 工作流 |
