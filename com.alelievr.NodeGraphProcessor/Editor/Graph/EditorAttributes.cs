using System;

namespace GraphProcessor
{
	/// <summary>
	/// Tell that this field is will generate an input port
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class InputAttribute : Attribute
	{
		public string		name;
		public bool			allowMultiple;

		/// <summary>
		/// Mark the field as an input port
		/// </summary>
		/// <param name="name">display name</param>
		/// <param name="allowMultiple">is connecting multiple edges allowed</param>
		public InputAttribute(string name = null, bool allowMultiple = false)
		{
			this.name = name;
			this.allowMultiple = allowMultiple;
		}
	}

	/// <summary>
	/// Tell that this field is will generate an output port
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class OutputAttribute : Attribute
	{
		public string		name;
		public bool			allowMultiple;

		/// <summary>
		/// Mark the field as an output port
		/// </summary>
		/// <param name="name">display name</param>
		/// <param name="allowMultiple">is connecting multiple edges allowed</param>
		public OutputAttribute(string name = null, bool allowMultiple = true)
		{
			this.name = name;
			this.allowMultiple = allowMultiple;
		}
	}

	/// <summary>
	/// Creates a vertical port instead of the default horizontal one
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class VerticalAttribute : Attribute
	{
	}

	/// <summary>
	/// Register the node in the NodeProvider class. The node will also be available in the node creation window.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class NodeMenuItemAttribute : Attribute
	{
		public string	menuTitle;
		public Type		onlyCompatibleWithGraph;

		/// <summary>
		/// Register the node in the NodeProvider class. The node will also be available in the node creation window.
		/// </summary>
		/// <param name="menuTitle">Path in the menu, use / as folder separators</param>
		/// <param name="onlyCompatibleWithGraph"></param>
		public NodeMenuItemAttribute(string menuTitle = null, Type onlyCompatibleWithGraph = null)
		{
			this.menuTitle = menuTitle;
			this.onlyCompatibleWithGraph = onlyCompatibleWithGraph;
		}
	}

	/// <summary>
	/// Set a custom drawer for a field. It can then be created using the FieldFactory
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	[Obsolete("You can use the standard Unity CustomPropertyDrawer instead.")]
	public class FieldDrawerAttribute : Attribute
	{
		public Type		fieldType;

		/// <summary>
		/// Register a custom view for a type in the FieldFactory class
		/// </summary>
		/// <param name="fieldType"></param>
		public FieldDrawerAttribute(Type fieldType)
		{
			this.fieldType = fieldType;
		}
	}

	/// <summary>
	/// Allow you to customize the input function of a port
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class CustomPortInputAttribute : Attribute
	{
		public string	fieldName;
		public Type		inputType;
		public bool		allowCast;

		/// <summary>
		/// Allow you to customize the input function of a port.
		/// See CustomPortsNode example in Samples.
		/// </summary>
		/// <param name="fieldName">local field of the node</param>
		/// <param name="inputType">type of input of the port</param>
		/// <param name="allowCast">if cast is allowed when connecting an edge</param>
		public CustomPortInputAttribute(string fieldName, Type inputType, bool allowCast = true)
		{
			this.fieldName = fieldName;
			this.inputType = inputType;
			this.allowCast = allowCast;
		}
	}

	/// <summary>
	/// Allow you to customize the input function of a port
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class CustomPortOutputAttribute : Attribute
	{
		public string	fieldName;
		public Type		outputType;
		public bool		allowCast;

		/// <summary>
		/// Allow you to customize the output function of a port.
		/// See CustomPortsNode example in Samples.
		/// </summary>
		/// <param name="fieldName">local field of the node</param>
		/// <param name="outputType">type of output of the port</param>
		/// <param name="allowCast">if cast is allowed when connecting an edge</param>
		public CustomPortOutputAttribute(string fieldName, Type outputType, bool allowCast = true)
		{
			this.fieldName = fieldName;
			this.outputType = outputType;
			this.allowCast = allowCast;
		}
	}

