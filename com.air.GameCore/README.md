# Game Core (`com.air.game-core`)

纯 C# 游戏基础库，**不依赖 Unity 引擎**，可在编辑器外或单元测试中复用。

## 功能

| 模块 | 命名空间 | 说明 |
|------|----------|------|
| 单例 | `Air.GameCore` | 泛型 `Singleton<T>` |
| 对象池 | `Air.GameCore` | `ObjectPool<T>`、`ListPool<T>`、`IPoolable` |
| 状态机 | `Air.GameCore.StateMachine` | `StateMachine`、`State`、`StateTransition` |
| 分层状态机 | `Air.GameCore.StateMachine.Layered` | `LayeredStateMachine`、多层级并行状态 |

## 安装

```json
"com.air.game-core": "file:../CustomPackages/com.air.GameCore"
```

无 UPM 传递依赖。

## 状态机示例

```csharp
using Air.GameCore.StateMachine;

public class IdleState : State
{
    public override void Enter() { }
    public override void Update(float deltaTime) { }
    public override void Exit() { }
}

var machine = new StateMachine(new IdleState());
machine.Tick(Time.deltaTime);
machine.ChangeState(oldState, new IdleState());
```

## 对象池示例

```csharp
using Air.GameCore;

var pool = new ObjectPool<MyClass>(() => new MyClass(), obj => obj.Reset());
var item = pool.Get();
pool.Release(item);
```

## 依赖

无（仅 .NET / Unity 程序集定义，无 `package.json` dependencies）。

## 相关包

- [Unity Game Core](../com.air.UnityGameCore/README.md) — 在 Unity 侧使用本包的状态机与单例模式。
