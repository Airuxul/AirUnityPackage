using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using AirUI.Core;

namespace AirUI.Editor
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
        /// <param name="className">类名</param>
        /// <param name="outputFolder">输出文件夹路径</param>
        public static void GenerateUIScript(GameObject targetGo, string className, string outputFolder)
        {
            if (!targetGo)
            {
                Debug.LogError("Target GameObject is null");
                return;
            }

            if (string.IsNullOrEmpty(className))
            {
                Debug.LogError("Class name is null or empty");
                return;
            }

            if (string.IsNullOrEmpty(outputFolder))
            {
                Debug.LogError("Output folder is null or empty");
                return;
            }

            try
            {
                // 确保输出目录存在
                if (!Directory.Exists(outputFolder))
                {
                    Directory.CreateDirectory(outputFolder);
                }

                // 检查是否已经生成过该类的脚本
                bool logicScriptExists = File.Exists(Path.Combine(outputFolder, className + ".cs"));
                bool designerScriptExists = File.Exists(Path.Combine(outputFolder, className + ".Designer.cs"));
                
                // 如果逻辑脚本已存在，则仅重新生成设计器脚本
                if (logicScriptExists && designerScriptExists)
                {
                    Debug.Log($"Logic script for {className} already exists, regenerating designer script only");
                    RegenerateDesignerScriptOnly(targetGo, className, outputFolder);
                    return;
                }

                // 处理已挂载的UIComponent
                HandleExistingUIComponents(targetGo, className);

                List<ComponentField> fields = CollectComponentFields(targetGo);
                
                // 生成逻辑脚本（如果不存在）
                if (!logicScriptExists)
                {
                    string logicScript = GenerateLogicScript(className, fields);
                    string logicScriptPath = Path.Combine(outputFolder, className + ".cs");
                    File.WriteAllText(logicScriptPath, logicScript);
                    Debug.Log($"Generated logic script: {logicScriptPath}");
                }
                else
                {
                    Debug.Log($"Logic script already exists, skipping: {className}.cs");
                }

                // 生成设计器脚本
                string designerScript = GenerateDesignerScript(className, fields);
                string designerScriptPath = Path.Combine(outputFolder, className + ".Designer.cs");
                File.WriteAllText(designerScriptPath, designerScript);
                Debug.Log($"Generated designer script: {designerScriptPath}");

                Debug.Log($"Successfully generated UI scripts for {className} to {outputFolder}");
                
                // 添加到UISerializer的待处理列表，等待编译完成
                UISerializer.AddPendingAttachment(GetGameObjectPath(targetGo), className);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to generate UI scripts for {className}: {e.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// 仅重新生成设计器脚本
        /// </summary>
        /// <param name="targetGo">目标GameObject</param>
        /// <param name="className">类名</param>
        /// <param name="outputFolder">输出文件夹路径</param>
        private static void RegenerateDesignerScriptOnly(GameObject targetGo, string className, string outputFolder)
        {
            try
            {
                // 处理已挂载的UIComponent
                HandleExistingUIComponents(targetGo, className);
                
                List<ComponentField> fields = CollectComponentFields(targetGo);
                
                // 仅重新生成设计器脚本
                string designerScript = GenerateDesignerScript(className, fields);
                string designerScriptPath = Path.Combine(outputFolder, className + ".Designer.cs");
                File.WriteAllText(designerScriptPath, designerScript);
                Debug.Log($"Regenerated designer script: {designerScriptPath}");
                
                // 添加到UISerializer的待处理列表，等待编译完成
                UISerializer.AddPendingAttachment(GetGameObjectPath(targetGo), className);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to regenerate designer script for {className}: {e.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// 处理已挂载的UIComponent
        /// </summary>
        /// <param name="targetGo">目标GameObject</param>
        /// <param name="className">类名</param>
        private static void HandleExistingUIComponents(GameObject targetGo, string className)
        {
            // 查找并移除已挂载的UIComponent
            UIComponent[] existingComponents = targetGo.GetComponents<UIComponent>();
            
            foreach (UIComponent component in existingComponents)
            {
                if (component != null)
                {
                    string componentTypeName = component.GetType().Name;
                    
                    // 如果是同类型的组件，记录日志
                    if (componentTypeName == className)
                    {
                        Debug.Log($"Found existing {className} component, will be replaced after script generation");
                    }
                    else
                    {
                        Debug.Log($"Removing existing UIComponent: {componentTypeName}");
                    }
                    
                    // 移除组件
                    UnityEngine.Object.DestroyImmediate(component);
                }
            }
            
            // 刷新GameObject
            EditorUtility.SetDirty(targetGo);
        }
        
        /// <summary>
        /// 获取GameObject的完整路径
        /// </summary>
        /// <param name="go">目标GameObject</param>
        /// <returns>完整路径字符串</returns>
        private static string GetGameObjectPath(GameObject go)
        {
            if (go == null) return "";
            
            string path = go.name ?? "";
            Transform parent = go.transform?.parent;
            while (parent != null)
            {
                string parentName = parent.name ?? "";
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
        private static string GenerateLogicScript(string className, List<ComponentField> fields)
        {
            var templatePath = GetLogicTemplatePath();
            if (!File.Exists(templatePath))
            {
                Debug.LogError($"Logic template file not found: {templatePath}");
                return "";
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
                Debug.LogError($"Designer template file not found: {templatePath}");
                return "";
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
        private static string GenerateScriptFromTemplate(string templatePath, string className, List<ComponentField> fields)
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
                    if (field.isParentField)
                    {
                        // 为Parent字段生成特殊格式
                        fieldsBuilder.AppendLine($"        [SerializeField] private {field.fieldType} {field.fieldName}; // Parent: {field.parentComponentName}");
                    }
                    else
                    {
                        // 为普通字段生成标准格式
                        fieldsBuilder.AppendLine($"        [SerializeField] private {field.fieldType} {field.fieldName};");
                    }
                }
                template = template.Replace("#FIELDS#", fieldsBuilder.ToString().TrimEnd());
            }
            
            return template;
        }
        
        /// <summary>
        /// 获取逻辑脚本模板文件路径
        /// </summary>
        /// <returns>逻辑脚本模板文件路径</returns>
        private static string GetLogicTemplatePath()
        {
            return Path.Combine("Packages/com.airui/Editor/Templates/", "UILogicTemplate.txt");
        }
        
        /// <summary>
        /// 获取设计器脚本模板文件路径
        /// </summary>
        /// <returns>设计器脚本模板文件路径</returns>
        private static string GetDesignerTemplatePath()
        {
            return Path.Combine("Packages/com.airui/Editor/Templates/", "UIDesignerTemplate.txt");
        }
        
        /// <summary>
        /// 收集GameObject中的组件字段信息
        /// </summary>
        /// <param name="root">根GameObject</param>
        /// <returns>组件字段列表</returns>
        private static List<ComponentField> CollectComponentFields(GameObject root)
        {
            List<ComponentField> fields = new List<ComponentField>();
            CollectComponentFieldsRecursive(root.transform, fields, "", null);
            return fields;
        }
        
        /// <summary>
        /// 递归收集组件字段信息
        /// </summary>
        /// <param name="transform">当前Transform</param>
        /// <param name="fields">字段列表</param>
        /// <param name="path">当前路径</param>
        /// <param name="parentComponent">父UIComponent</param>
        private static void CollectComponentFieldsRecursive(Transform transform, List<ComponentField> fields, string path, UIComponent parentComponent)
        {
            if (transform == null) return;
            
            Component[] components = transform.GetComponents<Component>();
            bool hasUIComponent = false;
            UIComponent currentUIComponent = null;
            
            // 先检查是否有自定义UIComponent
            foreach (Component component in components)
            {
                if (component == null) continue;
                
                Type componentType = component.GetType();
                
                if (UIComponentTypes.IsUIComponent(componentType))
                {
                    hasUIComponent = true;
                    currentUIComponent = component as UIComponent;
                    string fieldName = transform.name ?? "";
                    
                    // 添加UIComponent字段
                    fields.Add(new ComponentField
                    {
                        fieldName = fieldName,
                        fieldType = componentType.Name,
                        isBasicType = false,
                        gameObjectPath = path + fieldName,
                        isUIComponent = true
                    });
                    
                    // 如果有父组件，添加Parent字段
                    if (parentComponent != null)
                    {
                        fields.Add(new ComponentField
                        {
                            fieldName = fieldName + "_Parent",
                            fieldType = "UIComponent",
                            isBasicType = false,
                            gameObjectPath = path + fieldName,
                            isUIComponent = false,
                            isParentField = true,
                            parentComponentName = parentComponent.GetType().Name
                        });
                    }
                    
                    break;
                }
            }
            
            // 检查基础组件（无论是否有UIComponent都要检查）
            foreach (Component component in components)
            {
                if (component == null) continue;
                
                Type componentType = component.GetType();
                
                if (UIComponentTypes.IsBasicType(componentType))
                {
                    string fieldName = transform.name ?? "";
                    
                    // 检查是否已经添加过相同名称和类型的字段，避免重复
                    bool fieldExists = fields.Any(f => f.fieldName == fieldName && f.fieldType == componentType.Name);
                    if (!fieldExists)
                    {
                        fields.Add(new ComponentField
                        {
                            fieldName = fieldName,
                            fieldType = componentType.Name,
                            isBasicType = true,
                            gameObjectPath = path + fieldName,
                            isUIComponent = false
                        });
                    }
                }
            }
            
            // 无论是否有UIComponent，都继续遍历子物体
            string currentPath = string.IsNullOrEmpty(path) ? "" : path + "/";
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child != null)
                {
                    // 如果有UIComponent，传递当前组件作为父组件；否则传递原来的父组件
                    UIComponent nextParentComponent = hasUIComponent ? currentUIComponent : parentComponent;
                    CollectComponentFieldsRecursive(child, fields, currentPath, nextParentComponent);
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
        
        /// <summary>
        /// 是否为Parent字段
        /// </summary>
        public bool isParentField;
        
        /// <summary>
        /// 父UIComponent的名称
        /// </summary>
        public string parentComponentName;
    }
}
