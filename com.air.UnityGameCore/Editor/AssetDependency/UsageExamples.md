# 使用示例

## 代码示例

虽然工具主要通过UI使用，但也可以在代码中调用核心API。

### 示例1：在脚本中查询依赖

```csharp
using Air.UnityGameCore.Editor.AssetDependency;
using UnityEditor;
using UnityEngine;

public class DependencyExample
{
    [MenuItem("Examples/查询依赖示例")]
    public static void QueryDependencyExample()
    {
        // 获取某个资源的路径
        string assetPath = "Assets/Prefabs/Player.prefab";
        
        // 查询所有依赖（递归）
        var dependencies = AssetDependencyAnalyzer.GetDependencies(assetPath, true);
        
        Debug.Log($"[{assetPath}] 共有 {dependencies.Count} 个依赖:");
        foreach (string dep in dependencies)
        {
            Debug.Log($"  - {dep}");
        }
    }
}
```

### 示例2：查询反向依赖

```csharp
using Air.UnityGameCore.Editor.AssetDependency;
using UnityEditor;
using UnityEngine;

public class ReverseDependencyExample
{
    [MenuItem("Examples/查询反向依赖示例")]
    public static void QueryReverseDependencyExample()
    {
        string assetPath = "Assets/Materials/PlayerMaterial.mat";
        
        // 查询哪些资源依赖此材质
        var reverseDeps = AssetDependencyAnalyzer.GetReverseDependencies(assetPath);
        
        if (reverseDeps.Count == 0)
        {
            Debug.Log($"[{assetPath}] 没有资源依赖它，可以安全删除");
        }
        else
        {
            Debug.Log($"[{assetPath}] 被 {reverseDeps.Count} 个资源依赖:");
            foreach (string dep in reverseDeps)
            {
                Debug.Log($"  - {dep}");
            }
        }
    }
}
```

### 示例3：构建依赖树

```csharp
using Air.UnityGameCore.Editor.AssetDependency;
using UnityEditor;
using UnityEngine;

public class DependencyTreeExample
{
    [MenuItem("Examples/构建依赖树示例")]
    public static void BuildTreeExample()
    {
        string assetPath = "Assets/Scenes/MainScene.unity";
        
        // 构建依赖树（最大深度3层）
        DependencyNode tree = AssetDependencyAnalyzer.BuildDependencyTree(assetPath, 3);
        
        // 打印树结构
        PrintTree(tree, "");
    }
    
    private static void PrintTree(DependencyNode node, string indent)
    {
        Debug.Log($"{indent}└─ {node.AssetName}");
        
        if (node.IsCircular)
        {
            Debug.Log($"{indent}   (循环依赖)");
            return;
        }
        
        foreach (var child in node.Children)
        {
            PrintTree(child, indent + "  ");
        }
    }
}
```

### 示例4：获取资源详细信息

```csharp
using Air.UnityGameCore.Editor.AssetDependency;
using UnityEditor;
using UnityEngine;

public class AssetInfoExample
{
    [MenuItem("Examples/获取资源信息示例")]
    public static void GetAssetInfoExample()
    {
        string assetPath = "Assets/Textures/Logo.png";
        
        // 获取资源信息
        AssetInfo info = AssetDependencyAnalyzer.GetAssetInfo(assetPath);
        
        Debug.Log($"资源名称: {info.AssetName}");
        Debug.Log($"资源路径: {info.AssetPath}");
        Debug.Log($"资源类型: {info.AssetTypeName}");
        Debug.Log($"文件扩展名: {info.AssetType}");
        Debug.Log($"文件大小: {AssetDependencyAnalyzer.FormatFileSize(info.FileSize)}");
        Debug.Log($"最后修改: {info.LastModified}");
    }
}
```

### 示例5：批量检查未使用资源

```csharp
using System.Collections.Generic;
using System.Linq;
using Air.UnityGameCore.Editor.AssetDependency;
using UnityEditor;
using UnityEngine;

public class UnusedAssetsExample
{
    [MenuItem("Examples/查找未使用资源")]
    public static void FindUnusedAssets()
    {
        // 获取所有材质
        string[] materialGuids = AssetDatabase.FindAssets("t:Material", new[] { "Assets/Materials" });
        
        List<string> unusedMaterials = new List<string>();
        
        foreach (string guid in materialGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            
            // 检查反向依赖
            var reverseDeps = AssetDependencyAnalyzer.GetReverseDependencies(assetPath);
            
            if (reverseDeps.Count == 0)
            {
                unusedMaterials.Add(assetPath);
            }
        }
        
        Debug.Log($"找到 {unusedMaterials.Count} 个未使用的材质:");
        foreach (string path in unusedMaterials)
        {
            Debug.Log($"  - {path}");
        }
    }
}
```

### 示例6：检测循环依赖

