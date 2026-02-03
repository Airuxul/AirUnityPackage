using System;
using System.Collections.Generic;
using UnityEngine;

namespace Air.UnityGameCore.Runtime.Event
{
    /// <summary>
    /// 事件处理器。对本实例注册的事件进行记录，便于按名称或批量取消注册、清理，避免重复注册。
    /// </summary>
    public class EventHandler
    {
        private readonly Dictionary<string, Action> _unregisterActions = new();

        /// <summary>
        /// 注册无参事件。同一 eventName 仅允许注册一次，重复注册会打错并忽略。
        /// </summary>
        public void RegisterEvent(string eventName, Action callback)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                Debug.LogError("[EventHandler] 事件名称为空");
                return;
            }
            if (callback == null)
            {
                Debug.LogError("[EventHandler] 回调为空");
                return;
            }
            if (_unregisterActions.ContainsKey(eventName))
            {
                Debug.LogError($"[EventHandler] 已注册过该事件: {eventName}");
                return;
            }
            AppFacade.Instance.EventManager.RegisterEvent(eventName, callback);
            _unregisterActions[eventName] = () =>
            {
                AppFacade.Instance.EventManager.UnregisterEvent(eventName, callback);
            };
        }

        /// <summary>
        /// 注册带参事件。同一 eventName 仅允许注册一次，重复注册会打错并忽略。
        /// </summary>
        public void RegisterEvent<T>(string eventName, Action<T> callback)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                Debug.LogError("[EventHandler] 事件名称为空");
                return;
            }
            if (callback == null)
            {
                Debug.LogError("[EventHandler] 回调为空");
                return;
            }
            if (_unregisterActions.ContainsKey(eventName))
            {
                Debug.LogError($"[EventHandler] 已注册过该事件: {eventName}");
                return;
            }
            AppFacade.Instance.EventManager.RegisterEvent(eventName, callback);
            _unregisterActions[eventName] = () =>
            {
                AppFacade.Instance.EventManager.UnregisterEvent(eventName, callback);
            };
        }

        public void TriggerEvent(string eventName)
        {
            AppFacade.Instance.EventManager.TriggerEvent(eventName);
        }
        
        public void TriggerEvent<T>(string eventName, T param)
        {
            AppFacade.Instance.EventManager.TriggerEvent(eventName, param);
        }
        
        public void UnRegisterEvent(string eventName)
        {
            if (string.IsNullOrEmpty(eventName)) return;
            if (!_unregisterActions.TryGetValue(eventName, out var unregister))
            {
                Debug.LogWarning($"[EventHandler] 未找到已注册事件: {eventName}");
                return;
            }
            unregister.Invoke();
            _unregisterActions.Remove(eventName);
        }

        /// <summary>
        /// 清除本处理器注册的所有事件。
        /// </summary>
        public void Clear()
        {
            foreach (var unregister in _unregisterActions.Values)
            {
                unregister?.Invoke();
            }
            _unregisterActions.Clear();
        }

        /// <summary>
        /// 当前由本处理器注册的事件名称数量。
        /// </summary>
        public int RegisteredCount => _unregisterActions.Count;

        /// <summary>
        /// 是否已注册指定名称的事件。
        /// </summary>
        public bool HasEvent(string eventName)
        {
            return !string.IsNullOrEmpty(eventName) && _unregisterActions.ContainsKey(eventName);
        }
    }
}
