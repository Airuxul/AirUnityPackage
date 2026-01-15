using System.Collections.Generic;

namespace UnityGameCore.Runtime.Extensions {
    public static class ListExtensions {
        public static void RefreshWith<T>(this List<T> list, IEnumerable<T> items) {
            list.Clear();
            list.AddRange(items);
        }
    }
}