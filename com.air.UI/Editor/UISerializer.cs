using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using AirUI.Core;

namespace AirUI.Editor
{
    /// <summary>
    /// UI序列化器，负责自动绑定UI组件字段和挂载脚本
    /// </summary>
    public static class UISerializer
    {
        private const string PendingGoPathKey = "UISerializer_PendingGameObjectPath";
        private const string PendingClassNameKey = "UISerializer_PendingClassName";
        
        [DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            // 检查是否有待处理的任务
            if (HasPendingTask())
            {
                ProcessPendingTask();
            }
        }
        
        /// <summary>
        /// 添加待处理的附件任务
        /// </summary>
        /// <param name="gameObjectPath">GameObject路径</param>
        /// <param name="className">类名</param>
        public static void AddPendingAttachment(string gameObjectPath, string className)
        {
            // 保存到EditorPrefs
            EditorPrefs.SetString(PendingGoPathKey, gameObjectPath);
            EditorPrefs.SetString(PendingClassNameKey, className);
        }
        
        /// <summary>
        /// 检查是否有待处理的任务
        /// </summary>
        /// <returns>如果有待处理任务返回true</returns>
        private static bool HasPendingTask()
        {
            return EditorPrefs.HasKey(PendingGoPathKey);
        }
        
        /// <summary>
        /// 处理待处理的任务
        /// </summary>
        private static void ProcessPendingTask()
        {
            string gameObjectPath = EditorPrefs.GetString(PendingGoPathKey);
            string className = EditorPrefs.GetString(PendingClassNameKey);
            
            Debug.Log($"ProcessPendingTask: {gameObjectPath}, {className}");
            
            GameObject go = FindGameObjectByPath(gameObjectPath);
            if (go != null)
            {
                PostGenerateProcess(go, className);
            }
            else
            {
                Debug.LogWarning($"Could not find GameObject at path: {gameObjectPath}");
            }
            
            // 清除EditorPrefs
            ClearPendingTask();
        }
        
        /// <summary>
        /// 清除待处理任务
        /// </summary>
        private static void ClearPendingTask()
        {
            EditorPrefs.DeleteKey(PendingGoPathKey);
            EditorPrefs.DeleteKey(PendingClassNameKey);
        }
        
        /// <summary>
        /// 绑定UI组件字段
        /// </summary>
        /// <param name="uiComponent">UI组件实例</param>
        public static void BindUIComponent(this UIComponent uiComponent)
        {
            if (!uiComponent) return;
            
            Type componentType = uiComponent.GetType();
            FieldInfo[] fields = componentType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            
            foreach (FieldInfo field in fields)
            {
                if (field.GetCustomAttribute<SerializeField>() != null)
                {
                    string fieldName = field.Name;
                    Type fieldType = field.FieldType;
                    
                    // 特殊处理Parent字段
                    if (fieldName.EndsWith("_Parent") && fieldType == typeof(UIComponent))
                    {
                        BindParentField(uiComponent, field, fieldName);
                        continue;
                    }
                    
                    Component foundComponent = FindComponentInChildren(uiComponent.transform, fieldType, fieldName);
                    if (foundComponent)
                    {
                        field.SetValue(uiComponent, foundComponent);
                        Debug.Log($"Bound field {fieldName} to {foundComponent.name}");
                    }
                }
            }
            
            EditorUtility.SetDirty(uiComponent);
        }
        
        /// <summary>
        /// 绑定Parent字段
        /// </summary>
        /// <param name="uiComponent">UI组件实例</param>
        /// <param name="field">字段信息</param>
        /// <param name="fieldName">字段名</param>
        private static void BindParentField(UIComponent uiComponent, FieldInfo field, string fieldName)
        {
            // 从字段名中提取对应的UI组件名（去掉_Parent后缀）
            string componentFieldName = fieldName.Replace("_Parent", "");
            
            // 查找对应的UI组件字段
            FieldInfo componentField = uiComponent.GetType().GetField(componentFieldName, 
                BindingFlags.NonPublic | BindingFlags.Instance);
            
            if (componentField != null)
            {
                // 获取UI组件实例
                object componentInstance = componentField.GetValue(uiComponent);
                if (componentInstance != null)
                {
                    // 查找父级UIComponent
                    UIComponent parentComponent = FindParentUIComponent(uiComponent.transform);
                    if (parentComponent != null)
                    {
                        field.SetValue(uiComponent, parentComponent);
                        Debug.Log($"Bound Parent field {fieldName} to {parentComponent.GetType().Name}");
                    }
                    else
                    {
                        Debug.LogWarning($"Could not find parent UIComponent for {fieldName}");
                    }
                }
                else
                {
                    Debug.LogWarning($"Component field {componentFieldName} is null, cannot bind Parent field {fieldName}");
                }
            }
            else
            {
                Debug.LogWarning($"Could not find component field {componentFieldName} for Parent field {fieldName}");
            }
        }
        
