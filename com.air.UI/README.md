# Air UI (`com.air.ui`) v2

UI 面板、组件生命周期、State/Trigger、Editor 代码生成。

## 启动

```csharp
using Air.UI;
using Air.UnityGameCore.Runtime;

// 推荐：一行组合
var entry = GameEntry.CreateWithUI();
entry.Runtime.Events.On("game.ready", () => { });
entry.UI.Panels.ShowPanel(config, showParam);

// 或分步
var runtime = GameRuntime.CreateDefault();
var ui = UIFramework.Install(runtime);
```

## 组件事件

```csharp
public partial class MyPanel : UIPanel
{
    protected override void OnUIInit()
    {
        this.On("custom.event", OnCustom);
    }

    void OnCustom() { }

    protected override void OnUIDestory() => this.ClearEvents();
}
```

## 全局 UI 事件名

- `UIEvents.PanelShown`
- `UIEvents.PanelClosed`

## 依赖

```
com.air.ui → com.air.unity-game-core 2.0.0 → com.air.game-core
           → com.unity.textmeshpro (Editor)
```

## 生成代码命名空间

`Air.UI.Generated`
