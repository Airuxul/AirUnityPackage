# Node Graph Processor (`com.alelievr.node-graph-processor`)

基于 [Node Graph Processor](https://github.com/alelievr/NodeGraphProcessor) 的 **Fork**，在原有 UIElements 节点图编辑器基础上，增加 **编辑器图导出 → Runtime 执行** 能力。

**上游作者：** Antoine Lievrard (alelievr)  
**本仓库维护：** airuxul

## 相对上游的扩展

| 能力 | 说明 |
|------|------|
| `GraphExportData` | 图、节点、边的可序列化导出结构 |
| `RuntimeGraphBuilder` | 从 JSON / 二进制构建 `RuntimeGraph` |
| `RuntimeGraph` / `RuntimeBaseNode` | 运行时无 Editor 依赖的图执行 |
| `BaseGraphRunner` | MonoBehaviour 运行图的基础类 |
| 节点注册 | `RuntimeGraphBuilder.RegisterNodeCreator` 按类型映射 Editor 节点到 Runtime 节点 |

## 安装

```json
"com.alelievr.node-graph-processor": "file:../CustomPackages/com.alelievr.NodeGraphProcessor"
```

被 [Behavior Tree](../com.air.BehaviorTree/README.md) 依赖，使用行为树时需一并安装。

## 工作流

1. **Editor：** 创建 `BaseGraph` 子类资产，在 Graph 窗口编辑节点与连线。
2. **导出：** 工具栏导出为 JSON（或二进制），供打包进 `TextAsset` / `Resources`。
3. **Runtime：** `RuntimeGraphBuilder.FromJson(text)` 构建图，通过 `ProcessGraphProcessor` 或自定义 Runner 执行。

```csharp
using GraphProcessor;

var graph = RuntimeGraphBuilder.FromJson(jsonText);
// 注册自定义节点类型后 Build
graph.Dispose();
```

## 程序集

| 程序集 | 平台 |
|--------|------|
| `com.alelievr.NodeGraphProcessor.Runtime` | 全平台 |
| `com.alelievr.NodeGraphProcessor.Editor` | Editor |

## 依赖

无其他 CustomPackages 依赖。

## 相关包

- [Behavior Tree](../com.air.BehaviorTree/README.md) — 基于本包实现的行为树节点与 `BehaviorTreeRunner`。
