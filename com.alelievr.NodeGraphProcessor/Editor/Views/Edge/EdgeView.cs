using System;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;

namespace GraphProcessor
{
	public class EdgeView : Edge
	{
		public bool					isConnected = false;

		public SerializableEdge		serializedEdge { get { return userData as SerializableEdge; } }

		readonly string				edgeStyle = "GraphProcessorStyles/EdgeView";

		protected BaseGraphView		owner => ((input ?? output) as PortView).owner.owner;

		readonly Label				executionOrderLabel;

		public EdgeView() : base()
		{
			styleSheets.Add(Resources.Load<StyleSheet>(edgeStyle));
			RegisterCallback<MouseDownEvent>(OnMouseDown);

			executionOrderLabel = new Label { text = "", pickingMode = PickingMode.Ignore };
			executionOrderLabel.AddToClassList("edge-execution-order");
			executionOrderLabel.style.position = Position.Absolute;
			executionOrderLabel.style.width = 20;
			executionOrderLabel.style.height = 20;
			executionOrderLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
			executionOrderLabel.style.fontSize = 10;
			executionOrderLabel.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.9f);
			executionOrderLabel.style.borderBottomLeftRadius = 10;
			executionOrderLabel.style.borderBottomRightRadius = 10;
			executionOrderLabel.style.borderTopLeftRadius = 10;
			executionOrderLabel.style.borderTopRightRadius = 10;
			executionOrderLabel.style.display = DisplayStyle.None;
			Add(executionOrderLabel);

			RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
		}

		void OnGeometryChanged(GeometryChangedEvent evt)
		{
			UpdateExecutionOrderLabelPosition();
		}

		void UpdateExecutionOrderLabelPosition()
		{
			if (executionOrderLabel.style.display == DisplayStyle.None)
				return;

			var midpoint = GetEdgeMidpoint();
			if (midpoint.HasValue)
			{
				executionOrderLabel.style.left = midpoint.Value.x - 10;
				executionOrderLabel.style.top = midpoint.Value.y - 10;
				executionOrderLabel.style.marginLeft = 0;
				executionOrderLabel.style.marginTop = 0;
			}
			else
			{
				executionOrderLabel.style.left = new StyleLength(new Length(50, LengthUnit.Percent));
				executionOrderLabel.style.top = new StyleLength(new Length(50, LengthUnit.Percent));
				executionOrderLabel.style.marginLeft = -10;
				executionOrderLabel.style.marginTop = -10;
			}
		}

		Vector2? GetEdgeMidpoint()
		{
			var edgeType = GetType();
			while (edgeType != null)
			{
				var edgeControlProp = edgeType.GetProperty("edgeControl", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				var edgeControlField = edgeType.GetField("edgeControl", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
					?? edgeType.GetField("m_EdgeControl", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

				var edgeControl = edgeControlProp?.GetValue(this) ?? edgeControlField?.GetValue(this);
				if (edgeControl != null)
				{
					var controlPointsProp = edgeControl.GetType().GetProperty("controlPoints", BindingFlags.Public | BindingFlags.Instance);
					var controlPointsField = edgeControl.GetType().GetField("controlPoints", BindingFlags.Public | BindingFlags.Instance);
					var points = controlPointsProp?.GetValue(edgeControl) as Vector2[] ?? controlPointsField?.GetValue(edgeControl) as Vector2[];

					if (points != null && points.Length >= 2)
					{
						Vector2 midpoint;
						if (points.Length == 4)
						{
							float t = 0.5f;
							float u = 1 - t;
							float uu = u * u, uuu = uu * u;
							float tt = t * t, ttt = tt * t;
							midpoint = uuu * points[0] + 3 * uu * t * points[1] + 3 * u * tt * points[2] + ttt * points[3];
						}
						else
						{
							midpoint = (points[0] + points[points.Length - 1]) * 0.5f;
						}
						return midpoint;
					}
					break;
				}
				edgeType = edgeType.BaseType;
			}
			return null;
		}

		public void UpdateExecutionOrderLabel(int? order)
		{
			if (order.HasValue)
			{
				executionOrderLabel.text = order.Value.ToString();
				executionOrderLabel.style.display = DisplayStyle.Flex;
				executionOrderLabel.tooltip = $"执行顺序: {order.Value} (Y 越小越先执行)";
				schedule.Execute(UpdateExecutionOrderLabelPosition).ExecuteLater(0);
			}
			else
			{
				executionOrderLabel.style.display = DisplayStyle.None;
			}
		}

        public override void OnPortChanged(bool isInput)
		{
			base.OnPortChanged(isInput);
			UpdateEdgeSize();
			schedule.Execute(UpdateExecutionOrderLabelPosition).ExecuteLater(0);
		}

		public void UpdateEdgeSize()
		{
			if (input == null && output == null)
				return;

			PortData inputPortData = (input as PortView)?.portData;
			PortData outputPortData = (output as PortView)?.portData;

			for (int i = 1; i < 20; i++)
				RemoveFromClassList($"edge_{i}");
			int maxPortSize = Mathf.Max(inputPortData?.sizeInPixel ?? 0, outputPortData?.sizeInPixel ?? 0);
			if (maxPortSize > 0)
				AddToClassList($"edge_{Mathf.Max(1, maxPortSize - 6)}");
		}

		protected override void OnCustomStyleResolved(ICustomStyle styles)
		{
			base.OnCustomStyleResolved(styles);

			UpdateEdgeControl();
			schedule.Execute(UpdateExecutionOrderLabelPosition).ExecuteLater(0);
		}

		void OnMouseDown(MouseDownEvent e)
		{
			if (e.clickCount == 2)
			{
				// Empirical offset:
				var position = e.mousePosition;
                position += new Vector2(-10f, -28);
                Vector2 mousePos = owner.ChangeCoordinatesTo(owner.contentViewContainer, position);

				owner.AddRelayNode(input as PortView, output as PortView, mousePos);
			}
		}
	}
}