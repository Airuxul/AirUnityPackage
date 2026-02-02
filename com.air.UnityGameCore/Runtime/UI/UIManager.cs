using System.Collections.Generic;
using Air.UnityGameCore.Runtime.Resource;
using UnityEngine;

namespace Air.UnityGameCore.Runtime.UI
{
    public class UIManager
    {
        private readonly IResManager _resManager;
        private readonly Dictionary<string, UIPanel> _uiMap;

        public UIManager(IResManager resManager)
        {
            _resManager = resManager;
            _uiMap = new();
        }

        public UIPanel GetUIPanel(string uiPanelId)
        {
            return _uiMap[uiPanelId];
        }
        
        public void ShowPanel(UIPanelConfig uiPanelConfig)
        {
            var prefabPath = uiPanelConfig.PrefabPath;
            _resManager.LoadInstanceAsync<GameObject>(prefabPath, go =>
            {
                if (!go.TryGetComponent(out UIPanel uiPanel))
                {
                    Debug.LogError($"cant find UIPanel component, path: {prefabPath}");
                    return;
                }
                // todo 设置父物体
                uiPanel.Init(uiPanelConfig);
                uiPanel.Show(new UIShowParam());
                _uiMap.Add(uiPanelConfig.UIPanelId, uiPanel);
            });
        }

        public void DestoryPanel(UIPanel uiPanel)
        {
            var uiPanelConfig = uiPanel.UIPanelConfig;
            _uiMap.Remove(uiPanelConfig.UIPanelId);
            uiPanel.Destory();
            Object.Destroy(uiPanel.gameObject);
            _resManager.UnloadRes(uiPanelConfig.PrefabPath);
        }
    }
}