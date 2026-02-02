using UnityEngine;
using UnityEditor;
using Air.UnityGameCore.Runtime.UI.State;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;

namespace Air.UnityGameCore.Editor.UI
{
    /// <summary>
    /// UI状态控制器自定义编辑器
    /// </summary>
    [CustomEditor(typeof(UIStateCtrl))]
    public class UIStateCtrlEditor : UnityEditor.Editor
    {
        private UIStateCtrl _stateCtrl;
        private SerializedProperty _statesProperty;
        private SerializedProperty _currentStateNameProperty;
        private SerializedProperty _applyOnStartProperty;

        private bool[] _stateFoldouts;
        private string _newStateName = "NewState";

        private void OnEnable()
        {
            _stateCtrl = (UIStateCtrl)target;
            _statesProperty = serializedObject.FindProperty("states");
            _currentStateNameProperty = serializedObject.FindProperty("currentStateName");
            _applyOnStartProperty = serializedObject.FindProperty("applyOnStart");

            if (_stateFoldouts == null || _stateFoldouts.Length != _stateCtrl.States.Count)
            {
                _stateFoldouts = new bool[_stateCtrl.States.Count];
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("UI 状态控制器", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            // 基础设置
            DrawBasicSettings();

            EditorGUILayout.Space(10);

            // 添加新状态
            DrawAddStateSection();

            EditorGUILayout.Space(10);

            // 显示所有状态
            DrawStatesSection();

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 绘制基础设置
        /// </summary>
        private void DrawBasicSettings()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("基础设置", EditorStyles.boldLabel);
            
            EditorGUILayout.PropertyField(_applyOnStartProperty, new GUIContent("启动时应用状态"));
            
            // 当前状态选择
            DrawCurrentStateSelector();
            
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制当前状态选择器
        /// </summary>
        private void DrawCurrentStateSelector()
        {
            if (_stateCtrl.States.Count > 0)
            {
                List<string> stateNames = new List<string> { "(无)" };
                int currentIndex = 0;

                for (int i = 0; i < _stateCtrl.States.Count; i++)
                {
                    var state = _stateCtrl.States[i];
                    if (state != null)
                    {
                        stateNames.Add(state.StateName);
                        if (state.StateName == _currentStateNameProperty.stringValue)
                        {
                            currentIndex = i + 1;
                        }
                    }
                }

                EditorGUI.BeginChangeCheck();
                int newIndex = EditorGUILayout.Popup("当前状态", currentIndex, stateNames.ToArray());
                if (EditorGUI.EndChangeCheck())
                {
                    if (newIndex == 0)
                    {
                        _currentStateNameProperty.stringValue = string.Empty;
                    }
                    else
                    {
                        _currentStateNameProperty.stringValue = stateNames[newIndex];
                    }
                }

                // 预览按钮
                if (currentIndex > 0 && GUILayout.Button("预览当前状态"))
                {
                    _stateCtrl.PreviewState(_currentStateNameProperty.stringValue);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("还没有创建任何状态", MessageType.Info);
            }
        }

        /// <summary>
        /// 绘制添加状态区域
        /// </summary>
        private void DrawAddStateSection()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("添加新状态", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            _newStateName = EditorGUILayout.TextField("状态名称", _newStateName);

            if (GUILayout.Button("添加", GUILayout.Width(60)))
            {
                if (!string.IsNullOrEmpty(_newStateName))
                {
                    Undo.RecordObject(_stateCtrl, "Add State");
                    var newState = _stateCtrl.AddState(_newStateName);
                    if (newState != null)
                    {
                        _newStateName = "NewState";
                        System.Array.Resize(ref _stateFoldouts, _stateCtrl.States.Count);
                        EditorUtility.SetDirty(_stateCtrl);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制状态列表
        /// </summary>
        private void DrawStatesSection()
        {
            EditorGUILayout.LabelField($"状态列表 ({_stateCtrl.States.Count})", EditorStyles.boldLabel);

            if (_stateCtrl.States.Count == 0)
            {
                EditorGUILayout.HelpBox("没有状态。请添加一个新状态。", MessageType.Info);
                return;
            }

            // 确保折叠数组大小正确
            if (_stateFoldouts.Length != _stateCtrl.States.Count)
            {
                System.Array.Resize(ref _stateFoldouts, _stateCtrl.States.Count);
            }

            for (int i = 0; i < _stateCtrl.States.Count; i++)
            {
                var state = _stateCtrl.States[i];
                if (state == null) continue;

                DrawStateItem(state, i);
                EditorGUILayout.Space(5);
            }
        }

        /// <summary>
        /// 绘制单个状态项
        /// </summary>
        private void DrawStateItem(UIState state, int index)
        {
            EditorGUILayout.BeginVertical("box");

            // 状态标题栏
            EditorGUILayout.BeginHorizontal();
            _stateFoldouts[index] = EditorGUILayout.Foldout(_stateFoldouts[index], 
                $"{state.StateName} ({state.Actions.Count} 个动作)", true);

            // 预览按钮
            if (GUILayout.Button("预览", GUILayout.Width(50)))
            {
                _stateCtrl.PreviewState(state.StateName);
            }

            // 删除按钮
            if (GUILayout.Button("删除", GUILayout.Width(50)))
            {
                if (EditorUtility.DisplayDialog("确认删除", 
                    $"确定要删除状态 '{state.StateName}' 吗？", "删除", "取消"))
                {
                    Undo.RecordObject(_stateCtrl, "Remove State");
                    _stateCtrl.RemoveState(state.StateName);
                    System.Array.Resize(ref _stateFoldouts, _stateCtrl.States.Count);
                    EditorUtility.SetDirty(_stateCtrl);
                    return;
                }
            }

            EditorGUILayout.EndHorizontal();

            // 状态详情
            if (_stateFoldouts[index])
            {
                EditorGUI.indentLevel++;
                DrawStateActions(state);
                EditorGUI.indentLevel--;

                EditorGUILayout.Space(5);

                // 添加动作按钮
                DrawAddActionButtons(state);
            }

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制状态动作列表
        /// </summary>
        private void DrawStateActions(UIState state)
        {
            if (state.Actions.Count == 0)
            {
                EditorGUILayout.HelpBox("此状态没有动作。请添加动作。", MessageType.Info);
                return;
            }

            for (int i = 0; i < state.Actions.Count; i++)
            {
                var action = state.Actions[i];
                if (action == null) continue;

                DrawActionItem(state, action, i);
            }
        }

        /// <summary>
        /// 绘制单个动作项
        /// </summary>
        private void DrawActionItem(UIState state, StateAction action, int actionIndex)
        {
            if (action == null) return;

            EditorGUILayout.BeginVertical("helpbox");

            // 标题栏
            EditorGUILayout.BeginHorizontal();
            
            string actionTypeName = action.GetActionTypeName();
            string actionLabel = $"动作 {actionIndex + 1}: {actionTypeName} ({action.GetType().Name})";
            EditorGUILayout.LabelField(actionLabel, EditorStyles.boldLabel);

            // 验证状态指示
            if (!action.IsValid())
            {
                GUILayout.Label(new GUIContent("⚠", "此动作配置无效"), GUILayout.Width(20));
            }

            if (GUILayout.Button("删除", GUILayout.Width(50)))
            {
                Undo.RecordObject(_stateCtrl, "Remove Action");
                state.RemoveActionAt(actionIndex);
                EditorUtility.SetDirty(_stateCtrl);
                return;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel++;

            // 获取SerializedProperty
            var stateIndex = _stateCtrl.States.IndexOf(state);
            var stateProperty = _statesProperty.GetArrayElementAtIndex(stateIndex);
            var actionsProperty = stateProperty.FindPropertyRelative("actions");
            var actionProperty = actionsProperty.GetArrayElementAtIndex(actionIndex);

            // 根据具体类型绘制字段
            DrawActionFields(action, actionProperty);

            EditorGUI.indentLevel--;

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(3);
        }

        /// <summary>
        /// 根据动作类型绘制相应的字段
        /// </summary>
        private void DrawActionFields(StateAction action, SerializedProperty actionProperty)
        {
            // 目标对象（所有动作都有）
            var targetObjectProperty = actionProperty.FindPropertyRelative("targetObject");
            EditorGUILayout.PropertyField(targetObjectProperty, new GUIContent("目标对象"));

            // 根据具体类型绘制特定字段
            switch (action)
            {
                case VisibilityStateAction _:
                    DrawVisibilityActionFields(actionProperty);
                    break;

                case ColorStateAction _:
                    DrawColorActionFields(actionProperty);
                    break;

                case ScaleStateAction _:
                    DrawScaleActionFields(actionProperty);
                    break;

                case PositionStateAction _:
                    DrawPositionActionFields(actionProperty);
                    break;

                case RotationStateAction _:
                    DrawRotationActionFields(actionProperty);
                    break;

                case AlphaStateAction _:
                    DrawAlphaActionFields(actionProperty);
                    break;

                case InteractableStateAction _:
                    DrawInteractableActionFields(actionProperty);
                    break;

                case AnimatorTriggerStateAction _:
                    DrawAnimatorTriggerActionFields(actionProperty);
                    break;
            }
        }

        private void DrawVisibilityActionFields(SerializedProperty actionProperty)
        {
            var isVisibleProperty = actionProperty.FindPropertyRelative("isVisible");
            EditorGUILayout.PropertyField(isVisibleProperty, new GUIContent("是否显示"));
        }

        private void DrawColorActionFields(SerializedProperty actionProperty)
        {
            var targetColorProperty = actionProperty.FindPropertyRelative("targetColor");
            EditorGUILayout.PropertyField(targetColorProperty, new GUIContent("目标颜色"));
        }

        private void DrawScaleActionFields(SerializedProperty actionProperty)
        {
            var targetScaleProperty = actionProperty.FindPropertyRelative("targetScale");
            EditorGUILayout.PropertyField(targetScaleProperty, new GUIContent("目标缩放"));
        }

        private void DrawPositionActionFields(SerializedProperty actionProperty)
        {
            var anchoredPositionProperty = actionProperty.FindPropertyRelative("anchoredPosition");
            EditorGUILayout.PropertyField(anchoredPositionProperty, new GUIContent("锚点位置"));
        }

        private void DrawRotationActionFields(SerializedProperty actionProperty)
        {
            var targetRotationProperty = actionProperty.FindPropertyRelative("targetRotation");
            EditorGUILayout.PropertyField(targetRotationProperty, new GUIContent("目标旋转"));
        }

        private void DrawAlphaActionFields(SerializedProperty actionProperty)
        {
            var targetAlphaProperty = actionProperty.FindPropertyRelative("targetAlpha");
            EditorGUILayout.Slider(targetAlphaProperty, 0f, 1f, new GUIContent("目标透明度"));
        }

        private void DrawInteractableActionFields(SerializedProperty actionProperty)
        {
            var interactableProperty = actionProperty.FindPropertyRelative("interactable");
            var blockRaycastsProperty = actionProperty.FindPropertyRelative("blockRaycasts");
            
            EditorGUILayout.PropertyField(interactableProperty, new GUIContent("可交互"));
            EditorGUILayout.PropertyField(blockRaycastsProperty, new GUIContent("阻挡射线"));
        }

        private void DrawAnimatorTriggerActionFields(SerializedProperty actionProperty)
        {
            var triggerNameProperty = actionProperty.FindPropertyRelative("triggerName");
            EditorGUILayout.PropertyField(triggerNameProperty, new GUIContent("触发器名称"));
        }

        /// <summary>
        /// 绘制添加动作按钮
        /// </summary>
        private void DrawAddActionButtons(UIState state)
        {
            EditorGUILayout.BeginVertical();
            
            EditorGUILayout.LabelField("添加动作类型", EditorStyles.boldLabel);
            
            // 第一行：基础动作
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+ 显影", GUILayout.Height(25)))
            {
                AddAction<VisibilityStateAction>(state, "Add Visibility Action");
            }
            if (GUILayout.Button("+ 颜色", GUILayout.Height(25)))
            {
                AddAction<ColorStateAction>(state, "Add Color Action");
            }
            if (GUILayout.Button("+ 透明度", GUILayout.Height(25)))
            {
                AddAction<AlphaStateAction>(state, "Add Alpha Action");
            }
            EditorGUILayout.EndHorizontal();

            // 第二行：变换动作
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+ 缩放", GUILayout.Height(25)))
            {
                AddAction<ScaleStateAction>(state, "Add Scale Action");
            }
            if (GUILayout.Button("+ 位置", GUILayout.Height(25)))
            {
                AddAction<PositionStateAction>(state, "Add Position Action");
            }
            if (GUILayout.Button("+ 旋转", GUILayout.Height(25)))
            {
                AddAction<RotationStateAction>(state, "Add Rotation Action");
            }
            EditorGUILayout.EndHorizontal();

            // 第三行：交互和动画
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+ 交互", GUILayout.Height(25)))
            {
                AddAction<InteractableStateAction>(state, "Add Interactable Action");
            }
            if (GUILayout.Button("+ 动画触发", GUILayout.Height(25)))
            {
                AddAction<AnimatorTriggerStateAction>(state, "Add Animator Action");
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 添加指定类型的动作
        /// </summary>
        private void AddAction<T>(UIState state, string undoName) where T : StateAction, new()
        {
            Undo.RecordObject(_stateCtrl, undoName);
            var action = new T();
            state.AddAction(action);
            EditorUtility.SetDirty(_stateCtrl);
        }
    }
}
