# Behavior Tree

基于 NodeGraphProcessor 的行为树包，用于 AI 与游戏逻辑。

## 功能

- **控制节点**：Root、Sequence、Selector
- **叶节点**：Log、Delay
- 可视化编辑器
- JSON 导出与运行时加载

## 使用

### 创建行为树

1. 右键 → Create → Behavior Tree → Graph
2. 双击打开，或 Window → Behavior Tree → Open Graph
3. 右键添加节点：Root、Sequence、Selector、Log、Delay
4. 连接：父节点 output → 子节点 input
5. 工具栏 Export → Export to JSON 导出

### 运行时执行

1. 将导出的 JSON 放入 `Assets`（如 `Resources`）
2. 在场景中创建 GameObject，添加 `BehaviorTreeRunner` 组件
3. 将 JSON 资源拖到 `Graph Asset`
4. 选择 Play Mode：Once / Update / FixedUpdate

## 节点说明

| 节点 | 说明 |
|------|------|
| Root | 入口，必须有且仅有一个 |
| Sequence | 按顺序执行子节点，任一失败则失败 |
| Selector | 按顺序执行子节点，任一成功则成功 |
| Log | 输出日志，返回 Success |
| Delay | 延迟若干 tick 后返回 Success |

## 依赖

- com.alelievr.node-graph-processor
