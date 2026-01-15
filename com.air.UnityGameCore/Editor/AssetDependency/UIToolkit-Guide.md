# UIToolkit 实现指南

## 架构说明

本工具完全采用 **UIToolkit** 的最佳实践，将 UI 结构、样式和逻辑分离：

- **UXML 文件** (`AssetDependencyWindow.uxml`) - 定义 UI 结构
- **USS 文件** (`AssetDependencyWindow.uss`) - 定义样式
- **C# 文件** (`AssetDependencyWindow.cs`) - 处理逻辑

## 文件结构

```
AssetDependency/
├── AssetDependencyWindow.uxml    # UI 结构定义
├── AssetDependencyWindow.uss     # 样式表
└── AssetDependencyWindow.cs      # 逻辑控制
```

## UXML 文件结构

### 主要组成部分

```xml
<ui:VisualElement name="root-container">
    <!-- 1. 标题区域 -->
    <ui:VisualElement name="header-section">
        <ui:Label name="title-label" />
        <ui:Label name="description-label" />
    </ui:VisualElement>

    <!-- 2. 控制区域 -->
    <ui:VisualElement name="control-section">
        <uie:ObjectField name="asset-field" />
        <ui:Label name="asset-info-label" />
        <ui:VisualElement name="button-container">
            <ui:Button name="analyze-dependencies-btn" />
            <ui:Button name="analyze-reverse-dependencies-btn" />
            <ui:Button name="clear-btn" />
        </ui:VisualElement>
        <ui:Toggle name="recursive-toggle" />
    </ui:VisualElement>

    <!-- 3. 结果区域 -->
    <ui:VisualElement name="result-section">
        <ui:VisualElement name="result-header">
            <ui:Label name="result-title-label" />
            <ui:Label name="result-count-label" />
        </ui:VisualElement>
        <ui:ScrollView name="result-scroll-view" />
    </ui:VisualElement>
</ui:VisualElement>
```

### 命名规范

- **name**: 用短横线分隔的小写字母 (kebab-case)
  - 示例: `asset-field`, `analyze-dependencies-btn`
- **class**: 用短横线分隔的小写字母 (kebab-case)
  - 示例: `title-label`, `result-item`

## USS 样式表

### 样式组织

USS 文件按功能模块组织：

```css
/* ============================================ */
/* 标题区域 */
/* ============================================ */
.title-label {
    font-size: 18px;
    -unity-font-style: bold;
}

/* ============================================ */
/* 控制区域 */
/* ============================================ */
.control-section {
    padding: 10px;
    background-color: rgba(50, 50, 50, 0.3);
}

/* ============================================ */
/* 结果区域 */
/* ============================================ */
.result-scroll-view {
    flex-grow: 1;
    background-color: rgba(50, 50, 50, 0.2);
}
```

### 常用样式属性

| CSS 属性 | Unity USS 属性 | 说明 |
|---------|---------------|------|
| font-weight | -unity-font-style | bold, normal |
| text-align | -unity-text-align | middle-center, middle-left |
| display: flex | flex-direction | row, column |
| flex: 1 | flex-grow | 1 |

## C# 代码集成

### 1. 加载 UXML 和 USS

```csharp
public void CreateGUI()
{
    // 加载 UXML
    string uxmlPath = GetAssetPath("AssetDependencyWindow.uxml");
    VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
    visualTree.CloneTree(rootVisualElement);
    
    // 加载 USS
    string ussPath = GetAssetPath("AssetDependencyWindow.uss");
    StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ussPath);
    rootVisualElement.styleSheets.Add(styleSheet);
    
    // 绑定UI元素和事件
    BindUIElements();
    RegisterEvents();
}
```

### 2. 查询 UI 元素

使用 `Q<T>()` 方法通过 name 属性查询元素：

```csharp
private void BindUIElements()
{
    _assetField = rootVisualElement.Q<ObjectField>("asset-field");
    _analyzeDependenciesBtn = rootVisualElement.Q<Button>("analyze-dependencies-btn");
    _resultScrollView = rootVisualElement.Q<ScrollView>("result-scroll-view");
}
```

### 3. 使用 CSS 类

通过 `AddToClassList()` 添加样式类：

```csharp
private VisualElement CreateResultItem(string assetPath)
{
    VisualElement itemContainer = new VisualElement();
    itemContainer.AddToClassList("result-item");
    
    Label nameLabel = new Label(Path.GetFileName(assetPath));
    nameLabel.AddToClassList("result-item-name");
    
    return itemContainer;
}
```

## 自定义和扩展

### 修改 UI 结构

1. 打开 `AssetDependencyWindow.uxml`
2. 添加新的 UI 元素
3. 在 C# 中绑定元素

**示例：添加一个过滤框**

```xml
<!-- 在 UXML 中添加 -->
<ui:TextField name="filter-field" label="过滤" />
```

