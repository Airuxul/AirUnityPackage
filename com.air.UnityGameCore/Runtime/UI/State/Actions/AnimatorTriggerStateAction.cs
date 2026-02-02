using System;
using UnityEngine;

namespace Air.UnityGameCore.Runtime.UI.State
{
    /// <summary>
    /// 动画触发状态动作
    /// 触发Animator的动画状态
    /// </summary>
    [Serializable]
    public class AnimatorTriggerStateAction : StateAction
    {
        [SerializeField] private string triggerName;

        public string TriggerName
        {
            get => triggerName;
            set => triggerName = value;
        }

        public AnimatorTriggerStateAction()
        {
        }

        public AnimatorTriggerStateAction(GameObject target, string trigger)
        {
            targetObject = target;
            triggerName = trigger;
        }

        public override void Apply()
        {
            if (targetObject == null)
            {
                Debug.LogWarning("[AnimatorTriggerStateAction] 目标对象为空，无法触发动画");
                return;
            }

            if (string.IsNullOrEmpty(triggerName))
            {
                Debug.LogWarning("[AnimatorTriggerStateAction] 触发器名称为空");
                return;
            }

            var animator = targetObject.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger(triggerName);
            }
            else
            {
                Debug.LogWarning($"[AnimatorTriggerStateAction] 目标对象 {targetObject.name} 上没有找到Animator组件");
            }
        }

        public override string GetActionTypeName()
        {
            return "动画触发";
        }

        public override bool IsValid()
        {
            if (!base.IsValid()) return false;
            return targetObject.GetComponent<Animator>() != null && !string.IsNullOrEmpty(triggerName);
        }
    }
}
