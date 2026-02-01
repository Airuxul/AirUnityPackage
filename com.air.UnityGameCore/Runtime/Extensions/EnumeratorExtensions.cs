using System.Collections.Generic;

namespace Extensions {
    public static class EnumeratorExtensions {
        /// <summary>
        /// Converts an IEnumerator T to an IEnumerable T.
        /// </summary>
        /// <param name="e">An instance of IEnumerator.</param>
        /// <returns>An IEnumerable with the same elements as the input instance.</returns>    
        public static IEnumerable<T> ToEnumerable<T>(this IEnumerator<T> e) {
            while (e.MoveNext()) {
                yield return e.Current;
            }
        }
    }
}