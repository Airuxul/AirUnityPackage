# Unity Game Core (`com.air.unity-game-core`) v2

Unity 运行时基础设施：**无 UI、无全局单例**。

## API

| 类型 | 说明 |
|------|------|
| `GameRuntime` | `Events` + `Resources`，`GameRuntime.CreateDefault()` |
| `IGameRuntime` | 运行时契约 |
| `EventBus` | `On` / `Emit` / `Off` |
| `IResManager` | 资源加载（`UnityResManager`、`AssetBundleResManager`） |
| `PoolManager` / `UnityObjectPool<T>` | Unity 对象池 |

## 示例

```csharp
using Air.UnityGameCore.Runtime;

var runtime = GameRuntime.CreateDefault();
runtime.Events.On("game.start", () => { });
runtime.Events.Emit("game.start");
```

## 安装

```json
"com.air.unity-game-core": "file:../CustomPackages/com.air.UnityGameCore"
```

依赖：`com.air.game-core` 1.0.1+

## 与 UI 协作

见 [Air UI](../com.air.UI/README.md) 的 `GameEntry.CreateWithUI()`。