	/// <summary>
	/// Allow you to modify the generated port view from a field. Can be used to generate multiple ports from one field.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class CustomPortBehaviorAttribute : Attribute
	{
		public string		fieldName;

		/// <summary>
		/// Allow you to modify the generated port view from a field. Can be used to generate multiple ports from one field.
		/// You must add this attribute on a function of this signature
		/// <code>
		/// IEnumerable&lt;PortData&gt; MyCustomPortFunction(List&lt;SerializableEdge&gt; edges);
		/// </code>
		/// </summary>
		/// <param name="fieldName">local node field name</param>
		public CustomPortBehaviorAttribute(string fieldName)
		{
			this.fieldName = fieldName;
		}
	}

	/// <summary>
	/// Automatically export all serializable fields of a class/struct as multiple ports.
	/// Use with [Input] or [Output] on a field whose type is a serializable class or struct.
	/// Can also be used alone (without [Input]/[Output]) - the field is treated as Input for port generation.
	/// Each public field or [SerializeField] field of that type will become a separate port.
	/// When used without [Input]/[Output] or on a non-public field, add [SerializeField] for Unity serialization.
	/// Note: You need [CustomPortInput] and [CustomPortOutput] for data transfer when using this attribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class ExportFieldsAsPortsAttribute : Attribute
	{
		/// <summary>
		/// Optional name prefix for generated ports. If null, uses the sub-field name.
		/// </summary>
		public string portNamePrefix;

		/// <summary>
		/// Automatically export all serializable fields as multiple ports.
		/// </summary>
		/// <param name="portNamePrefix">Optional prefix for port display names</param>
		public ExportFieldsAsPortsAttribute(string portNamePrefix = null)
		{
			this.portNamePrefix = portNamePrefix;
		}
	}

	/// <summary>
	/// When applied to an input field (or ExportFieldsAsPorts + Input), shows PropertyField(s) for editing default value when the port has no connection.
	/// Output fields do not support default value editing. Without this attribute, input fields use the original logic (no inline editor).
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class AllowDefaultEditAttribute : Attribute
	{
	}

	/// <summary>
	/// Allow to bind a method to generate a specific set of ports based on a field type in a node
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class CustomPortTypeBehavior : Attribute
	{
		/// <summary>
		/// Target type
		/// </summary>
		public Type type;

		public CustomPortTypeBehavior(Type type)
		{
			this.type = type;
		}
	}

	/// <summary>
	/// Allow you to have a custom view for your stack nodes
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class CustomStackNodeView : Attribute
	{
		public Type	stackNodeType;

		/// <summary>
		/// Allow you to have a custom view for your stack nodes
		/// </summary>
		/// <param name="stackNodeType">The type of the stack node you target</param>
		public CustomStackNodeView(Type stackNodeType)
		{
			this.stackNodeType = stackNodeType;
		}
	}

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class VisibleIf : Attribute
	{
		public string fieldName;
		public object value;

		public VisibleIf(string fieldName, object value)
		{
			this.fieldName = fieldName;
			this.value = value;
		}
	}

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class ShowInInspector : Attribute
	{
		public bool showInNode;

		public ShowInInspector(bool showInNode = false)
		{
			this.showInNode = showInNode;
		}
	}
	
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class ShowAsDrawer : Attribute
	{
	}
	
	[AttributeUsage(AttributeTargets.Field)]
	public class SettingAttribute : Attribute
	{
		public string name;

		public SettingAttribute(string name = null)
		{
			this.name = name;
		}
	}

	[AttributeUsage(AttributeTargets.Method)]
	public class IsCompatibleWithGraph : Attribute {}
	
	[AttributeUsage(AttributeTargets.Class)]
	public class NodeCustomEditor : Attribute
	{
		public Type nodeType;

		public NodeCustomEditor(Type nodeType)
		{
			this.nodeType = nodeType;
		}
	}
}
