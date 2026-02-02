using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Air.UnityGameCore.Runtime.UI;
using Transform = UnityEngine.Transform;

namespace Air.UnityGameCore.Editor.UI
{
    /// <summary>
    /// UI脚本生成器，负责根据GameObject结构自动生成UI脚本代码
    /// </summary>
    public static class UIScriptGenerator
    {
        /// <summary>
        /// 为指定的GameObject生成UI脚本（指定类名和输出路径）
        /// </summary>
        /// <param name="targetGo">目标GameObject</param>
        /// <param name="className">UI类名</param>
        /// <param name="outputFolder">输出文件夹路径</param>
        public static void GenerateUIScript(GameObject targetGo, string className, string outputFolder, UIType uiType)
        {
            if (!ValidateInputParameters(targetGo, className, outputFolder))
            {
                return;
            }

            try
            {
                // 检查并处理已有UIComponent的情况
                if (TryHandleExistingUIComponent(targetGo))
                {
                    return;
                }
                
                // 确保输出目录存在
                EnsureOutputDirectory(outputFolder);

                // 检查并处理脚本已存在的情况
                if (TryHandleExistingScripts(targetGo, className, outputFolder))
                {
                    return;
                }

                // 首次生成脚本
                GenerateNewScripts(targetGo, className, outputFolder, uiType);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to generate UI scripts for: {e.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// 验证输入参数
        /// </summary>
        private static bool ValidateInputParameters(GameObject targetGo, string className, string outputFolder)
        {
            if (!targetGo)
            {
                Debug.LogError("Target GameObject is null");
                return false;
            }

            if (string.IsNullOrEmpty(className))
            {
                Debug.LogError("Class name is null or empty");
                return false;
            }

            if (string.IsNullOrEmpty(outputFolder))
            {
                Debug.LogError("Output folder is null or empty");
                return false;
            }

            return true;
        }
        
        /// <summary>
        /// 尝试处理已有UIComponent的情况
        /// </summary>
        /// <returns>如果处理了已有UIComponent返回true</returns>
        private static bool TryHandleExistingUIComponent(GameObject targetGo)
        {
            UIComponent existingUIComponent = targetGo.GetComponent<UIComponent>();
            if (existingUIComponent == null)
            {
                return false;
            }

            string existingClassName = existingUIComponent.GetType().Name;
            Debug.Log($"GameObject already has UIComponent: {existingClassName}, updating designer script only");

            string existingDesignerPath = FindDesignerScriptPath(existingClassName);
            if (!string.IsNullOrEmpty(existingDesignerPath))
            {
                string existingOutputFolder = Path.GetDirectoryName(existingDesignerPath);
                RegenerateDesignerScriptOnly(existingUIComponent, existingOutputFolder);
            }
            else
            {
                Debug.LogError($"Could not find existing Designer script for {existingClassName}");
            }

            return true;
        }
        
        /// <summary>
        /// 确保输出目录存在
        /// </summary>
        private static void EnsureOutputDirectory(string outputFolder)
        {
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
                Debug.Log($"Created output directory: {outputFolder}");
            }
        }
        
        /// <summary>
        /// 尝试处理脚本已存在的情况
        /// </summary>
        /// <returns>如果处理了已存在脚本返回true</returns>
        private static bool TryHandleExistingScripts(GameObject targetGo, string className, string outputFolder)
        {
            bool logicScriptExists = File.Exists(Path.Combine(outputFolder, className + ".cs"));
            bool designerScriptExists = File.Exists(Path.Combine(outputFolder, className + ".Designer.cs"));

            if (!logicScriptExists || !designerScriptExists)
            {
                return false;
            }

            Debug.Log($"Logic script for {className} already exists, skipping generation");

            return true;
        }
        
        /// <summary>
        /// 生成新的脚本（首次生成）
        /// </summary>
        private static void GenerateNewScripts(GameObject targetGo, string className, string outputFolder, UIType uiType)
        {
            // 收集字段
            List<ComponentField> fields = CollectComponentFields(targetGo);

            // 生成逻辑脚本
            string logicScript = GenerateLogicScript(className, fields, uiType);
            string logicScriptPath = Path.Combine(outputFolder, className + ".cs");
            if (string.IsNullOrEmpty(logicScript))
            {
                throw new Exception("logicScript is null or empty");
            }
            File.WriteAllText(logicScriptPath, logicScript);
            Debug.Log($"Generated logic script: {logicScriptPath}");

            // 生成设计器脚本
            string designerScript = GenerateDesignerScript(className, fields);
            string designerScriptPath = Path.Combine(outputFolder, className + ".Designer.cs");
            File.WriteAllText(designerScriptPath, designerScript);
            Debug.Log($"Generated designer script: {designerScriptPath}");

            UITypeInfo typeInfo = UITypeConfig.GetInfo(uiType);
            Debug.Log($"Successfully generated {typeInfo.DisplayName} scripts for {className} to {outputFolder}");

            // 添加到UISerializer的待处理列表，等待编译完成
            UISerializer.AddPendingAttachment(GetGameObjectPath(targetGo), className);
        }
        
        /// <summary>
        /// 查找Designer脚本的路径
        /// </summary>
        /// <param name="className">类名</param>
        /// <returns>Designer脚本路径，如果没找到返回null</returns>
        public static string FindDesignerScriptPath(string className)
        {
            string designerFileName = className + ".Designer.cs";
            string[] guids = AssetDatabase.FindAssets($"{className} t:Script");
            
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (assetPath.EndsWith(designerFileName))
                {
                    return assetPath;
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// 仅重新生成Designer脚本并重新绑定
        /// </summary>
        /// <param name="uiComponent">UI组件实例</param>
        /// <param name="outputFolder">输出文件夹路径</param>
        public static void RegenerateDesignerScriptOnly(UIComponent uiComponent, string outputFolder)
        {
            if (uiComponent == null)
            {
                Debug.LogError("UIComponent is null, cannot regenerate designer script");
                return;
            }

            GameObject targetGo = uiComponent.gameObject;
            string className = uiComponent.GetType().Name;

            try
            {
                Debug.Log($"Updating Designer script for {className}...");

                // 1. 清空现有字段的引用（避免引用旧的、可能已删除的组件）
                uiComponent.ClearUIComponentFields();

                // 2. 收集新的字段
                List<ComponentField> fields = CollectComponentFields(targetGo);
                Debug.Log($"Collected {fields.Count} fields from GameObject hierarchy");

                // 3. 重新生成设计器脚本
                string designerScript = GenerateDesignerScript(className, fields);
                string designerScriptPath = Path.Combine(outputFolder, className + ".Designer.cs");
                File.WriteAllText(designerScriptPath, designerScript);
                Debug.Log($"Regenerated designer script: {designerScriptPath}");
                
                // todo 生成状态机相关代码

                // 4. 强制导入脚本并刷新
                AssetDatabase.ImportAsset(designerScriptPath, ImportAssetOptions.ForceUpdate);
                AssetDatabase.Refresh();

                // 5. 等待编译完成后重新绑定字段
                // 使用双重延迟确保编译完成
                EditorApplication.delayCall += () =>
                {
                    EditorApplication.delayCall += () =>
                    {
                        if (uiComponent != null && targetGo != null)
                        {
                            Debug.Log($"Re-binding fields for {className}...");
                            uiComponent.BindUIComponent();
                            Debug.Log($"Successfully updated and re-bound {className}");

                            // 高亮显示GameObject
                            Selection.activeGameObject = targetGo;
                            EditorGUIUtility.PingObject(targetGo);
                        }
                    };
                };
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to regenerate designer script for {className}: {e.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// 获取GameObject的完整路径
        /// </summary>
        /// <param name="go">目标GameObject</param>
        /// <returns>完整路径字符串</returns>
        private static string GetGameObjectPath(GameObject go)
        {
            if (go == null) return "";

            string path = go.name;
            Transform parent = go.transform.parent;
            while (parent != null)
            {
                string parentName = parent.name;
                if (!string.IsNullOrEmpty(parentName))
                {
                    path = parentName + "/" + path;
                }

                parent = parent.parent;
            }

            return path;
        }

        /// <summary>
        /// 生成逻辑脚本内容
        /// </summary>
        /// <param name="className">类名</param>
        /// <param name="fields">组件字段列表</param>
        /// <returns>逻辑脚本内容</returns>
        private static string GenerateLogicScript(string className, List<ComponentField> fields, UIType uiType)
        {
            var templatePath = GetLogicTemplatePath(uiType);
            if (!File.Exists(templatePath))
            {
                throw new Exception($"Logic template file not found: {templatePath}");
            }

            return GenerateScriptFromTemplate(templatePath, className, fields);
        }

        /// <summary>
        /// 生成设计器脚本内容
        /// </summary>
        /// <param name="className">类名</param>
        /// <param name="fields">组件字段列表</param>
        /// <returns>设计器脚本内容</returns>
        private static string GenerateDesignerScript(string className, List<ComponentField> fields)
        {
            var templatePath = GetDesignerTemplatePath();
            if (!File.Exists(templatePath))
            {
                throw new Exception($"Designer template file not found: {templatePath}");
            }

            return GenerateScriptFromTemplate(templatePath, className, fields);
        }

        /// <summary>
        /// 根据模板生成脚本内容
        /// </summary>
        /// <param name="templatePath">模板文件路径</param>
        /// <param name="className">类名</param>
        /// <param name="fields">组件字段列表</param>
        /// <returns>生成的脚本内容</returns>
        private static string GenerateScriptFromTemplate(string templatePath, string className,
            List<ComponentField> fields)
        {
            string template = File.ReadAllText(templatePath);

            // 替换类名
            template = template.Replace("#CLASSNAME#", className);

            // 替换字段
            if (template.Contains("#FIELDS#"))
            {
                StringBuilder fieldsBuilder = new StringBuilder();
                foreach (ComponentField field in fields)
                {
                    // 为字段生成标准格式
                    fieldsBuilder.AppendLine($"\t\t[SerializeField] private {field.fieldType} {field.fieldName};");
                }

                template = template.Replace("#FIELDS#", fieldsBuilder.ToString().TrimEnd());
            }

            return template;
        }

        /// <summary>
        /// 获取逻辑脚本模板文件路径
        /// </summary>
        /// <param name="uiType">UI类型</param>
        /// <returns>逻辑脚本模板文件路径</returns>
        private static string GetLogicTemplatePath(UIType uiType)
        {
            var info = UITypeConfig.GetInfo(uiType);
            return FindTemplateFile(info.TemplateFileName);
        }

        /// <summary>
        /// 获取设计器脚本模板文件路径
        /// </summary>
        /// <returns>设计器脚本模板文件路径</returns>
        private static string GetDesignerTemplatePath()
        {
            return FindTemplateFile("UIDesignerTemplate.txt");
        }
        
        /// <summary>
        /// 查找模板文件的实际路径
        /// </summary>
        /// <param name="templateFileName">模板文件名</param>
        /// <returns>模板文件的完整路径</returns>
        private static string FindTemplateFile(string templateFileName)
        {
            // 使用AssetDatabase查找模板文件
            string[] guids = AssetDatabase.FindAssets(Path.GetFileNameWithoutExtension(templateFileName) + " t:TextAsset");
            
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (assetPath.Contains("Templates") && assetPath.EndsWith(templateFileName))
                {
                    return Path.GetFullPath(assetPath);
                }
            }
            
            throw new FileNotFoundException($"Could not find template file: {templateFileName}. Please ensure the template exists in the Templates folder.");
        }

        /// <summary>
        /// 将GameObject名称转换为字段名（首字母小写）
        /// </summary>
        /// <param name="name">GameObject名称</param>
        /// <returns>字段名</returns>
        private static string ToFieldName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;
            
            // 首字母转小写
            return char.ToLower(name[0]) + name.Substring(1);
        }
        
        /// <summary>
        /// 收集GameObject中的组件字段信息
        /// </summary>
        /// <param name="root">根GameObject</param>
        /// <returns>组件字段列表</returns>
        private static List<ComponentField> CollectComponentFields(GameObject root)
        {
            List<ComponentField> fields = new List<ComponentField>();
            
            // 从根节点的子节点开始收集，跳过根节点自身的组件
            for (int i = 0; i < root.transform.childCount; i++)
            {
                Transform child = root.transform.GetChild(i);
                if (child != null)
                {
                    CollectComponentFieldsRecursive(child, fields, "");
                }
            }
            
            return fields;
        }

        /// <summary>
        /// 递归收集组件字段信息
        /// </summary>
        /// <param name="transform">当前Transform</param>
        /// <param name="fields">字段列表</param>
        /// <param name="path">当前路径</param>
        private static void CollectComponentFieldsRecursive(Transform transform, List<ComponentField> fields, string path)
        {
            if (transform == null) return;

            Component[] components = transform.GetComponents<Component>();
            bool hasUIComponent = false;

            // 收集当前GameObject上所有需要的组件
            List<ComponentField> currentObjectFields = new List<ComponentField>();

            // 先检查是否有自定义UIComponent
            foreach (Component component in components)
            {
                if (component == null) continue;

                Type componentType = component.GetType();

                if (UIComponentTypes.IsUIComponent(componentType))
                {
                    hasUIComponent = true;
                    string fieldName = ToFieldName(transform.name ?? "");

                    // 添加UIComponent字段
                    currentObjectFields.Add(new ComponentField
                    {
                        fieldName = fieldName,
                        fieldType = componentType.Name,
                        isBasicType = false,
                        gameObjectPath = path + transform.name,
                        isUIComponent = true
                    });

                    break;
                }
            }

            // 收集基础组件（无论是否有UIComponent都要检查）
            foreach (Component component in components)
            {
                if (component == null) continue;

                Type componentType = component.GetType();

                if (!UIComponentTypes.IsBasicType(componentType)) continue;

                string fieldName = ToFieldName(transform.name ?? "");

                // 检查是否已经在当前对象字段中存在相同类型
                bool fieldExists = currentObjectFields.Any(f => f.fieldType == componentType.Name);
                if (!fieldExists)
                {
                    currentObjectFields.Add(new ComponentField
                    {
                        fieldName = fieldName,
                        fieldType = componentType.Name,
                        isBasicType = true,
                        gameObjectPath = path + transform.name,
                        isUIComponent = false
                    });
                }
            }

            // 如果同一GameObject上有多个组件，添加类型后缀避免重名
            if (currentObjectFields.Count > 1)
            {
                foreach (ComponentField field in currentObjectFields)
                {
                    // 添加组件类型作为后缀
                    field.fieldName = field.fieldName + field.fieldType;
                }
            }

            // 将当前对象的所有字段添加到总字段列表
            fields.AddRange(currentObjectFields);

            // 关键逻辑：只有在当前节点没有UIComponent时，才继续遍历子节点
            // 如果有UIComponent，则停止深入，因为该UIComponent应该管理自己的子组件
            if (!hasUIComponent)
            {
                string currentPath = string.IsNullOrEmpty(path) ? transform.name : path + "/" + transform.name;
                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform child = transform.GetChild(i);
                    if (child != null)
                    {
                        CollectComponentFieldsRecursive(child, fields, currentPath + "/");
                    }
                }
            }
        }
    }

    /// <summary>
    /// 组件字段信息
    /// </summary>
    [Serializable]
    public class ComponentField
    {
        /// <summary>
        /// 字段名
        /// </summary>
        public string fieldName;

        /// <summary>
        /// 字段类型
        /// </summary>
        public string fieldType;

        /// <summary>
        /// 是否为基础类型
        /// </summary>
        public bool isBasicType;

        /// <summary>
        /// GameObject路径
        /// </summary>
        public string gameObjectPath;

        /// <summary>
        /// 是否为UIComponent
        /// </summary>
        public bool isUIComponent;
    }
}