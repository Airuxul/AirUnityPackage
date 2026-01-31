using System;
using System.Collections.Generic;

namespace Air.UnityGameCore.Runtime.UI
{
    /// <summary>
    /// UI配置数据
    /// </summary>
    public class UIConfig
    {
        /// <summary>
        /// 显示顺序，数值越大越靠前
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Prefab资源路径
        /// </summary>
        public string PrefabPath { get; set; }

        /// <summary>
        /// UI类型（用于实例化后类型转换）
        /// </summary>
        public Type PanelType { get; set; }
        
        /// <summary>
        /// UI Show 参数类型
        /// </summary>
        public Type ShowParamType { get; set; }

        /// <summary>
        /// UI所在层级
        /// </summary>
        public string UILayer { get; set; }
    }

    /// <summary>
    /// UI配置注册中心（框架层，不包含具体业务配置）
    /// </summary>
    public static class UIConfigRegistry
    {
        private static readonly Dictionary<string, UIConfig> _configMap = new();
        private static bool _isInitialized = false;

        /// <summary>
        /// 注册UI配置
        /// </summary>
        /// <param name="panelId">面板唯一标识符</param>
        /// <param name="config">UI配置</param>
        public static void Register(string panelId, UIConfig config)
        {
            if (_configMap.ContainsKey(panelId))
            {
                UnityEngine.Debug.LogWarning($"UI配置重复注册: {panelId}，将覆盖旧配置");
            }
            _configMap[panelId] = config;
        }

        /// <summary>
        /// 批量注册UI配置
        /// </summary>
        /// <param name="configs">配置字典</param>
        public static void RegisterBatch(Dictionary<string, UIConfig> configs)
        {
            foreach (var kvp in configs)
            {
                Register(kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        /// 获取UI配置
        /// </summary>
        /// <param name="panelId">面板标识符</param>
        /// <returns>UI配置，如果不存在返回null</returns>
        public static UIConfig GetConfig(string panelId)
        {
            if (!_isInitialized)
            {
                UnityEngine.Debug.LogError("UIConfigRegistry未初始化，请先调用游戏层的配置注册方法");
                return null;
            }

            if (_configMap.TryGetValue(panelId, out var config))
            {
                return config;
            }

            UnityEngine.Debug.LogError($"未找到UI配置: {panelId}");
            return null;
        }

        /// <summary>
        /// 标记配置已初始化完成
        /// </summary>
        public static void MarkInitialized()
        {
            _isInitialized = true;
            UnityEngine.Debug.Log($"UIConfigRegistry初始化完成，共注册 {_configMap.Count} 个UI配置");
        }

        /// <summary>
        /// 清空所有配置（用于测试或重新初始化）
        /// </summary>
        public static void Clear()
        {
            _configMap.Clear();
            _isInitialized = false;
        }

        /// <summary>
        /// 检查是否已初始化
        /// </summary>
        public static bool IsInitialized => _isInitialized;

        /// <summary>
        /// 获取所有已注册的面板ID
        /// </summary>
        public static IEnumerable<string> GetAllPanelIds()
        {
            return _configMap.Keys;
        }
    }
}