        /// <summary>
        /// 查找父级UIComponent
        /// </summary>
        /// <param name="transform">当前Transform</param>
        /// <returns>父级UIComponent，如果没找到返回null</returns>
        private static UIComponent FindParentUIComponent(Transform transform)
        {
            Transform parent = transform.parent;
            while (parent != null)
            {
                UIComponent parentComponent = parent.GetComponent<UIComponent>();
                if (parentComponent != null)
                {
                    return parentComponent;
                }
                parent = parent.parent;
            }
            return null;
        }
        
        /// <summary>
        /// 在子物体中查找指定类型的组件
        /// </summary>
        /// <param name="parent">父Transform</param>
        /// <param name="componentType">组件类型</param>
        /// <param name="fieldName">字段名</param>
        /// <returns>找到的组件，如果没找到返回null</returns>
        private static Component FindComponentInChildren(Transform parent, Type componentType, string fieldName)
        {
            Component[] components = parent.GetComponentsInChildren(componentType, true);
            
            if (components.Length == 1)
            {
                return components[0];
            }
            
            // 尝试通过名称匹配
            foreach (Component component in components)
            {
                string componentName = component.name.Replace(" ", "").Replace("(", "").Replace(")", "");
                string expectedName = char.ToLower(componentName[0]) + componentName.Substring(1) + componentType.Name;
                
                if (expectedName == fieldName)
                {
                    return component;
                }
            }
            
            // 尝试通过字段名包含组件名的方式匹配
            foreach (Component component in components)
            {
                if (component.name.IndexOf(fieldName.Replace(componentType.Name, ""), StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return component;
                }
            }
            
            return components.Length > 0 ? components[0] : null;
        }
        
        /// <summary>
        /// 生成后的处理流程
        /// </summary>
        /// <param name="targetGo">目标GameObject</param>
        /// <param name="className">类名</param>
        private static void PostGenerateProcess(GameObject targetGo, string className)
        {
            if (!targetGo) return;
            
            // 挂载脚本
            AttachScriptToGameObject(targetGo, className);
            
            // 绑定字段
            if (targetGo.TryGetComponent(out UIComponent uiComponent))
            {
                // 脚本已经编译完成，立即绑定
                BindUIComponent(uiComponent);
            }
        }
        
        /// <summary>
        /// 将脚本挂载到GameObject上
        /// </summary>
        /// <param name="targetGo">目标GameObject</param>
        /// <param name="className">类名</param>
        private static void AttachScriptToGameObject(GameObject targetGo, string className)
        {
            // 检查是否已经有UIComponent类型的组件
            UIComponent existingUIComponent = targetGo.GetComponent<UIComponent>();
            if (existingUIComponent)
            {
                Debug.Log($"UIComponent already attached to {targetGo.name}");
                return;
            }
            
            // 尝试通过MonoScript挂载脚本
            MonoScript[] scripts = MonoImporter.GetAllRuntimeMonoScripts();
            MonoScript targetScript = null;
            
            foreach (MonoScript script in scripts)
            {
                if (script.GetClass() != null && script.GetClass().Name == className)
                {
                    targetScript = script;
                    break;
                }
            }
            
            if (targetScript && targetScript.GetClass() != null)
            {
                targetGo.AddComponent(targetScript.GetClass());
                Debug.Log($"Attached script {className} to {targetGo.name}");
                EditorUtility.SetDirty(targetGo);
            }
            else
            {
                // 备用方法：通过反射
                Type scriptType = GetTypeByName(className);
                if (scriptType != null)
                {
                    targetGo.AddComponent(scriptType);
                    Debug.Log($"Attached script {className} to {targetGo.name} via reflection");
                    EditorUtility.SetDirty(targetGo);
                }
                else
                {
                    Debug.LogError($"Could not find script type {className}. Script compilation may have failed.");
                }
            }
        }
        
        /// <summary>
        /// 通过类型名获取类型
        /// </summary>
        /// <param name="typeName">类型名</param>
        /// <returns>类型实例，如果没找到返回null</returns>
        private static Type GetTypeByName(string typeName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(typeName);
                if (type != null)
                    return type;
            }
            return null;
        }

        /// <summary>
        /// 通过路径查找GameObject
        /// </summary>
        /// <param name="path">GameObject路径</param>
        /// <returns>找到的GameObject，如果没找到返回null</returns>
        private static GameObject FindGameObjectByPath(string path)
        {
            string[] parts = path.Split('/');
            GameObject current = GameObject.Find(parts[0]);
            
            for (int i = 1; i < parts.Length && current != null; i++)
            {
                Transform child = current.transform.Find(parts[i]);
                current = child != null ? child.gameObject : null;
            }
            
            return current;
        }
    }

    /// <summary>
    /// UIComponent的自定义编辑器
    /// </summary>
    [CustomEditor(typeof(UIComponent), true)]
    public class UIComponentEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Bind Fields"))
            {
                UISerializer.BindUIComponent(target as UIComponent);
            }
        }
    }
}
