using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GraphProcessor
{
	public struct ConditionalLink {}
	
	/// <summary>
	/// This is the base class for every node that is executed by the conditional processor, it takes an executed bool as input to 
	/// </summary>
    [Serializable]
    public abstract class ConditionalNode : BaseNode, IConditionalNode
	{
		// These booleans will controls wether or not the execution of the folowing nodes will be done or discarded.
		[Input(name = "Executed", allowMultiple = true)]
		public ConditionalLink executed;

		public abstract IEnumerable< ConditionalNode > GetExecutedNodes();

		// Assure that the executed field is always at the top of the node port section
		public override FieldInfo[] GetNodeFields()
		{
			var fields = base.GetNodeFields();
			Array.Sort(fields, (f1, f2) => f1.Name == nameof(executed) ? -1 : 1);
			return fields;
		}
	}

	/// <summary>
	/// This class represent a simple node which takes one event in parameter and pass it to the next node
	/// </summary>
	[Serializable]
	public abstract class LinearConditionalNode : ConditionalNode, IConditionalNode
	{
		[Output(name = "Executes"), ExecutionOrder]
		public ConditionalLink	executes;

		public override IEnumerable< ConditionalNode >	GetExecutedNodes()
		{
			var nodes = outputPorts.FirstOrDefault(n => n.fieldName == nameof(executes))
				?.GetEdges().Select(e => e.inputNode as ConditionalNode);
			return ApplyExecutionOrder(this, nameof(executes), nodes ?? Enumerable.Empty<ConditionalNode>());
		}
	}
	
	/// <summary>
	/// This class represent a waitable node which invokes another node after a time/frame
	/// </summary>
	[Serializable]
	public abstract class WaitableNode : LinearConditionalNode
	{
		[Output(name = "Execute After"), ExecutionOrder]
		public ConditionalLink executeAfter;

		protected void ProcessFinished()
		{
			onProcessFinished.Invoke(this);
		}

		public Action<WaitableNode> onProcessFinished;

		public IEnumerable< ConditionalNode > GetExecuteAfterNodes()
		{
			var nodes = outputPorts.FirstOrDefault(n => n.fieldName == nameof(executeAfter))
				?.GetEdges().Select(e => e.inputNode as ConditionalNode);
			return ApplyExecutionOrder(this, nameof(executeAfter), nodes ?? Enumerable.Empty<ConditionalNode>());
		}
	}
}