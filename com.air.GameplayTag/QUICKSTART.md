# Gameplay Tag 快速入门指南

## 5分钟上手教程

### 步骤1：创建标签数据库

1. 打开Unity编辑器
2. 菜单栏选择 **Window > Gameplay Tag Manager**
3. 点击 **"Create Database"** 按钮
4. 在弹出的保存对话框中：
   - 导航到 `Assets/Resources/` 文件夹（如果不存在，先创建一个）
   - 文件名保持为 `GameplayTagDatabase`
   - 点击保存

✅ 数据库创建完成！

### 步骤2：添加标签（树形编辑）

在Gameplay Tag Manager窗口中，使用树形结构直接编辑：

**方式1：创建根标签**
1. 点击工具栏的 **"Add Root Tag"** 按钮
2. 输入根标签名称，例如 `Character`
3. 点击 **"Add"**

**方式2：添加子标签**
1. 在已有的标签上点击 **"+"** 按钮（或右键选择"Add Child Tag"）
2. 输入子标签名称，例如 `Type`（不需要输入父路径）
3. 点击 **"Add"**

**示例：创建完整的标签树**
```
1. 创建根标签: Character
2. 在 Character 上点 + 添加: Type
3. 在 Type 上点 + 添加: Player
4. 在 Type 上点 + 添加: Enemy
5. 在 Character 上点 + 添加: State
6. 在 State 上点 + 添加: Healthy
7. 在 State 上点 + 添加: Dead
```

**快捷操作**：
- 🖱️ **右键菜单**：在任何标签上右键，可以添加子标签、重命名、删除
- ⌨️ **键盘快捷键**：重命名时按 Enter 确认，Esc 取消
- 📋 **复制路径**：点击 "C" 按钮快速复制完整路径
- ✏️ **重命名**：点击 "R" 按钮或右键选择"Rename"

✅ 标签添加完成！不需要重复输入父路径了！

### 💡 快捷操作

**双击重命名：**
```
双击标签名称 → 直接编辑 → 点击其它地方自动保存
```
不需要点击重命名按钮！

**精简按钮：**
- `[+]` 添加子标签
- `[×]` 删除标签
- 其它功能通过右键菜单访问

**键盘快捷键：**
- `Enter` - 确认重命名
- `Esc` - 取消重命名

### 步骤3：在GameObject上使用标签

1. 在场景中选择一个GameObject
2. 在Inspector中点击 **"Add Component"**
3. 搜索并添加 **"Gameplay Tag Component"**
4. 在组件中点击 **"+"** 按钮
5. 从下拉菜单中选择标签（如 `Character.Type.Player`）

✅ GameObject现在有标签了！

### 步骤4：在脚本中使用标签

创建一个新的C#脚本：

```csharp
using UnityEngine;
using Air.GameplayTag;

public class PlayerController : MonoBehaviour
{
    // 在Inspector中显示标签选择器
    [SerializeField] private GameplayTag playerTag;
    [SerializeField] private GameplayTagContainer immunityTags;

    void Start()
    {
        // 方式1：使用扩展方法
        if (gameObject.HasGameplayTag(playerTag))
        {
            Debug.Log("这是玩家对象");
        }

        // 方式2：使用组件
        var tagComponent = GetComponent<GameplayTagComponent>();
        if (tagComponent != null)
        {
            // 添加标签
            tagComponent.AddTag("Status.Buff.Speed".ToGameplayTag());
            
            // 检查标签
            if (tagComponent.HasTag("Character.Type.Player".ToGameplayTag()))
            {
                Debug.Log("玩家角色");
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // 检查碰撞对象是否是敌人
        if (collision.gameObject.HasGameplayTag("Character.Type.Enemy".ToGameplayTag()))
        {
            Debug.Log("碰到敌人了！");
        }
    }
}
```

✅ 脚本编写完成！

## 常见使用场景

### 场景1：角色状态管理

