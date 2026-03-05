using System;
using System.Collections.Generic;

namespace Air.GameCore
{
    /// <summary>
    /// Generic object pool. Reduces allocations in hot paths.
    /// </summary>
    public static class ObjectPool<T> where T : class, new()
    {
        private static readonly Stack<T> Pool = new();
        private static readonly Func<T> DefaultFactory = () => new T();

        private static Func<T> _factory = DefaultFactory;

        /// <summary>
        /// Sets custom factory for creating new instances. Use for types without parameterless constructor.
        /// </summary>
        public static void SetFactory(Func<T> factory) => _factory = factory ?? DefaultFactory;

        /// <summary>
        /// Gets an instance from pool. Creates new if pool is empty.
        /// </summary>
        public static T Get()
        {
            return Pool.Count > 0 ? Pool.Pop() : _factory();
        }

        /// <summary>
        /// Returns instance to pool. Calls Reset() if type implements IPoolable.
        /// </summary>
        public static void Return(T obj)
        {
            if (obj == null) return;
            if (obj is IPoolable poolable)
                poolable.Reset();
            Pool.Push(obj);
        }
    }
}
