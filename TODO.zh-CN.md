# 跨包优化待办（用户版）

**最后更新：** 2026-06-03 · **范围：** 各 UPM 包现有功能的后续优化（中文）

> Agent 统筹文档（英文）：[`docs/TODO_ROADMAP.md`](docs/TODO_ROADMAP.md)  
> 各包明细：子模块根目录 `TODO.zh-CN.md`；技术 ID 与英文条目见各包 `docs/TODO.md`。

## 分层边界（不可越界）

| 层级 | 包 | 负责 | 禁止 |
|------|-----|------|------|
| L0 | `com.air.game-core` | 对象池、FSM/流程、命令撤销、实体逻辑、JSON 契约 | Unity、HTTP、UI、`GameRuntime` |
| L1 | `com.air.unity-game-core` | `GameRuntime`、资源、Unity 实体、定时器、场景/存档/音频 | UI 面板、CLI 命令实现 |
| L2 UI | `com.air.unity-ui` | `UIFramework`、面板、导航栈 | 重复实现事件总线/资源加载 |
| L2 CLI | `com.air.unity-connector` | HTTP 调用、命令目录、任务 | 游戏运行时组装、产品 UI |
| 领域 | gameplay-tag、behavior-tree、timeline-kit、node-graph | 各自领域 API | 无故依赖 L1 |
| 工具 | `unity-cmd` | 配置、目录缓存、CLI 调度 | Unity 命令处理器本体 |

跨包需求须拆成**提供方**与**消费方**两条（例：先 NGP-01 图邻接缓存，再 BT-01 校验）。

## 各包待办索引

| 包 | 用户文档 | 约条数 |
|----|----------|--------|
| game-core | [packages/com.air.game-core/TODO.zh-CN.md](packages/com.air.game-core/TODO.zh-CN.md) | 11 |
| unity-game-core | [packages/com.air.unity-game-core/TODO.zh-CN.md](packages/com.air.unity-game-core/TODO.zh-CN.md) | 15 |
| unity-ui | [packages/com.air.unity-ui/TODO.zh-CN.md](packages/com.air.unity-ui/TODO.zh-CN.md) | 10 |
| gameplay-tag | [packages/com.air.gameplay-tag/TODO.zh-CN.md](packages/com.air.gameplay-tag/TODO.zh-CN.md) | 10 |
| behavior-tree | [packages/com.air.unity-behavior-tree/TODO.zh-CN.md](packages/com.air.unity-behavior-tree/TODO.zh-CN.md) | 9 |
| node-graph-processor | [packages/com.alelievr.node-graph-processor/TODO.zh-CN.md](packages/com.alelievr.node-graph-processor/TODO.zh-CN.md) | 9 |
| timeline-kit | [packages/com.air.unity-timeline-kit/TODO.zh-CN.md](packages/com.air.unity-timeline-kit/TODO.zh-CN.md) | 9 |
| unity-cli | [packages/unity-cli/TODO.zh-CN.md](packages/unity-cli/TODO.zh-CN.md) | 12 |

## 建议优先顺序（P0）

| 顺序 | ID | 包 | 事项 | 原因 |
|------|-----|-----|------|------|
| 1 | GC-01 | game-core | L0 单元测试 | 为 L1 改动提供安全网 |
| 2 | UGC-01～03 | unity-game-core | 资源与实体生命周期 | 泄漏影响 UI 与玩法 |
| 3 | UI-01～02 | unity-ui | 卸载清理、Hide 误销毁 | 用户可见稳定性 |
| 4 | NGP-01～02 | node-graph-processor | 运行时图性能 | 行为树与导出依赖 |
| 5 | BT-01～02 | behavior-tree | 校验与冒烟测试 | 领域正确性 |
| 6 | TK-01～02 | timeline-kit | 预加载键与清单校验 | 示例可用性 |
| 7 | GT-01～02 | gameplay-tag | 重命名状态与传播 | 数据一致性 |
| 8 | CONN-01 | unity-connector | Profiler 参数修复 | 自动化可靠性 |
| 9 | CMD-01 | unity-cmd | 子模块/CI 完整性 | Agent 工作流 |

## 分阶段推进

- **阶段 A（基础）：** GC-01、UGC-01～03、NGP-01～02  
- **阶段 B（体验）：** UI-01～04、UGC-07～09、GT-01～02  
- **阶段 C（领域）：** BT-*、TK-*、GT-*、CONN/CMD  
- **阶段 D（打磨）：** 各包 P2/P3 项  

## 评审时拒绝的越界方案

| 错误做法 | 正确归属 |
|----------|----------|
| 在 game-core 放 HTTP | 仅 `com.air.unity-connector` |
| 在 unity-game-core 写 UI 面板 | `com.air.unity-ui` |
| 在 NGP 放 Sequence/Selector 节点 | `com.air.unity-behavior-tree` |
| 在 README 维护命令表 | `unity-cmd list` + 缓存目录 |
