# Air Behavior Tree

基于 [Node Graph Processor](https://github.com/alelievr/NodeGraphProcessor) 实现的行为树 Unity 包。

## 功能

- **可视化编辑**：在节点图编辑器中设计行为树
- **标准节点类型**：
  - **Root**：根节点，必须有且仅有一个
  - **Selector**：选择器，按顺序执行子节点直到一个成功
  - **Sequence**：序列，按顺序执行子节点直到一个失败
  - **Inverter**：反转器，反转子节点结果
  - **Action**：动作节点基类，可扩展实现具体行为
- **运行时执行**：通过 `BehaviorTreeRunner` 组件在场景中运行行为树

## 使用方法

### 创建行为树

1. 菜单 **Assets > Create > Air > Behavior Tree** 创建新资源
2. 双击资源或 **Window > Air > Behavior Tree** 打开编辑器
3. 右键添加节点，从 Root 开始连接子节点

### 运行行为树

1. 将 `BehaviorTreeGraph` 资源拖到 `BehaviorTreeRunner` 组件的 **Behavior Tree** 字段
2. 运行场景，行为树会每帧执行

### 自定义动作

继承 `ActionNode` 并实现 `OnExecute` 方法：

```csharp
[NodeMenuItem("Behavior Tree/Actions/My Action", typeof(BehaviorTreeGraph))]
public class MyActionNode : ActionNode
{
    protected override BTStatus OnExecute(IBehaviorTreeContext context)
    {
        var ctx = context as DefaultBehaviorTreeContext;
        // 使用 ctx.GameObject 访问挂载的 GameObject
        return BTStatus.Success;
    }
}
```

### 自定义上下文

实现 `IBehaviorTreeContext` 接口，在 `BehaviorTreeRunner` 中设置 `Context` 属性以传递游戏特定数据（如黑板、AI 代理等）。

## 依赖

- com.alelievr.node-graph-processor
