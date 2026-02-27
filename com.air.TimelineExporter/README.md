# Timeline Exporter

将 Unity Timeline 导出为 JSON，在运行时通过 `TimelinePlayer` 模拟播放，无需 `PlayableDirector`。

---

## 一、工作流程概览

```
[Unity Editor] TimelineAsset → 导出 JSON → [Runtime] TimelineData + TimelinePlayer → 模拟播放
```

1. **编辑期**：在 Unity 中创建 Timeline，选中后通过菜单导出为 JSON。
2. **运行期**：加载 JSON 得到 `TimelineData`，用 `TimelinePlayer` 驱动播放，并注册各 Clip 类型的处理逻辑。

---

## 二、导出 Timeline

### 2.1 导出步骤

1. 在 Project 中选中一个 `TimelineAsset`。
2. 菜单：**Assets → Timeline Exporter → Export Timeline**。
3. 选择保存路径，导出为 `.json` 文件。

### 2.2 导出内容

- **轨道**：轨道名、类型、是否静音、Clips。
- **Clip**：开始时间、时长、ClipType、DisplayName、CustomData（如 ControlTrack 的配置）。
- **引用**：资源路径、GUID、类型等。

### 2.3 支持的 Clip 类型

| Clip 类型 | 说明 | CustomData |
|-----------|------|-------------|
| `ControlPlayableAsset` | 控制 GameObject 激活、粒子、子 Timeline、ITimeControl | `ControlClipData` JSON |
| `CustomClip` | 自定义 Clip，可扩展 | 空或自定义 JSON |

---

## 三、运行时播放

### 3.1 使用 TimelinePlayerComponent（推荐）

在场景中挂载 `TimelinePlayerComponent`，配置后自动加载并播放：

1. **Timeline Json**：拖入导出的 `TextAsset`（JSON）。
2. **Binding Root**：绑定查找根节点，为本物体 `Transform`。
3. **Timeline 预览**：从导出数据的 `timelineAssetPath` / `timelineResourcePath` 加载 TimelineAsset。Play 时若选中该物体，Timeline 窗口会同步预览播放进度（只读，无法拖拽进度条）。
4. **Bindings**：手动绑定 `key → GameObject`，key 通常为 ControlClip 的 `sourceBindingKey` 或轨道名。

**自动绑定**：会递归遍历 `Binding Root` 子节点，按 `name` 填充 Bindings。

**内置注册**：

- `CustomClip` → `CustomRuntimeBehaviour`
- `ControlPlayableAsset` → `ControlTrackBehaviour`

### 3.2 手动使用 TimelinePlayer

```csharp
// 1. 加载数据
var data = TimelineDataLoader.LoadFromJson(jsonText);
// 或
var data = TimelineDataLoader.LoadFromResources("Exports/TestTimeline");
// 或
var data = TimelineDataLoader.LoadFromFile(path);

// 2. 创建 Player
var player = new TimelinePlayer(data);

// 3. 配置绑定
player.BindingRoot = transform;
player.Bindings["Square"] = squareGameObject;  // key = sourceBindingKey 或 trackName

// 4. 注册 Clip 处理逻辑
player.RegisterBehaviour("CustomClip", new CustomRuntimeBehaviour());
player.RegisterBehaviour("ControlPlayableAsset", new ControlTrackBehaviour());

// 5. 播放
player.Play();

// 6. 每帧更新
void Update()
{
    player?.Update(Time.deltaTime);
}
```

### 3.3 TimelinePlayer API

| 方法/属性 | 说明 |
|-----------|------|
| `Play()` / `Play(double startTime)` | 开始播放 |
| `Pause()` | 暂停 |
| `Stop()` | 停止并重置 |
| `SetTime(double time)` | 跳转到指定时间 |
| `Update(float deltaTime)` | 每帧调用，驱动播放 |
| `CurrentTime` / `Duration` / `IsPlaying` | 当前时间、总时长、是否播放中 |
| `OnTimeUpdated` / `OnPlaybackFinished` | 时间更新、播放结束事件 |
| `Bindings` | key → GameObject 绑定表 |
| `SubTimelinePlayers` | 子 Timeline（ControlTrack updateDirector 用） |
| `RegisterBehaviour(string clipType, ISimulatedPlayableBehaviour)` | 注册 Clip 类型处理逻辑 |

---

## 四、ControlTrack 与绑定

### 4.1 ControlClipData

ControlTrack 的 Clip 在 `TimelineClipData.CustomData` 中存储 `ControlClipData` JSON：

| 字段 | 说明 |
|------|------|
| `sourceBindingKey` | 绑定 key，对应 `Bindings[key]`；prefab 时为父节点 key |
| `trackName` | 轨道名，作为 fallback 绑定 key |
| `prefabRefId` | 资源表中 prefab 的 Id；运行时若绑定未找到则动态实例化 |
| `active` | 是否控制 GameObject 激活 |
| `updateDirector` | 是否控制子 Timeline 播放 |
| `updateParticle` | 是否控制粒子系统 |
| `updateITimeControl` | 是否调用 `ITimeControl` |
| `searchHierarchy` | 粒子是否在子节点中查找 |
| `postPlayback` | 0=Revert, 1=LeaveAsIs, 2=LeaveActive |

### 4.2 Prefab 动态实例化

当 ControlClip 引用 prefab（`prefabGameObject` 或 `sourceGameObject` 的默认值为 prefab）时：

- 导出时加入统一资源表 `TimelineData.Resources`，按 Guid 去重；`ControlClipData.prefabRefId` 指向该资源
- 运行时若 Bindings 和层级中均未找到目标，则从资源表取 `resourcePath`，`Resources.Load` + `Instantiate` 创建实例
- 父节点：`BindingRoot`（当前物体 Transform）
- `postPlayback == 0`（Revert）时，clip 结束时销毁实例