```csharp
public class CharacterHealth : MonoBehaviour
{
    private GameplayTagComponent tagComponent;

    void Start()
    {
        tagComponent = GetComponent<GameplayTagComponent>();
    }

    public void TakeDamage(float damage)
    {
        // 检查是否无敌
        if (tagComponent.HasTag("Character.State.Invincible".ToGameplayTag()))
        {
            Debug.Log("无敌状态，无法受伤");
            return;
        }

        // 应用伤害...
        
        // 如果死亡
        if (health <= 0)
        {
            tagComponent.RemoveTag("Character.State.Healthy".ToGameplayTag());
            tagComponent.AddTag("Character.State.Dead".ToGameplayTag());
        }
    }
}
```

### 场景2：技能系统

```csharp
public class Ability : MonoBehaviour
{
    [SerializeField] private GameplayTagContainer requiredTags; // 需要的标签
    [SerializeField] private GameplayTagContainer blockingTags; // 阻止的标签

    public bool CanCast(GameObject target)
    {
        var targetTags = target.GetComponent<GameplayTagComponent>();
        if (targetTags == null) return false;

        // 检查目标是否有所有必需的标签
        if (!targetTags.HasAllTags(requiredTags))
        {
            Debug.Log("目标缺少必需的标签");
            return false;
        }

        // 检查目标是否有任何阻止标签
        if (targetTags.HasAnyTags(blockingTags))
        {
            Debug.Log("目标有阻止技能的标签");
            return false;
        }

        return true;
    }
}
```

### 场景3：敌人AI

```csharp
public class EnemyAI : MonoBehaviour
{
    void Update()
    {
        // 查找所有玩家
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        
        foreach (var obj in allObjects)
        {
            // 只追逐玩家角色
            if (obj.HasGameplayTag("Character.Type.Player".ToGameplayTag()))
            {
                // 但不追逐隐身的玩家
                if (!obj.HasGameplayTag("Status.Buff.Invisible".ToGameplayTag()))
                {
                    ChaseTarget(obj);
                    break;
                }
            }
        }
    }

    void ChaseTarget(GameObject target)
    {
        // 追逐逻辑...
    }
}
```

## Inspector中的标签字段

### 单个标签字段

```csharp
[SerializeField] private GameplayTag myTag;
```

在Inspector中会显示：
- 文本框：显示当前标签
- 下拉按钮 (▼)：点击选择标签

### 标签容器字段

```csharp
[SerializeField] private GameplayTagContainer myTags;
```

在Inspector中会显示：
- 折叠箭头：展开/收起标签列表
- **+** 按钮：添加新标签
- **×** 按钮：清空所有标签
- 每个标签旁边的 **-** 按钮：删除单个标签

## 下一步

- 阅读完整的 [README.md](README.md) 了解所有功能
- 查看 [Examples.cs](Runtime/Examples.cs) 了解更多使用示例
- 打开 **Gameplay Tag Manager** 窗口管理标签
- 尝试创建复杂的标签查询（GameplayTagQuery）

## 常见问题

### Q: 数据库找不到？
A: 确保数据库保存在 `Assets/Resources/GameplayTagDatabase.asset`，这样才能通过 `GameplayTagDatabase.Instance` 访问。

### Q: Inspector中看不到标签选择器？
A: 确认脚本使用了正确的命名空间 `using Air.GameplayTag;`

### Q: 如何删除标签？
A: 在Gameplay Tag Manager窗口中，点击标签右侧的 **×** 按钮。

### Q: 标签可以在运行时动态添加吗？
A: 可以！使用 `GameplayTagDatabase.Instance.AddTag("New.Tag")` 添加（但建议在编辑器中定义）。

### Q: 性能如何？
A: 标签比较是字符串比较，对于大多数游戏来说性能足够。如果在Update中频繁检查，建议缓存结果。

## 获取帮助

如果遇到问题：
1. 检查Unity控制台是否有错误信息
2. 确认所有必需的文件都存在
3. 尝试重新导入包
4. 查看完整文档 README.md

祝你使用愉快！🎮

