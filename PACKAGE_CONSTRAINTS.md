# CustomPackages 新 Package 实现约束（按功能归属）

## 1. 现状分析（基于结构 + 代码职责，不只看依赖）

| 包名 | 现有结构特征 | 当前主要职责（结合代码判断） | 不应承载的逻辑 |
| --- | --- | --- | --- |
| `com.air.GameCore` | 单 asmdef，无 `Editor/Runtime` 分拆，纯 C# 文件 | 平台无关的基础能力：`Singleton<T>`、状态机、基础对象池等 | 任何 `UnityEngine`/`UnityEditor` 依赖、场景对象生命周期逻辑 |
| `com.air.UnityGameCore` | `Runtime/Editor` 分拆 | Unity 运行时基础设施：UI 管理、资源加载、时间系统、Mono 单例、Unity 扩展、编辑器工具 | 非 Unity 的纯算法/通用领域模型；与具体业务域耦合的玩法逻辑 |
| `com.air.GameplayTag` | `Runtime/Editor` 分拆 | Gameplay Tag 领域：Tag 数据结构、查询、数据库、组件、编辑器管理器 | 与 Tag 无关的 UI/资源/流程控制 |
| `com.air.BehaviorTree` | `Runtime/Editor` 分拆，节点分层明确 | 行为树领域：图编辑节点、运行时节点、运行器、节点参数数据 | 通用图框架基础能力（应放 NodeGraphProcessor） |
| `com.air.TimelineKit` | `Runtime/Editor/Samples` | Timeline 扩展领域：Clip/Track/Behaviour 基类、引用导出、动态加载接口 | 通用 UI 框架、AI 行为树、非 Timeline 资源系统 |
| `com.alelievr.NodeGraphProcessor` | `Runtime/Editor` 分拆，底层图框架代码量大 | 图编辑器与图运行时基础设施（通用底座） | 任何项目业务域逻辑（如具体技能、任务、战斗规则） |

结论：当前包划分总体清晰，已经形成“基础层 -> Unity基建层 -> 玩法域层”的结构；后续新增 Package 应继续保持单一职责，避免跨域堆叠。

## 2. 新 Package 的强约束

### 2.1 命名与目录

1. 包名统一：`com.air.<feature-name>`（小写、短横线）。
2. 命名空间统一：`Air.<FeatureName>`，Editor 命名空间追加 `.Editor`。
3. 默认目录模板：

```text
com.air.<feature-name>/
  package.json
  Runtime/
    com.air.<Feature>.Runtime.asmdef
  Editor/
    com.air.<Feature>.Editor.asmdef
  Samples~/               (可选)
  README.md
```

4. 如果包只有运行时能力，可不建 `Editor/`，但必须说明“无编辑器扩展”。

### 2.2 依赖方向（必须单向）

1. 基础层（低层）：`GameCore`、`NodeGraphProcessor.Runtime`。
2. Unity 基建层：`UnityGameCore.Runtime`，可依赖 `GameCore`。
3. 玩法域层：`GameplayTag`、`BehaviorTree`、`TimelineKit`，仅依赖其必要底座。
4. 禁止反向依赖：低层包不得引用高层包。
5. 禁止“横向随意依赖”：域包间依赖必须有明确领域关系和文档说明。

### 2.3 Runtime / Editor 边界

1. `Runtime` 程序集禁止直接引用 `UnityEditor` 命名空间或 API。
2. 必须放在 `Editor` 的代码：`EditorWindow`、`MenuItem`、`PropertyDrawer`、`AssetDatabase`、`PrefabUtility` 等。
3. 必须放在 `Runtime` 的代码：玩家构建中运行的逻辑、`MonoBehaviour`、数据结构、运行时接口。
4. 仅允许极少量 `#if UNITY_EDITOR` 作为运行时调试钩子，不得以此代替结构拆分。

### 2.4 功能归属决策表（新增逻辑放哪里）

1. 纯 C#、可脱离 Unity 运行：放 `com.air.GameCore` 或新增纯逻辑包。
2. Unity 生命周期、场景对象、资源加载、UI基建：放 `com.air.UnityGameCore/Runtime`。
3. Gameplay Tag 数据与匹配规则：放 `com.air.GameplayTag/Runtime`。
4. Tag 编辑器窗口、Tag 选择器：放 `com.air.GameplayTag/Editor`。
5. 行为树节点编辑、导出、图可视化：放 `com.air.BehaviorTree/Editor`。
6. 行为树运行时执行、节点状态流转：放 `com.air.BehaviorTree/Runtime`。
7. Timeline Clip/Track 扩展与运行时加载：放 `com.air.TimelineKit/Runtime`。
8. Timeline 资源引用扫描与导出工具：放 `com.air.TimelineKit/Editor`。
9. 通用图框架能力（与业务无关）：放 `com.alelievr.NodeGraphProcessor`。

