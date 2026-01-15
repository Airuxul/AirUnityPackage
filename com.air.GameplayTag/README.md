# Gameplay Tag System

一个类似UE4的Gameplay Tag系统，用于Unity项目中管理和使用层级化的标签。

## 功能特点

- ✅ 层级化标签系统（使用 `.` 分隔符，如 `Character.State.Stunned`）
- ✅ 标签容器（GameplayTagContainer）用于存储多个标签
- ✅ 强大的标签查询系统（支持AND、OR、NOT等逻辑运算）
- ✅ **UIToolkit 可视化编辑器**（双击重命名、自动保存、树形编辑）
- ✅ Inspector中的标签选择器
- ✅ GameObject组件支持
- ✅ 扩展方法，方便在代码中使用
- ✅ 导入/导出JSON功能

## 快速开始

### 1. 创建标签数据库

1. 在Unity菜单中选择 `Window > Gameplay Tag Manager`
2. 点击 "Create Database" 按钮
3. 将数据库保存在 `Assets/Resources/` 文件夹中，命名为 `GameplayTagDatabase`

### 2. 添加标签

在Gameplay Tag Manager窗口中使用树形编辑：
1. 点击工具栏的 `Add Root Tag` 创建根标签（如 `Character`）
2. 在标签上点击 `+` 按钮添加子标签（如在 `Character` 下添加 `State`）
3. 继续在子标签下添加更多层级（如在 `State` 下添加 `Stunned`）

**提示**：使用右键菜单快速编辑，不需要重复输入父路径！

### 3. 使用标签

#### 在GameObject上使用

1. 为GameObject添加 `GameplayTagComponent` 组件
2. 在Inspector中点击 `+` 按钮添加标签

#### 在脚本中使用

```csharp
using Air.GameplayTag;

public class Example : MonoBehaviour
{
    [SerializeField] private GameplayTag damageTag;
    [SerializeField] private GameplayTagContainer requiredTags;
    
    void Start()
    {
        // 检查GameObject是否有特定标签
        if (gameObject.HasGameplayTag(damageTag))
        {
            Debug.Log("对象可以造成伤害");
        }
        
        // 添加标签到GameObject
        gameObject.AddGameplayTag("Character.State.Invincible".ToGameplayTag());
        
        // 检查标签容器
        var component = GetComponent<GameplayTagComponent>();
        if (component.HasAllTags(requiredTags))
        {
            Debug.Log("满足所有必需标签");
        }
    }
}
```

## 核心类说明

### GameplayTag

单个标签，支持层级结构。

```csharp
GameplayTag tag = new GameplayTag("Character.State.Stunned");

// 检查标签匹配
bool matches = tag.MatchesTag(new GameplayTag("Character.State"));

// 获取父标签
GameplayTag parent = tag.GetParentTag(); // "Character.State"

// 获取所有父标签
List<GameplayTag> parents = tag.GetParentTags();
```

### GameplayTagContainer

标签容器，用于存储多个标签。

```csharp
GameplayTagContainer container = new GameplayTagContainer();

// 添加标签
container.AddTag(new GameplayTag("Character.State.Stunned"));

// 检查是否包含标签
bool hasTag = container.HasTag(new GameplayTag("Character"));

// 检查是否包含所有标签
bool hasAll = container.HasAllTags(otherContainer);

// 检查是否包含任意标签
bool hasAny = container.HasAnyTags(otherContainer);
```

### GameplayTagQuery

复杂的标签查询。

```csharp
// 创建查询：必须有 "Character" 标签，但不能有 "Dead" 标签
var query = GameplayTagQuery.MakeQuery_And(
    GameplayTagQuery.MakeQuery_MatchAllTags(
        new GameplayTagContainer(new GameplayTag("Character"))
    ),
    GameplayTagQuery.MakeQuery_MatchNoTags(
        new GameplayTagContainer(new GameplayTag("Character.State.Dead"))
    )
);

// 执行查询
bool matches = query.Matches(tagContainer);
```

### GameplayTagComponent

GameObject组件，用于为对象添加标签。

```csharp
var component = gameObject.GetComponent<GameplayTagComponent>();

// 添加标签
component.AddTag(new GameplayTag("Enemy.Type.Boss"));

// 检查标签
if (component.HasTag(new GameplayTag("Enemy")))
{
    Debug.Log("这是敌人");
}

// 使用查询
if (component.MatchesQuery(query))
{
    Debug.Log("匹配查询条件");
}
```