**要求**：prefab 需放在 `Assets/Resources/` 或其子目录下。

### 4.3 绑定解析顺序

1. `Bindings[sourceBindingKey]`
2. `Bindings[trackName]`
3. `Bindings[clip.DisplayName]`
4. `BindingRoot.Find(trackName)` 或 `Find(displayName)`
5. 递归在 `BindingRoot` 子节点中查找
6. 若 `prefabRefId` 非空，从 `TimelineData.Resources` 查 ResourcePath，再 Resources.Load 并实例化

### 4.4 子 Timeline

若 ControlClip 启用 `updateDirector`，需在 `SubTimelinePlayers` 中注册对应 `TimelinePlayer`，key 为 `sourceBindingKey` 或 `trackName`。

---

## 五、自定义 Clip 类型

### 5.1 实现 ISimulatedPlayableBehaviour

```csharp
public class MyClipBehaviour : ISimulatedPlayableBehaviour
{
    public void OnGraphStart(IPlayableContext context, IClipContext clipContext) { }
    public void OnGraphStop(IPlayableContext context, IClipContext clipContext) { }
    public void OnBehaviourPlay(IPlayableContext context, IClipContext clipContext)
    {
        // Clip 进入时
        var clip = clipContext.Clip;
        var go = clipContext.Bindings.TryGetValue("MyKey", out var g) ? g : null;
    }
    public void OnBehaviourPause(IPlayableContext context, IClipContext clipContext) { }
    public void PrepareFrame(IPlayableContext context, IClipContext clipContext) { }
    public void ProcessFrame(IPlayableContext context, IClipContext clipContext)
    {
        // 每帧调用
        var time = clipContext.Playable.GetTime();
        var delta = clipContext.FrameData.DeltaTime;
    }
}
```

### 5.2 使用 IClipContext

| 成员 | 说明 |
|------|------|
| `Playable` | `IPlayableHandle`：`GetTime()`, `GetDuration()` |
| `FrameData` | `IFrameData`：`DeltaTime`, `EffectiveWeight`, `EffectiveSpeed`, `Weight` |
| `Clip` | `TimelineClipData` |
| `Track` | `TimelineTrackData` |
| `Bindings` | 绑定表 |
| `SubTimelinePlayers` | 子 Timeline |
| `BindingRoot` | 绑定查找根 |

### 5.3 注册自定义 Behaviour

```csharp
player.RegisterBehaviour("MyClipType", new MyClipBehaviour());
```

`clipType` 需与导出 JSON 中 `TimelineClipData.ClipType` 一致（通常为 Clip Asset 的类名）。

### 5.4 与 Timeline 共用逻辑（BaseRuntimeBehaviour）

若希望同一套逻辑在 **Timeline（PlayableDirector）** 和 **TimelinePlayer 模拟** 中复用，可继承 `BaseRuntimeBehaviour`：

```csharp
public class MyBehaviour : BaseRuntimeBehaviour
{
    protected override void OnBehaviourPlay(IPlayableContext context, IClipContext clipContext)
    {
        // 两路都会调用
    }
    protected override void ProcessFrame(IPlayableContext context, IClipContext clipContext)
    {
        // 注意：Timeline 路径下 clipContext 为 null
    }
}
```

---

## 六、数据格式

### 6.1 TimelineData

```json
{
  "id": "TestTimeline",
  "name": "TestTimeline",
  "duration": 6.08,
  "frameRate": 60,
  "timelineAssetPath": "Assets/Resources/TestTimeline.playable",
  "timelineResourcePath": "TestTimeline",
  "tracks": [...],
  "resources": [
    {
      "id": "guid-or-refId",
      "assetPath": "Assets/Resources/Prefabs/Square.prefab",
      "assetType": "UnityEngine.GameObject",
      "guid": "...",
      "resourcePath": "Prefabs/Square",
      "resourceType": "Prefab"
    }
  ],
  "version": 1
}
```

**resources**：统一资源表，按 Guid 去重。多个 clip 引用同一资源时只存一份。

### 6.2 TimelineTrackData

- `id`, `name`, `trackType`, `muted`, `bindingType`, `clips`

### 6.3 TimelineClipData

- `id`, `startTime`, `duration`, `clipIn`, `timeScale`, `clipType`, `displayName`
- `referenceIds`, `customData`（如 ControlClipData JSON）

---

## 七、依赖与版本

- **Unity**：2020.3+
- **com.unity.timeline**：1.6.4

---

## 八、目录结构

```
com.air.TimelineExporter/
├── package.json
├── README.md
├── Runtime/
│   ├── Data/           TimelineData, TimelineResourceData, TimelineTrackData, TimelineClipData, ControlClipData
│   ├── Interfaces/     IPlayableBehaviour, IPlayableContext, ISimulatedPlayableBehaviour,
│   │                   IClipContext, IPlayableHandle, IFrameData
│   ├── Adapters/       PlayableContextAdapter（Playable+FrameData → IPlayableContext）
│   ├── Simulation/     SimulatedPlayableContext, ClipPlayableContext
│   ├── Behaviour/      BaseRuntimeBehaviour, CustomRuntimeBehaviour, ControlTrackBehaviour
│   ├── TimelinePlayer, TimelinePlayerComponent, TimelineDataLoader
│   └── ...
└── Editor/
    ├── TimelineExporter.cs    导出菜单与逻辑
    ├── Clip/CustomClip.cs
    ├── Track/CustomTrack.cs
    └── ...
```
