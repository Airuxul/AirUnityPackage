using System.Collections.Generic;

namespace Air.UnityGameCore.Editor.UI
{
    /// <summary>
    /// UI类型枚举
    /// </summary>
    public enum UIType
    {
        Panel,
        Component
    }

    /// <summary>
    /// UI类型配置信息
    /// </summary>
    public class UITypeInfo
    {
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }
        
        /// <summary>
        /// 输出文件夹名称
        /// </summary>
        public string FolderName { get; set; }
        
        /// <summary>
        /// 逻辑脚本模板文件名
        /// </summary>
        public string TemplateFileName { get; set; }
        
        /// <summary>
        /// 基类名称
        /// </summary>
        public string BaseClassName { get; set; }
        
        /// <summary>
        /// 命名空间
        /// </summary>
        public string Namespace { get; set; }
    }

    /// <summary>
    /// UI类型配置管理器
    /// </summary>
    public static class UITypeConfig
    {
        private static readonly Dictionary<UIType, UITypeInfo> _configs = new Dictionary<UIType, UITypeInfo>
        {
            {
                UIType.Panel, new UITypeInfo
                {
                    DisplayName = "Panel",
                    FolderName = "Panels",
                    TemplateFileName = "UIPanelLogicTemplate.txt",
                    BaseClassName = "UIPanel",
                    Namespace = "Air.UnityGameCore.Generated.Panel"
                }
            },
            {
                UIType.Component, new UITypeInfo
                {
                    DisplayName = "Component",
                    FolderName = "Components",
                    TemplateFileName = "UICompLogicTemplate.txt",
                    BaseClassName = "UIComponent",
                    Namespace = "Air.UnityGameCore.Generated.Comp"
                }
            }
        };

        /// <summary>
        /// 获取UI类型配置信息
        /// </summary>
        public static UITypeInfo GetInfo(UIType type)
        {
            return _configs.TryGetValue(type, out var info) ? info : null;
        }

        /// <summary>
        /// 获取所有可用的UI类型
        /// </summary>
        public static UIType[] GetAllTypes()
        {
            return new[] { UIType.Panel, UIType.Component };
        }

        /// <summary>
        /// 根据基类名称判断UI类型
        /// </summary>
        public static UIType GetTypeByBaseClass(System.Type componentType)
        {
            while (componentType != null)
            {
                foreach (var kvp in _configs)
                {
                    if (componentType.Name == kvp.Value.BaseClassName)
                    {
                        return kvp.Key;
                    }
                }
                componentType = componentType.BaseType;
            }
            
            // 默认返回Component类型
            return UIType.Component;
        }
    }
}
