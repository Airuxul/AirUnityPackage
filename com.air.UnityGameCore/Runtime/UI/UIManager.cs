using System.Collections.Generic;

namespace Air.UnityGameCore.Runtime.UI
{
    public class UIManager
    {
        private Stack<UIPanel> _uiStack;

        public void ShowPanel<T>() where T : UIPanel
        {
            
        }

        public void ClosePanel()
        {
            
        }
    }
}