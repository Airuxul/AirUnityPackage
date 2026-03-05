# Air Unity Package
自用Unity包

## 包结构
* com.air.GameCore 通用游戏代码，不依赖Unity，纯c#代码逻辑
* com.air.UnityGameCore 通用Unity代码，依赖GameCore
* com.air.GameplayTag 游戏Tag，仿UE GameplayTag
* com.air.UI 游戏UI框架
* com.alelievr.NodeGraphProcessor 魔改了下里面逻辑，支持Editor导出数据Runtime执行的基础节点图
* com.air.BehaviorTree 根据NodeGraphProcessor实现行为树逻辑
* com.air.TimelineKit 拓展timeline逻辑