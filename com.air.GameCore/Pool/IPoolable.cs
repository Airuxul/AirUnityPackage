namespace Air.GameCore
{
    /// <summary>
    /// Reset method for pooled objects. Called before returning to pool.
    /// </summary>
    public interface IPoolable
    {
        void Reset();
    }
}
