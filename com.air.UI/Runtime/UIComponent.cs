using UnityEngine;

namespace AirUI.Core
{
    /// <summary>
    /// UI组件基类，提供UI生命周期管理
    /// </summary>
    public abstract class UIComponent : MonoBehaviour
    {
        /// <summary>
        /// 组件是否已初始化
        /// </summary>
        protected bool IsInitialized { get; private set; }

        [SerializeField] protected UIComponent parent;

        #region Unity生命周期

        protected virtual void Awake()
        {
            // 子类可以重写此方法进行初始化
            OnInit();
        }

        protected virtual void Start()
        {
            // 子类可以重写此方法进行启动逻辑
        }

        protected virtual void OnEnable()
        {
            // 子类可以重写此方法进行启用逻辑
        }

        protected virtual void OnDisable()
        {
            // 子类可以重写此方法进行禁用逻辑
        }

        protected virtual void OnDestroy()
        {
            // 子类可以重写此方法进行清理逻辑
        }

        #endregion

        #region UI生命周期

        /// <summary>
        /// 初始化UI组件
        /// </summary>
        public virtual void OnInit()
        {
            if (IsInitialized) return;
            // 子类可以重写此方法进行初始化逻辑
            IsInitialized = true;
        }

        /// <summary>
        /// 显示UI组件
        /// </summary>
        public virtual void OnShow()
        {
            // 子类可以重写此方法进行显示逻辑
        }

        /// <summary>
        /// 隐藏UI组件
        /// </summary>
        public virtual void OnHide()
        {
            // 子类可以重写此方法进行隐藏逻辑
        }

        #endregion
    }
}