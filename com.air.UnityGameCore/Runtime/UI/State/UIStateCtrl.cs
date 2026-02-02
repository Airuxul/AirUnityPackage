using System;
using System.Collections.Generic;
using UnityEngine;

namespace Air.UnityGameCore.Runtime.UI.State
{
    /// <summary>
    /// UI状态数据
    /// </summary>
    [Serializable]
    public class UIState
    {
        [SerializeField] private string stateName;
        
        // 使用SerializeReference支持多态序列化
        [SerializeReference] private List<StateAction> actions = new();

        public string StateName => stateName;
        public List<StateAction> Actions => actions;

        public UIState(string name)
        {
            stateName = name;
        }

        /// <summary>
        /// 应用当前状态
        /// </summary>
        public void Apply()
        {
            foreach (var action in actions)
            {
                if (action != null && action.IsValid())
                {
                    try
                    {
                        action.Apply();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[UIState] 应用动作失败: {e.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// 添加动作
        /// </summary>
        public void AddAction(StateAction action)
        {
            if (action != null && !actions.Contains(action))
            {
                actions.Add(action);
            }
        }

        /// <summary>
        /// 移除动作
        /// </summary>
        public void RemoveAction(StateAction action)
        {
            actions.Remove(action);
        }

        /// <summary>
        /// 移除指定索引的动作
        /// </summary>
        public void RemoveActionAt(int index)
        {
            if (index >= 0 && index < actions.Count)
            {
                actions.RemoveAt(index);
            }
        }

        /// <summary>
        /// 获取所有有效的动作
        /// </summary>
        public List<StateAction> GetValidActions()
        {
            return actions.FindAll(a => a != null && a.IsValid());
        }
    }

    /// <summary>
    /// UI状态控制器
    /// 支持在编辑器中预设多个状态，并在运行时切换
    /// </summary>
    [RequireComponent(typeof(UIComponent))]
    public class UIStateCtrl : MonoBehaviour
    {
        [SerializeField] private List<UIState> states = new List<UIState>();
        [SerializeField] private string currentStateName;
        [SerializeField] private bool applyOnStart = true;

        private UIState currentState;
        private Dictionary<string, UIState> stateDict;

        /// <summary>
        /// 当前状态名称
        /// </summary>
        public string CurrentStateName => currentStateName;

        /// <summary>
        /// 所有状态列表
        /// </summary>
        public List<UIState> States => states;

        private void Awake()
        {
            InitializeStates();
        }

        private void Start()
        {
            if (applyOnStart && !string.IsNullOrEmpty(currentStateName))
            {
                SetState(currentStateName);
            }
        }

        /// <summary>
        /// 初始化状态字典
        /// </summary>
        private void InitializeStates()
        {
            stateDict = new Dictionary<string, UIState>();
            foreach (var state in states)
            {
                if (state != null && !string.IsNullOrEmpty(state.StateName))
                {
                    if (!stateDict.ContainsKey(state.StateName))
                    {
                        stateDict[state.StateName] = state;
                    }
                    else
                    {
                        Debug.LogWarning($"[UIStateCtrl] 状态名称重复: {state.StateName}");
                    }
                }
            }
        }

        /// <summary>
        /// 设置状态
        /// </summary>
        /// <param name="stateName">状态名称</param>
        /// <returns>是否设置成功</returns>
        public bool SetState(string stateName)
        {
            if (string.IsNullOrEmpty(stateName))
            {
                Debug.LogWarning("[UIStateCtrl] 状态名称不能为空");
                return false;
            }

            if (stateDict == null)
            {
                InitializeStates();
            }

            if (stateDict.TryGetValue(stateName, out var state))
            {
                currentState = state;
                currentStateName = stateName;
                state.Apply();
                return true;
            }
            else
            {
                Debug.LogWarning($"[UIStateCtrl] 未找到状态: {stateName}");
                return false;
            }
        }

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <param name="stateName">状态名称</param>
        /// <returns>状态对象</returns>
        public UIState GetState(string stateName)
        {
            if (stateDict == null)
            {
                InitializeStates();
            }

            stateDict.TryGetValue(stateName, out var state);
            return state;
        }

        /// <summary>
        /// 添加新状态
        /// </summary>
        /// <param name="stateName">状态名称</param>
        /// <returns>新创建的状态</returns>
        public UIState AddState(string stateName)
        {
            if (string.IsNullOrEmpty(stateName))
            {
                Debug.LogWarning("[UIStateCtrl] 状态名称不能为空");
                return null;
            }

            if (stateDict == null)
            {
                InitializeStates();
            }

            if (stateDict.ContainsKey(stateName))
            {
                Debug.LogWarning($"[UIStateCtrl] 状态已存在: {stateName}");
                return stateDict[stateName];
            }

            var newState = new UIState(stateName);
            states.Add(newState);
            stateDict[stateName] = newState;
            return newState;
        }

        /// <summary>
        /// 移除状态
        /// </summary>
        /// <param name="stateName">状态名称</param>
        public void RemoveState(string stateName)
        {
            var state = GetState(stateName);
            if (state != null)
            {
                states.Remove(state);
                stateDict.Remove(stateName);

                if (currentStateName == stateName)
                {
                    currentStateName = string.Empty;
                    currentState = null;
                }
            }
        }

        /// <summary>
        /// 刷新当前状态
        /// </summary>
        public void RefreshCurrentState()
        {
            if (currentState != null)
            {
                currentState.Apply();
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// 编辑器预览状态
        /// </summary>
        public void PreviewState(string stateName)
        {
            var state = GetState(stateName);
            state?.Apply();
        }
#endif
    }
}