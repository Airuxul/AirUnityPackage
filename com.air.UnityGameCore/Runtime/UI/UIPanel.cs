namespace Air.UnityGameCore.Runtime.UI
{
    public class UIPanel : UIComponent
    {
        public UIPanelConfig UIPanelConfig { get; private set; }

        public void Init(UIPanelConfig uiPanelConfig)
        {
            UIPanelConfig = uiPanelConfig;
            base.Init();
        }
    }
}