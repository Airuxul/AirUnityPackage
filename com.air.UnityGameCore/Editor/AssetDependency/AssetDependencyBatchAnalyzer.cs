using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Air.UnityGameCore.Editor.AssetDependency
{
    /// <summary>
    /// 资源依赖批量分析工具
    /// 提供批量分析、统计和报告生成功能
    /// </summary>
    public class AssetDependencyBatchAnalyzer : EditorWindow
    {
        private List<Object> _targetAssets = new List<Object>();
        private Vector2 _scrollPosition;
        private Vector2 _resultScrollPosition;
        
        private bool _analyzing = false;
        private List<AssetAnalysisResult> _analysisResults = new List<AssetAnalysisResult>();
        
        private SortMode _sortMode = SortMode.Name;
        private bool _showDetails = true;

        private enum SortMode
        {
            Name,
            DependencyCount,
            ReverseDependencyCount,
            FileSize
        }

        [MenuItem("Tools/资源依赖批量分析")]
        public static void ShowWindow()
        {
            AssetDependencyBatchAnalyzer window = GetWindow<AssetDependencyBatchAnalyzer>();
            window.titleContent = new GUIContent("批量依赖分析");
            window.minSize = new Vector2(700, 500);
        }

        private void OnGUI()
        {
            DrawHeader();
            DrawAssetList();
            DrawControls();
            DrawResults();
        }

        /// <summary>
        /// 绘制标题
        /// </summary>
        private void DrawHeader()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 16
            };
            EditorGUILayout.LabelField("资源依赖批量分析工具", titleStyle);
            
            EditorGUILayout.LabelField("批量分析多个资源的依赖关系，生成统计报告", EditorStyles.miniLabel);
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        /// <summary>
        /// 绘制资源列表
        /// </summary>
        private void DrawAssetList()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            EditorGUILayout.LabelField($"待分析资源列表 ({_targetAssets.Count})", EditorStyles.boldLabel);
            
            EditorGUILayout.Space(3);
            
            // 拖放区域
            Rect dropArea = GUILayoutUtility.GetRect(0, 50, GUILayout.ExpandWidth(true));
            GUI.Box(dropArea, "拖拽资源到此处添加\n或从Project窗口选中后点击\"添加选中资源\"", EditorStyles.helpBox);
            
            HandleDragAndDrop(dropArea);
            
            EditorGUILayout.Space(3);
            
            // 快捷按钮
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("添加选中资源", GUILayout.Height(25)))
            {
                AddSelectedAssets();
            }
            
            if (GUILayout.Button("清空列表", GUILayout.Height(25)))
            {
                _targetAssets.Clear();
            }
            
            if (GUILayout.Button("添加文件夹中所有资源", GUILayout.Height(25)))
            {
                AddAssetsFromFolder();
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(3);
            
            // 资源列表
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(150));
            
            for (int i = _targetAssets.Count - 1; i >= 0; i--)
            {
                if (_targetAssets[i] == null)
                {
                    _targetAssets.RemoveAt(i);
                    continue;
                }
                
                EditorGUILayout.BeginHorizontal();
                
                EditorGUILayout.ObjectField(_targetAssets[i], typeof(Object), false, GUILayout.ExpandWidth(true));
                
                if (GUILayout.Button("×", GUILayout.Width(25)))
                {
                    _targetAssets.RemoveAt(i);
                }
                
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        /// <summary>
        /// 绘制控制面板
        /// </summary>
        private void DrawControls()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            EditorGUILayout.LabelField("分析选项", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            
            _showDetails = EditorGUILayout.ToggleLeft("显示详细信息", _showDetails, GUILayout.Width(120));
            
            EditorGUILayout.LabelField("排序方式:", GUILayout.Width(70));
            _sortMode = (SortMode)EditorGUILayout.EnumPopup(_sortMode, GUILayout.Width(150));
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(5);
            
            EditorGUILayout.BeginHorizontal();
            
            GUI.enabled = !_analyzing && _targetAssets.Count > 0;
            if (GUILayout.Button("开始批量分析", GUILayout.Height(30)))
            {
                StartBatchAnalysis();
            }
            GUI.enabled = true;
            
            GUI.enabled = _analysisResults.Count > 0;
            if (GUILayout.Button("导出CSV报告", GUILayout.Height(30)))
            {
                ExportToCSV();
            }
            
            if (GUILayout.Button("导出详细报告", GUILayout.Height(30)))
            {
                ExportDetailedReport();
            }
            GUI.enabled = true;
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        /// <summary>
        /// 绘制结果
        /// </summary>
        private void DrawResults()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            EditorGUILayout.LabelField($"分析结果 ({_analysisResults.Count})", EditorStyles.boldLabel);
            
            if (_analysisResults.Count == 0)
            {
                EditorGUILayout.HelpBox("点击\"开始批量分析\"按钮开始分析", MessageType.Info);
            }
            else
            {
                DrawResultsTable();
            }
            
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制结果表格
        /// </summary>
        private void DrawResultsTable()
        {
            // 排序结果
            SortResults();
            
            // 统计摘要
            DrawSummary();
            
            EditorGUILayout.Space(3);
            
            // 表头
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUILayout.LabelField("资源名称", EditorStyles.boldLabel, GUILayout.Width(200));
            EditorGUILayout.LabelField("依赖数", EditorStyles.boldLabel, GUILayout.Width(60));
            EditorGUILayout.LabelField("被依赖数", EditorStyles.boldLabel, GUILayout.Width(80));
            EditorGUILayout.LabelField("文件大小", EditorStyles.boldLabel, GUILayout.Width(80));
            EditorGUILayout.LabelField("类型", EditorStyles.boldLabel, GUILayout.Width(80));
            EditorGUILayout.LabelField("操作", EditorStyles.boldLabel, GUILayout.Width(80));
            EditorGUILayout.EndHorizontal();
            
            // 结果列表
            _resultScrollPosition = EditorGUILayout.BeginScrollView(_resultScrollPosition);
            
            foreach (var result in _analysisResults)
            {
                DrawResultRow(result);
            }
            
            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// 绘制摘要
        /// </summary>
        private void DrawSummary()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            int totalDeps = _analysisResults.Sum(r => r.DependencyCount);
            int totalReverseDeps = _analysisResults.Sum(r => r.ReverseDependencyCount);
            long totalSize = _analysisResults.Sum(r => r.FileSize);
            
            EditorGUILayout.LabelField("统计摘要:", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"总资源数: {_analysisResults.Count} | " +
                                      $"总依赖数: {totalDeps} | " +
                                      $"总被依赖数: {totalReverseDeps} | " +
                                      $"总大小: {AssetDependencyAnalyzer.FormatFileSize(totalSize)}");
            
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制结果行
        /// </summary>
        private void DrawResultRow(AssetAnalysisResult result)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            
            // 资源名称
            EditorGUILayout.LabelField(result.AssetName, GUILayout.Width(200));
            
            // 依赖数
            EditorGUILayout.LabelField(result.DependencyCount.ToString(), GUILayout.Width(60));
            
            // 被依赖数
            Color oldColor = GUI.color;
            if (result.ReverseDependencyCount == 0)
            {
                GUI.color = Color.yellow;
            }
            EditorGUILayout.LabelField(result.ReverseDependencyCount.ToString(), GUILayout.Width(80));
            GUI.color = oldColor;
            
            // 文件大小
            EditorGUILayout.LabelField(AssetDependencyAnalyzer.FormatFileSize(result.FileSize), GUILayout.Width(80));
            
            // 类型
            EditorGUILayout.LabelField(result.AssetType, GUILayout.Width(80));
            
            // 操作按钮
            if (GUILayout.Button("选择", GUILayout.Width(40)))
            {
                Object asset = AssetDatabase.LoadAssetAtPath<Object>(result.AssetPath);
                if (asset != null)
                {
                    Selection.activeObject = asset;
                    EditorGUIUtility.PingObject(asset);
                }
            }
            
            EditorGUILayout.EndHorizontal();
            
            // 详细信息
            if (_showDetails && result.IsExpanded)
            {
                DrawResultDetails(result);
            }
        }

        /// <summary>
        /// 绘制结果详情
        /// </summary>
        private void DrawResultDetails(AssetAnalysisResult result)
        {
            EditorGUI.indentLevel++;
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField($"路径: {result.AssetPath}", EditorStyles.wordWrappedMiniLabel);
            
            if (result.Dependencies.Count > 0)
            {
                EditorGUILayout.LabelField($"主要依赖 (前5个):", EditorStyles.miniLabel);
                for (int i = 0; i < Mathf.Min(5, result.Dependencies.Count); i++)
                {
                    EditorGUILayout.LabelField($"  • {result.Dependencies[i]}", EditorStyles.miniLabel);
                }
            }
            
            EditorGUILayout.EndVertical();
            
            EditorGUI.indentLevel--;
        }

        /// <summary>
        /// 处理拖放
        /// </summary>
        private void HandleDragAndDrop(Rect dropArea)
        {
            Event evt = Event.current;
            
            if (dropArea.Contains(evt.mousePosition))
            {
                if (evt.type == EventType.DragUpdated)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    evt.Use();
                }
                else if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    
                    foreach (Object draggedObject in DragAndDrop.objectReferences)
                    {
                        if (!_targetAssets.Contains(draggedObject))
                        {
                            _targetAssets.Add(draggedObject);
                        }
                    }
                    
                    evt.Use();
                }
            }
        }

        /// <summary>
        /// 添加选中的资源
        /// </summary>
        private void AddSelectedAssets()
        {
            foreach (Object obj in Selection.objects)
            {
                if (!_targetAssets.Contains(obj))
                {
                    _targetAssets.Add(obj);
                }
            }
        }

        /// <summary>
        /// 从文件夹添加资源
        /// </summary>
        private void AddAssetsFromFolder()
        {
            string folderPath = EditorUtility.OpenFolderPanel("选择文件夹", "Assets", "");
            if (string.IsNullOrEmpty(folderPath))
                return;
            
            if (!folderPath.StartsWith(Application.dataPath))
            {
                EditorUtility.DisplayDialog("错误", "请选择项目Assets文件夹下的目录", "确定");
                return;
            }
            
            string relativePath = "Assets" + folderPath.Substring(Application.dataPath.Length);
            string[] guids = AssetDatabase.FindAssets("", new[] { relativePath });
            
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                Object asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                
                if (asset != null && !_targetAssets.Contains(asset))
                {
                    _targetAssets.Add(asset);
                }
            }
        }

        /// <summary>
        /// 开始批量分析
        /// </summary>
        private void StartBatchAnalysis()
        {
            _analysisResults.Clear();
            _analyzing = true;
            
            try
            {
                for (int i = 0; i < _targetAssets.Count; i++)
                {
                    Object asset = _targetAssets[i];
                    string assetPath = AssetDatabase.GetAssetPath(asset);
                    
                    float progress = (float)i / _targetAssets.Count;
                    EditorUtility.DisplayProgressBar("批量分析中", 
                        $"正在分析: {asset.name} ({i + 1}/{_targetAssets.Count})", progress);
                    
                    AssetAnalysisResult result = AnalyzeAsset(assetPath);
                    _analysisResults.Add(result);
                }
                
                EditorUtility.DisplayDialog("完成", $"批量分析完成！共分析 {_analysisResults.Count} 个资源", "确定");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                _analyzing = false;
            }
        }

        /// <summary>
        /// 分析单个资源
        /// </summary>
        private AssetAnalysisResult AnalyzeAsset(string assetPath)
        {
            AssetInfo info = AssetDependencyAnalyzer.GetAssetInfo(assetPath);
            List<string> deps = AssetDependencyAnalyzer.GetDependencies(assetPath, true);
            List<string> reverseDeps = AssetDependencyAnalyzer.GetReverseDependencies(assetPath);
            
            return new AssetAnalysisResult
            {
                AssetPath = assetPath,
                AssetName = info.AssetName,
                AssetType = info.AssetTypeName,
                FileSize = info.FileSize,
                DependencyCount = deps.Count,
                ReverseDependencyCount = reverseDeps.Count,
                Dependencies = deps,
                ReverseDependencies = reverseDeps
            };
        }

        /// <summary>
        /// 排序结果
        /// </summary>
        private void SortResults()
        {
            switch (_sortMode)
            {
                case SortMode.Name:
                    _analysisResults = _analysisResults.OrderBy(r => r.AssetName).ToList();
                    break;
                case SortMode.DependencyCount:
                    _analysisResults = _analysisResults.OrderByDescending(r => r.DependencyCount).ToList();
                    break;
                case SortMode.ReverseDependencyCount:
                    _analysisResults = _analysisResults.OrderByDescending(r => r.ReverseDependencyCount).ToList();
                    break;
                case SortMode.FileSize:
                    _analysisResults = _analysisResults.OrderByDescending(r => r.FileSize).ToList();
                    break;
            }
        }

        /// <summary>
        /// 导出为CSV
        /// </summary>
        private void ExportToCSV()
        {
            string path = EditorUtility.SaveFilePanel("导出CSV报告", "", "dependency_analysis.csv", "csv");
            if (string.IsNullOrEmpty(path))
                return;
            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("资源名称,资源路径,类型,文件大小(字节),依赖数,被依赖数");
            
            foreach (var result in _analysisResults)
            {
                sb.AppendLine($"\"{result.AssetName}\",\"{result.AssetPath}\",\"{result.AssetType}\"," +
                            $"{result.FileSize},{result.DependencyCount},{result.ReverseDependencyCount}");
            }
            
            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
            
            EditorUtility.RevealInFinder(path);
            Debug.Log($"CSV报告已导出到: {path}");
        }

        /// <summary>
        /// 导出详细报告
        /// </summary>
        private void ExportDetailedReport()
        {
            string path = EditorUtility.SaveFilePanel("导出详细报告", "", "dependency_analysis_detailed.txt", "txt");
            if (string.IsNullOrEmpty(path))
                return;
            
            using (StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8))
            {
                writer.WriteLine("======================================");
                writer.WriteLine("资源依赖批量分析详细报告");
                writer.WriteLine($"生成时间: {System.DateTime.Now}");
                writer.WriteLine($"分析资源数: {_analysisResults.Count}");
                writer.WriteLine("======================================");
                writer.WriteLine();
                
                foreach (var result in _analysisResults)
                {
                    writer.WriteLine($"[{result.AssetName}]");
                    writer.WriteLine($"路径: {result.AssetPath}");
                    writer.WriteLine($"类型: {result.AssetType}");
                    writer.WriteLine($"大小: {AssetDependencyAnalyzer.FormatFileSize(result.FileSize)}");
                    writer.WriteLine($"依赖数: {result.DependencyCount}");
                    writer.WriteLine($"被依赖数: {result.ReverseDependencyCount}");
                    
                    if (result.Dependencies.Count > 0)
                    {
                        writer.WriteLine("\n依赖项:");
                        foreach (string dep in result.Dependencies)
                        {
                            writer.WriteLine($"  - {dep}");
                        }
                    }
                    
                    if (result.ReverseDependencies.Count > 0)
                    {
                        writer.WriteLine("\n被依赖项:");
                        foreach (string revDep in result.ReverseDependencies)
                        {
                            writer.WriteLine($"  - {revDep}");
                        }
                    }
                    
                    writer.WriteLine();
                    writer.WriteLine("--------------------------------------");
                    writer.WriteLine();
                }
            }
            
            EditorUtility.RevealInFinder(path);
            Debug.Log($"详细报告已导出到: {path}");
        }

        /// <summary>
        /// 资源分析结果
        /// </summary>
        private class AssetAnalysisResult
        {
            public string AssetPath { get; set; }
            public string AssetName { get; set; }
            public string AssetType { get; set; }
            public long FileSize { get; set; }
            public int DependencyCount { get; set; }
            public int ReverseDependencyCount { get; set; }
            public List<string> Dependencies { get; set; }
            public List<string> ReverseDependencies { get; set; }
            public bool IsExpanded { get; set; }
        }
    }
}


