using System;

namespace Air.BehaviorTree
{
    /// <summary>
    /// When applied to an output port, child nodes are ordered by their Y position in the graph.
    /// Nodes with smaller Y (higher on screen) execute first.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class OrderByPositionAttribute : Attribute
    {
    }
}