## 扩展方法

```csharp
using Air.GameplayTag;

// 字符串转GameplayTag
GameplayTag tag = "Character.State.Stunned".ToGameplayTag();

// GameObject扩展方法
gameObject.AddGameplayTag(tag);
gameObject.RemoveGameplayTag(tag);
bool hasTag = gameObject.HasGameplayTag(tag);
GameplayTagContainer tags = gameObject.GetGameplayTags();
```

## 标签命名规范

建议使用以下层级结构：

- **顶层类别**：主要功能类别（如 `Character`、`Ability`、`Status`、`Weapon`）
- **子类别**：具体分类（如 `Character.State`、`Ability.Type`）
- **具体标签**：详细描述（如 `Character.State.Stunned`、`Ability.Type.Fire`）

### 示例标签层级

```
Character
├── State
│   ├── Stunned
│   ├── Frozen
│   └── Invincible
├── Type
│   ├── Player
│   ├── Enemy
│   └── NPC
└── Class
    ├── Warrior
    ├── Mage
    └── Rogue

Ability
├── Type
│   ├── Fire
│   ├── Ice
│   └── Lightning
└── Target
    ├── Self
    ├── Single
    └── AOE

Status
├── Buff
│   ├── Speed
│   └── Damage
└── Debuff
    ├── Slow
    └── Poison
```

## 编辑器工具

### Gameplay Tag Manager Window

- 打开方式：`Window > Gameplay Tag Manager`
- 核心特性：
  - **双击重命名**：双击标签名称直接编辑
  - **自动保存**：点击其它地方自动保存，无需确认
  - **树形结构编辑**：直观的层级视图，无需重复输入父路径
  - **精简按钮**：只有 [+] 添加和 [×] 删除两个按钮
  - **右键菜单**：快速访问所有功能
  - **搜索过滤**：快速查找标签
  - **复制路径**：一键复制完整标签路径
  - **折叠/展开**：管理复杂的标签树
  - **现代化界面**：基于 UIToolkit，性能优秀

### Property Drawers

- GameplayTag字段会显示一个下拉按钮，可以从数据库中选择标签
- GameplayTagContainer字段会显示折叠列表，支持添加/删除标签

### Database Editor

- 选择GameplayTagDatabase资源时，Inspector会显示：
  - 标签统计信息
  - 打开管理器窗口按钮
  - 导出/导入JSON功能

## 导入导出

### 导出标签到JSON

1. 选择GameplayTagDatabase资源
2. 在Inspector中点击 "Export to JSON"
3. 选择保存位置

### 从JSON导入标签

1. 选择GameplayTagDatabase资源
2. 在Inspector中点击 "Import from JSON"
3. 选择JSON文件

JSON格式：
```json
{
    "tags": [
        "Character.State.Stunned",
        "Character.State.Frozen",
        "Ability.Type.Fire"
    ]
}
```

## 最佳实践

1. **标签数据库位置**：将GameplayTagDatabase放在 `Assets/Resources/` 文件夹中，这样可以通过 `GameplayTagDatabase.Instance` 访问
2. **命名一致性**：使用统一的命名规范，如 `Category.Subcategory.Detail`
3. **避免过深层级**：建议不超过4层
4. **使用查询**：对于复杂的标签条件判断，使用GameplayTagQuery而不是多个if语句
5. **性能优化**：标签比较是字符串比较，在性能敏感的代码中要注意缓存结果

## 与UE4对比

| 功能 | UE4 | 本系统 |
|------|-----|--------|
| 层级标签 | ✅ | ✅ |
| 标签容器 | ✅ | ✅ |
| 标签查询 | ✅ | ✅ |
| 可视化编辑器 | ✅ | ✅ |
| 组件支持 | ✅ | ✅ |
| 网络同步 | ✅ | ❌ (需自行实现) |
| 蓝图支持 | ✅ | N/A (Unity使用C#) |

## 许可证

本包遵循项目许可证。

## 版本历史

### 1.1.0
- 完整的Gameplay Tag系统实现
- 标签管理器窗口
- Property Drawers
- 查询系统
- 组件支持
- 导入/导出功能