### 2.5 单一职责与复用规则

1. 包内只保留“同一领域核心能力 + 必要适配层”。
2. 新功能若与现有包主责不一致，必须新建包，不得硬塞。
3. 可复用逻辑优先下沉到底层包（例如非 Unity 逻辑下沉到 `GameCore`）。
4. 同类基础能力不得重复实现（例如纯对象池重复写在多个包）。

### 2.6 API 与可维护性

1. 跨包仅暴露稳定公共 API，内部实现用 `internal`。
2. 每个包必须提供最小 README：用途、依赖、入口 API、示例。
3. 新增对外 API 时，必须补充调用示例或 Samples。

## 3. 禁止项（硬性）

1. 在 `Runtime` asmdef 中引入 `UnityEditor`。
2. 在基础层包中引用上层域包。
3. 在第三方底座包中加入项目业务逻辑。
4. 在无领域关联的包中增加“顺手工具类”导致包职责漂移。

## 4. 新 Package 提交流程（建议作为 PR 门禁）

1. 提交前自检：目录模板、asmdef 分层、依赖方向、命名空间是否符合本规范。
2. 扫描检查：`Runtime` 下是否出现 `using UnityEditor`。
3. 评审检查：新增代码是否放入正确包，是否出现“跨域堆叠”。
4. 文档检查：`package.json`、README、样例是否齐全。

## 5. 评审清单（可复制到 PR）

- [ ] 包名和命名空间符合规范。
- [ ] `Runtime` 与 `Editor` 已正确拆分。
- [ ] 依赖方向符合“低层不反向依赖高层”。
- [ ] 新增逻辑放在对应领域包内。
- [ ] `Runtime` 无 `UnityEditor` 引用。
- [ ] README/示例已更新。

## 6. 给 AI 代码生成的硬规则（可直接复制）

```text
[目标]
在 CustomPackages 内新增/修改代码时，必须把功能逻辑放进正确的包，不允许跨域堆叠。

[硬约束]
1) Runtime 禁止使用 UnityEditor / UnityEditor.* API。
2) Editor 代码必须放在 Editor 目录和 Editor asmdef。
3) 纯 C#、可脱离 Unity 的逻辑放 GameCore，不放 UnityGameCore。
4) Unity 生命周期/资源/UI 基建逻辑放 UnityGameCore。
5) GameplayTag 相关逻辑只放 GameplayTag 包。
6) BehaviorTree 相关逻辑只放 BehaviorTree 包。
7) Timeline 扩展相关逻辑只放 TimelineKit 包。
8) 通用图框架能力只放 NodeGraphProcessor，不放业务逻辑。
9) 依赖方向必须单向：低层不能依赖高层。
10) 新增对外 API 时必须同步更新 README（用途、依赖、示例）。

[生成前自检]
- 我写的代码是否放在了正确的包？
- Runtime 是否出现了 UnityEditor 引用？
- 是否引入了反向依赖/无关横向依赖？
- 是否把可复用基础逻辑下沉到低层包？
```

## 7. AI 生成功能归属速查

| 需求描述关键词 | 应放包 |
| --- | --- |
| 状态机、纯对象池、纯算法工具 | `com.air.GameCore` |
| MonoBehaviour 生命周期、UI 管理、资源加载、计时器 | `com.air.UnityGameCore/Runtime` |
| 自定义 Inspector、EditorWindow、菜单命令 | `com.air.UnityGameCore/Editor` 或对应包 `Editor` |
| Tag 定义、Tag 查询、Tag 组件 | `com.air.GameplayTag/Runtime` |
| Tag 管理器窗口、Tag PropertyDrawer | `com.air.GameplayTag/Editor` |
| 行为树节点编辑、图导出、图窗口 | `com.air.BehaviorTree/Editor` |
| 行为树运行时节点、执行器 | `com.air.BehaviorTree/Runtime` |
| Timeline Clip/Track/Playable 扩展、引用加载 | `com.air.TimelineKit/Runtime` |
| Timeline 资源扫描与导出工具 | `com.air.TimelineKit/Editor` |
| 图框架基础设施（无业务语义） | `com.alelievr.NodeGraphProcessor` |
