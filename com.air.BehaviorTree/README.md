# Behavior Tree (`com.air.behavior-tree`)

基于 [Node Graph Processor](../com.alelievr.NodeGraphProcessor/README.md) 的行为树包，用于 AI 与游戏逻辑。

## 功能

- **控制节点：** Root、Sequence、Selector、Parallel
- **装饰节点：** Invert
- **动作节点：** Log、Wait
- 可视化编辑器（继承 Node Graph Processor）
- JSON 导出与 `BehaviorTreeRunner` 运行时执行

## 安装

```json
"com.air.behavior-tree": "file:../CustomPackages/com.air.BehaviorTree"
```

自动依赖 `com.alelievr.node-graph-processor` 1.3.1+；manifest 中需能解析到 Node Graph Processor 包路径。

## 使用

### 创建行为树

1. 右键 → **Create → Behavior Tree → Graph**
2. 双击打开，或 **Window → Behavior Tree → Open Graph**
3. 右键添加节点（见下表）
4. 连接：父节点 output → 子节点 input（控制节点按 Y 坐标排序子节点）
5. 工具栏 **Export → Export to JSON** 导出

### 运行时执行

1. 将导出的 JSON 放入 `Assets`（如 `Resources`）
2. 场景中创建 GameObject，添加 `BehaviorTreeRunner`
3. 将 JSON `TextAsset` 赋给 **Graph Asset**
4. 选择 **Play Mode：** Once / Update / FixedUpdate

```csharp
using Air.BehaviorTree;

// 手动 Tick
var status = runner.Tick();
```

## 节点说明

| 节点 | 菜单路径 | 说明 |
|------|----------|------|
| Root | Behavior Tree/Root | 入口，必须有且仅有一个 |
| Sequence | Behavior Tree/Sequence | 顺序执行子节点，任一失败则失败 |
| Selector | Behavior Tree/Selector | 顺序执行子节点，任一成功则成功 |
| Parallel | Behavior Tree/Parallel | 并行执行子节点 |
| Invert | Behavior Tree/Invert | 反转单个子节点结果 |
| Log | Behavior Tree/Log | 输出日志，返回 Success |
| Wait | Behavior Tree/Wait | 等待若干 tick 后返回 Success |

## 程序集

| 程序集 | 说明 |
|--------|------|
| `com.air.BehaviorTree.Runtime` | 运行时节点与 `BehaviorTreeRunner` |
| `com.air.BehaviorTree.Editor` | 编辑器节点与 Graph 视图 |

## 依赖

| 包 | 说明 |
|----|------|
| [com.alelievr.node-graph-processor](../com.alelievr.NodeGraphProcessor/README.md) | 图编辑、导出与 `RuntimeGraphBuilder` |

## 版本

- **0.1.0** — 初始行为树节点集与 Runner