```csharp
// 在 C# 中绑定
private TextField _filterField;

private void BindUIElements()
{
    // ... 其他绑定
    _filterField = rootVisualElement.Q<TextField>("filter-field");
}
```

### 修改样式

直接编辑 `AssetDependencyWindow.uss` 文件：

```css
/* 修改按钮颜色 */
.primary-button {
    background-color: rgba(50, 120, 180, 0.8);
}

.primary-button:hover {
    background-color: rgba(60, 140, 200, 0.9);
}
```

### 添加新的 CSS 类

1. 在 USS 文件中定义新类：

```css
.warning-label {
    color: rgb(255, 200, 0);
    -unity-font-style: bold;
}
```

2. 在代码中使用：

```csharp
Label warningLabel = new Label("警告信息");
warningLabel.AddToClassList("warning-label");
```

## 响应式设计

### Flexbox 布局

USS 支持 Flexbox 布局：

```css
.button-container {
    flex-direction: row;  /* 水平排列 */
    justify-content: space-between;  /* 两端对齐 */
}

.action-button {
    flex-grow: 1;  /* 自动扩展 */
    margin-right: 5px;
}
```

### 动态样式

使用伪类实现交互效果：

```css
.result-item:hover {
    background-color: rgba(76, 76, 76, 0.5);
}

.action-button:active {
    background-color: rgba(80, 80, 80, 0.8);
}
```

## 主题和变量

虽然 USS 不直接支持 CSS 变量，但可以通过一致的颜色值创建主题：

```css
/* 深色主题配色 */
/* 背景色: rgba(50, 50, 50, 0.3) */
/* 主色: rgba(50, 120, 180, 0.8) */
/* 文本色: rgb(220, 220, 220) */
/* 次要文本: rgb(180, 180, 180) */
```

## 调试技巧

### 1. 使用 UI Toolkit Debugger

在 Unity 编辑器中：
1. 打开工具窗口
2. 菜单: `Window > UI Toolkit > Debugger`
3. 选择你的 EditorWindow
4. 实时查看和修改样式

### 2. 动态修改样式

在运行时测试样式：

```csharp
// 临时修改样式用于测试
element.style.backgroundColor = new Color(1, 0, 0);
element.style.fontSize = 20;
```

### 3. 检查元素层级

```csharp
// 打印元素树结构
void PrintHierarchy(VisualElement element, int indent = 0)
{
    Debug.Log($"{new string(' ', indent)}{element.name} ({element.GetType().Name})");
    foreach (var child in element.Children())
    {
        PrintHierarchy(child, indent + 2);
    }
}
```

## 性能优化

### 1. 避免频繁修改样式

❌ **不推荐**：
```csharp
element.style.color = Color.red;
element.style.fontSize = 12;
element.style.marginTop = 5;
```

✅ **推荐**：
```csharp
element.AddToClassList("my-styled-element");
```

### 2. 复用 VisualElement

对于大量相似元素，考虑对象池模式。

### 3. 延迟加载

仅在需要时创建复杂的 UI 元素。

## 最佳实践

### ✅ 推荐做法

1. **分离关注点**: UI 结构在 UXML，样式在 USS，逻辑在 C#
2. **使用语义化命名**: `asset-field` 比 `field1` 更清晰
3. **CSS 类而非内联样式**: 使用 `AddToClassList()` 而非 `element.style.xxx`
4. **模块化设计**: 将相关样式组织在一起
5. **注释说明**: 在 USS 中使用注释分隔不同区域

### ❌ 避免做法

1. **不要在代码中硬编码样式**: 应该在 USS 中定义
2. **不要过度嵌套**: 保持 UXML 结构扁平化
3. **不要使用魔法数字**: 在 USS 中定义统一的间距和尺寸
4. **不要忽略可访问性**: 确保文本大小和对比度合理

## 参考资源

### Unity 官方文档
- [UI Toolkit](https://docs.unity3d.com/Manual/UIElements.html)
- [UXML 格式参考](https://docs.unity3d.com/Manual/UIE-UXML.html)
- [USS 格式参考](https://docs.unity3d.com/Manual/UIE-USS.html)
- [UI Toolkit 调试器](https://docs.unity3d.com/Manual/UIE-Debugger.html)

### USS 支持的 CSS 属性
- [USS 属性参考](https://docs.unity3d.com/Manual/UIE-USS-Properties-Reference.html)
- [USS 选择器](https://docs.unity3d.com/Manual/UIE-USS-Selectors.html)

## 总结

通过 UXML 和 USS 实现 UIToolkit 的优势：

1. **可维护性**: 结构、样式、逻辑分离
2. **可复用性**: 样式类可以跨多个元素复用
3. **可读性**: UI 结构一目了然
4. **灵活性**: 无需重新编译即可调整样式
5. **性能**: CSS 类比内联样式更高效

这种方式符合现代 Web 开发的最佳实践，也是 Unity 官方推荐的 UIToolkit 使用方式。


