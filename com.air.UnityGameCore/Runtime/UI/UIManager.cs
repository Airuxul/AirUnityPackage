using System.Collections.Generic;

namespace Air.UnityGameCore.Runtime.UI
{
    public class UIManager
    {
        private Stack<UIPanel> _uiStack;

        public void ShowPanel<T>(UIConfig uiConfig) where T : UIPanel
        {
            
        }

        public void ClosePanel()
        {
            
        }
    }
}