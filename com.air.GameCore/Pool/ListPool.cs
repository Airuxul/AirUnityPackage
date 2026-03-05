using System.Collections.Generic;

namespace Air.GameCore
{
    /// <summary>
    /// Pool for List&lt;T&gt;. Clears list before returning.
    /// </summary>
    public static class ListPool<T>
    {
        private static readonly Stack<List<T>> Pool = new();

        public static List<T> Get()
        {
            return Pool.Count > 0 ? Pool.Pop() : new List<T>();
        }

        public static void Return(List<T> list)
        {
            if (list == null) return;
            list.Clear();
            Pool.Push(list);
        }
    }
}
