namespace UI
{
    public class UIPanel : UIComponent
    {
        public UIPanelConfig UIPanelConfig { get; set; }
        public void Init(UIPanelConfig uiPanelConfig)
        {
            UIPanelConfig = uiPanelConfig;
            base.Init();
        }
    }
}