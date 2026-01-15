using System.IO;
using UnityEditor;
using UnityEngine;
using AirUI.Core;

namespace AirUI.Editor
{
    /// <summary>
    /// UI脚本生成器窗口
    /// </summary>
    public class UIGeneratorWindow : EditorWindow
    {
        private GameObject targetGameObject;
        private string className = "";
        private string outputFolder = "Assets/Scripts/Generated";
        
        [MenuItem("Window/AirUI/UI Script Generator")]
        public static void ShowWindow()
        {
            GetWindow<UIGeneratorWindow>("UI Script Generator");
        }
        
        /// <summary>
        /// 设置目标GameObject（用于从右键菜单打开时预填充）
        /// </summary>
        /// <param name="gameObject">目标GameObject</param>
        public static void SetTargetGameObject(GameObject gameObject)
        {
            var window = GetWindow<UIGeneratorWindow>("UI Script Generator");
            window.targetGameObject = gameObject;
            window.className = gameObject.name;
            window.Focus();
        }
        
        private void OnEnable()
        {
            minSize = new Vector2(400, 300);
        }
        
        private void OnGUI()
        {
            DrawHeader();
            DrawMainSettings();
            DrawGenerateButton();
        }
        
        private void DrawHeader()
        {
            EditorGUILayout.Space(10);
            
            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
            titleStyle.fontSize = 18;
            titleStyle.alignment = TextAnchor.MiddleCenter;
            EditorGUILayout.LabelField("AirUI Script Generator", titleStyle);
            
            EditorGUILayout.Space(5);
            
            EditorGUILayout.HelpBox(
                "拖拽GameObject到下方区域，设置类名和输出文件夹，然后生成UI脚本",
                MessageType.Info);
            
            EditorGUILayout.Space(10);
        }
        
        private void DrawMainSettings()
        {
            EditorGUILayout.LabelField("设置", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            
            // GameObject拖拽区域
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("目标GameObject", EditorStyles.boldLabel);
            
            if (targetGameObject == null)
            {
                EditorGUILayout.HelpBox("请拖拽一个GameObject到下方区域", MessageType.Info);
            }
            else
            {
                EditorGUILayout.ObjectField("当前选择:", targetGameObject, typeof(GameObject), false);
                if (GUILayout.Button("清除选择", GUILayout.Height(25)))
                {
                    targetGameObject = null;
                    className = "";
                }
            }
            
            // 拖拽区域
            var dropArea = GUILayoutUtility.GetRect(0, 60, GUILayout.ExpandWidth(true));
            GUI.Box(dropArea, "拖拽GameObject到这里");
            
            // 处理拖拽
            HandleDragAndDrop(dropArea);
            
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(10);
            
            // 类名设置
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("脚本设置", EditorStyles.boldLabel);
            
            className = EditorGUILayout.TextField("类名", className);
            
            if (string.IsNullOrEmpty(className))
            {
                EditorGUILayout.HelpBox("类名不能为空", MessageType.Warning);
            }
            
            // 输出文件夹
            EditorGUILayout.BeginHorizontal();
            outputFolder = EditorGUILayout.TextField("输出文件夹", outputFolder);
            if (GUILayout.Button("选择", GUILayout.Width(60)))
            {
                string selectedPath = EditorUtility.OpenFolderPanel("选择输出文件夹", "Assets", "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    // 转换为相对路径
                    if (selectedPath.StartsWith(Application.dataPath))
                    {
                        outputFolder = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                    }
                    else
                    {
                        outputFolder = selectedPath;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            
            // 创建文件夹按钮
            if (!Directory.Exists(outputFolder))
            {
                if (GUILayout.Button("创建输出文件夹", GUILayout.Height(25)))
                {
                    Directory.CreateDirectory(outputFolder);
                    AssetDatabase.Refresh();
                }
            }
            
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(10);
        }
        
        private void DrawGenerateButton()
        {
            EditorGUILayout.Space(20);
            
            // 生成按钮
            GUI.enabled = CanGenerateScript();
            if (GUILayout.Button("生成UI脚本", GUILayout.Height(40)))
            {
                GenerateScript();
            }
            GUI.enabled = true;
        }
        
        private void HandleDragAndDrop(Rect dropArea)
        {
            Event currentEvent = Event.current;
            
            switch (currentEvent.type)
            {
                case EventType.DragUpdated:
                    if (dropArea.Contains(currentEvent.mousePosition))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        currentEvent.Use();
                    }
                    break;
                    
                case EventType.DragPerform:
                    if (dropArea.Contains(currentEvent.mousePosition))
                    {
                        DragAndDrop.AcceptDrag();
                        
                        foreach (Object draggedObject in DragAndDrop.objectReferences)
                        {
                            if (draggedObject is GameObject go)
                            {
                                targetGameObject = go;
                                if (string.IsNullOrEmpty(className))
                                {
                                    className = go.name;
                                }
                                break;
                            }
                        }
                        
                        currentEvent.Use();
                    }
                    break;
            }
        }
        
        private bool CanGenerateScript()
        {
            return targetGameObject != null && 
                   !string.IsNullOrEmpty(className) && 
                   !string.IsNullOrEmpty(outputFolder);
        }
        
        private void GenerateScript()
        {
            if (!CanGenerateScript())
            {
                EditorUtility.DisplayDialog("错误", "请检查所有必填项", "确定");
                return;
            }
            
            try
            {
                // 使用新的接口生成脚本
                UIScriptGenerator.GenerateUIScript(targetGameObject, className, outputFolder);
                
                // 刷新AssetDatabase
                AssetDatabase.Refresh();
                
                // 选中生成的脚本文件
                string logicScriptPath = Path.Combine(outputFolder, className + ".cs");
                if (File.Exists(logicScriptPath))
                {
                    Object scriptAsset = AssetDatabase.LoadAssetAtPath<Object>(logicScriptPath);
                    if (scriptAsset != null)
                    {
                        Selection.activeObject = scriptAsset;
                        EditorGUIUtility.PingObject(scriptAsset);
                    }
                }
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog("Error", $"生成脚本时发生错误:\n{e.Message}", "确定");
                Debug.LogError($"UI脚本生成失败: {e}");
            }
        }
    }
}
