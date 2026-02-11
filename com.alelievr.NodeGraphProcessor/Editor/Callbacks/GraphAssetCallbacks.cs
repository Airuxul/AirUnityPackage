using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace GraphProcessor
{
	public static class GraphAssetCallbacks
	{
		[MenuItem("Assets/Create/GraphProcessor", false, 10)]
		public static void CreateGraphPorcessor()
		{
			var graph = ScriptableObject.CreateInstance<BaseGraph>();
			ProjectWindowUtil.CreateAsset(graph, "GraphProcessor.asset");
		}

		[OnOpenAsset(0)]
		public static bool OnBaseGraphOpened(int instanceID, int line)
		{
			var asset = EditorUtility.InstanceIDToObject(instanceID) as BaseGraph;

			if (asset != null)
			{
				EditorWindow.GetWindow<GraphEditWindow>().InitializeGraph(asset);
				return true;
			}
			return false;
		}
	}

}
