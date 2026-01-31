using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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
        protected List<UIComponent> childs;

        public bool IsInit { get; set; }
        public bool IsDestoryed { get; set; }
        public UIConfig UIConfig { get; set; }
        public UIShowParam UIShowParam { get; set; }

        #region UI生命周期

        public void Init()
        {
            if (IsInit) return;
            IsInit = true;
            IsDestoryed = false;
            foreach (var child in childs)
            {
                child.Init();   
            }
            OnUIInit();
        }

        public void Show(UIShowParam param)
        {
            UIShowParam = param;
            foreach (var child in childs)
            {
                child.Show(param);
            }
            OnUIShow(param);
        }

        public void Hide()
        {
            OnUIHide();
            foreach (var child in childs)
            {
                child.Hide();
            }
        }

        public void Destory()
        {
            OnUIDestory();
            IsInit = false;
            IsDestoryed = true;
            foreach (var child in childs)
            {
                child.Destory();
            }
        }

        /// <summary>
        /// 初始化UI组件
        /// </summary>
        protected virtual void OnUIInit()
        {
        }

        /// <summary>
        /// 显示UI组件
        /// </summary>
        protected virtual void OnUIShow(UIShowParam param)
        {
            // 子类可以重写此方法进行显示逻辑
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