```csharp
using Air.UnityGameCore.Editor.AssetDependency;
using UnityEditor;
using UnityEngine;

public class CircularDependencyExample
{
    [MenuItem("Examples/检测循环依赖")]
    public static void DetectCircularDependencies()
    {
        string assetPath = "Assets/Prefabs/ComplexObject.prefab";
        
        // 构建依赖树
        DependencyNode tree = AssetDependencyAnalyzer.BuildDependencyTree(assetPath, 10);
        
        // 查找循环依赖
        bool hasCircular = CheckCircular(tree);
        
        if (hasCircular)
        {
            Debug.LogWarning($"[{assetPath}] 存在循环依赖！");
        }
        else
        {
            Debug.Log($"[{assetPath}] 没有循环依赖");
        }
    }
    
    private static bool CheckCircular(DependencyNode node)
    {
        if (node.IsCircular)
            return true;
        
        foreach (var child in node.Children)
        {
            if (CheckCircular(child))
                return true;
        }
        
        return false;
    }
}
```

### 示例7：生成自定义报告

```csharp
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Air.UnityGameCore.Editor.AssetDependency;
using UnityEditor;
using UnityEngine;

public class CustomReportExample
{
    [MenuItem("Examples/生成自定义依赖报告")]
    public static void GenerateCustomReport()
    {
        string reportPath = "Assets/DependencyReport.txt";
        
        // 获取所有预制体
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });
        
        using (StreamWriter writer = new StreamWriter(reportPath))
        {
            writer.WriteLine("预制体依赖分析报告");
            writer.WriteLine($"生成时间: {System.DateTime.Now}");
            writer.WriteLine($"总预制体数: {prefabGuids.Length}");
            writer.WriteLine();
            writer.WriteLine("=".PadRight(80, '='));
            writer.WriteLine();
            
            foreach (string guid in prefabGuids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                AssetInfo info = AssetDependencyAnalyzer.GetAssetInfo(assetPath);
                
                var deps = AssetDependencyAnalyzer.GetDependencies(assetPath, false);
                var reverseDeps = AssetDependencyAnalyzer.GetReverseDependencies(assetPath);
                
                writer.WriteLine($"预制体: {info.AssetName}");
                writer.WriteLine($"路径: {assetPath}");
                writer.WriteLine($"大小: {AssetDependencyAnalyzer.FormatFileSize(info.FileSize)}");
                writer.WriteLine($"直接依赖数: {deps.Count}");
                writer.WriteLine($"被依赖数: {reverseDeps.Count}");
                
                // 统计依赖类型
                var depTypes = deps.GroupBy(d => System.IO.Path.GetExtension(d))
                                  .OrderByDescending(g => g.Count());
                
                if (depTypes.Any())
                {
                    writer.WriteLine("依赖类型分布:");
                    foreach (var group in depTypes)
                    {
                        writer.WriteLine($"  {group.Key}: {group.Count()}");
                    }
                }
                
                writer.WriteLine();
                writer.WriteLine("-".PadRight(80, '-'));
                writer.WriteLine();
            }
        }
        
        AssetDatabase.Refresh();
        Debug.Log($"报告已生成: {reportPath}");
        
        // 在Project窗口中高亮显示
        Object report = AssetDatabase.LoadAssetAtPath<Object>(reportPath);
        EditorGUIUtility.PingObject(report);
    }
}
```

## 实际工作流示例

### 工作流1：优化预制体大小

**目标**: 减小某个预制体及其依赖的总大小

**步骤**:
1. 在Project中选中预制体
2. 右键 > 资源依赖 > 查看依赖关系
3. 查看依赖列表，关注.png和.jpg文件
4. 点击"选择"按钮查看每个纹理
5. 压缩或替换过大的纹理
6. 重新分析确认大小减小

### 工作流2：清理资源前的检查

**目标**: 安全地删除不再使用的资源

**步骤**:
1. 打开"批量依赖分析"工具
2. 将要清理的文件夹拖入或点击"添加文件夹中所有资源"
3. 点击"开始批量分析"
4. 将排序方式改为"被依赖数"
5. 查看被依赖数为0的资源（黄色显示）
6. 确认后删除这些资源

### 工作流3：重构资源结构

**目标**: 了解资源之间的依赖关系，便于重新组织

**步骤**:
1. 打开"资源依赖树形视图"
2. 选择核心资源（如主场景或核心预制体）
3. 设置最大深度为5
4. 点击"重新分析"查看完整依赖树
5. 点击"导出文本"保存树结构
6. 根据依赖关系规划新的文件夹结构
7. 逐步移动资源并验证

### 工作流4：AB包依赖规划

**目标**: 规划AssetBundle的资源分配

**步骤**:
1. 列出所有要打包的根资源
2. 使用"批量依赖分析"分析这些资源
3. 导出CSV报告
4. 在Excel中分析共享依赖
5. 将共享依赖提取到公共AB包
6. 重新验证依赖关系

## 提示与技巧

### 技巧1：快速定位大文件依赖
在批量分析中，按文件大小排序，快速找到占用空间最大的依赖。

### 技巧2：组合使用多个窗口
同时打开主窗口和树形视图，主窗口用于快速查询，树形视图用于深入分析。

### 技巧3：定期生成报告
使用批量分析定期生成项目依赖报告，跟踪资源依赖变化。

### 技巧4：利用Console输出
对于简单查询，使用右键菜单的"打印到控制台"功能最快。

### 技巧5：搜索功能
在树形视图中使用搜索功能，快速定位特定资源的依赖位置。

---

这些示例展示了工具的灵活性和强大功能。你可以根据项目需求定制和扩展这些示例。


