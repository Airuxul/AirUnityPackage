namespace GraphProcessor
{
    /// <summary>
    /// When a node implements this interface, child edges from all of its output ports
    /// will be sorted by the child node's Y position. Used for control-flow nodes whose
    /// execution order depends on vertical layout (e.g. sequence, selector in behavior trees).
    /// </summary>
    public interface ISortChildrenByPosition
    {
    }
}
