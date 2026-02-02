using System.Collections.Generic;
using UnityEngine;

namespace Air.UnityGameCore.Runtime.UI
{
    /// <summary>
    /// UI组件基类，提供UI生命周期管理
    /// </summary>
    public abstract class UIComponent : MonoBehaviour
    {
        [SerializeField] 
        private UIComponent parent;

        public UIComponent Parent
        {
            set => parent = value;
            get => parent;
        }

        [SerializeField]
        protected List<UIComponent> childs = new();

        public List<UIComponent> Childs => childs;

        public bool IsInit { get; set; }
        public bool IsDestoryed { get; set; }

        public UIShowParam UIShowParam { get; set; }
        
        // todo UI触发器，触发事件或者动画
        // todo UI状态机，快捷修改UI状态

        #region UI生命周期

        protected void Init()
        {
            if (IsInit) return;
            IsInit = true;
            IsDestoryed = false;
            if (childs != null)
            {
                foreach (var child in Childs)
                {
                    child?.Init();
                }
            }
            OnUIInit();
        }

        public void Show(UIShowParam param)
        {
            UIShowParam = param;
            foreach (var child in Childs)
            {
                child?.Destory();
            }
            OnUIShow(param);
            // todo 动画逻辑
            
        }
        

        public void Hide()
        {
            OnUIHide();
            foreach (var child in Childs)
            {
                child?.Destory();
            }
            // todo hide动画
        }

        public void Destory()
        {
            OnUIDestory();
            IsInit = false;
            IsDestoryed = true;
            foreach (var child in Childs)
            {
                child?.Destory();
            }
        }

        /// <summary>
        /// 初始化UI组件
        /// </summary>
        protected virtual void OnUIInit()
        {
            // 子类可以重写此方法进行初始化逻辑
        }

        /// <summary>
        /// 显示UI组件
        /// </summary>
        protected virtual void OnUIShow(UIShowParam param)
        {
            // 子类可以重写此方法进行显示逻辑
        }

        
        /// <summary>
        /// 显示完UI组件后回调
        /// </summary>
        protected virtual void OnUIShowAfter()
        {
            // 子类可以重写此方法进行动画展示完后的逻辑
        }

        /// <summary>
        /// 隐藏UI组件
        /// </summary>
        protected virtual void OnUIHide()
        {
            // 子类可以重写此方法进行隐藏逻辑
        }

        /// <summary>
        /// 销毁UI组件
        /// </summary>
        protected virtual void OnUIDestory()
        {
            
        }
        #endregion
    }
}