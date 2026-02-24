using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace GraphProcessor
{
    /// <summary>
    /// Implement this to define type conversions for the graph.
    /// Example:
    /// <code>
    /// public class CustomConversions : ITypeAdapter
    /// {
    ///     public static Vector4 ConvertFloatToVector(float from) => new Vector4(from, from, from, from);
    /// }
    /// </code>
    /// </summary>
    public abstract class ITypeAdapter
    {
        public virtual IEnumerable<(Type, Type)> GetIncompatibleTypes() { yield break; }
    }

    /// <summary>
    /// Type conversion utilities for graph port connections.
    /// </summary>
    public static class TypeAdapter
    {
        static Dictionary<(Type from, Type to), Func<object, object>> adapters = new Dictionary<(Type from, Type to), Func<object, object>>();
        static Dictionary<(Type from, Type to), MethodInfo> adapterMethods = new Dictionary<(Type from, Type to), MethodInfo>();
        static List<(Type from, Type to)> incompatibleTypes = new List<(Type from, Type to)>();
        static bool adaptersLoaded = false;

#if !ENABLE_IL2CPP
        static Func<object, object> ConvertTypeMethodHelper<TParam, TReturn>(MethodInfo method)
        {
            var func = (Func<TParam, TReturn>)Delegate.CreateDelegate(typeof(Func<TParam, TReturn>), method);
            return (object param) => func((TParam)param);
        }
#endif

        static void LoadAllAdapters()
        {
            foreach (Type type in AppDomain.CurrentDomain.GetAllTypes())
            {
                if (typeof(ITypeAdapter).IsAssignableFrom(type) && !type.IsAbstract)
                {
                    var adapter = Activator.CreateInstance(type) as ITypeAdapter;
                    if (adapter != null)
                    {
                        foreach (var types in adapter.GetIncompatibleTypes())
                        {
                            incompatibleTypes.Add((types.Item1, types.Item2));
                            incompatibleTypes.Add((types.Item2, types.Item1));
                        }
                    }

                    foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                    {
                        if (method.GetParameters().Length != 1 || method.ReturnType == typeof(void))
                            continue;

                        Type from = method.GetParameters()[0].ParameterType;
                        Type to = method.ReturnType;

                        try
                        {
#if ENABLE_IL2CPP
                            Func<object, object> r = (object param) => method.Invoke(null, new object[] { param });
#else
                            var genericHelper = typeof(TypeAdapter).GetMethod("ConvertTypeMethodHelper", BindingFlags.Static | BindingFlags.NonPublic);
                            var constructedHelper = genericHelper.MakeGenericMethod(from, to);
                            var r = (Func<object, object>)constructedHelper.Invoke(null, new object[] { method });
#endif
                            adapters[(from, to)] = r;
                            adapterMethods[(from, to)] = method;
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"Failed to load type conversion: {method}\n{e}");
                        }
                    }
                }
            }

            foreach (var kp in adapters)
            {
                if (!adapters.ContainsKey((kp.Key.to, kp.Key.from)))
                    Debug.LogError($"Missing reverse conversion: {kp.Key.from} <-> {kp.Key.to}");
            }

            adaptersLoaded = true;
        }

        public static bool AreIncompatible(Type from, Type to)
        {
            return incompatibleTypes.Any(k => k.from == from && k.to == to);
        }

        public static bool AreAssignable(Type from, Type to)
        {
            if (!adaptersLoaded)
                LoadAllAdapters();
            if (AreIncompatible(from, to))
                return false;
            return adapters.ContainsKey((from, to));
        }

        public static MethodInfo GetConvertionMethod(Type from, Type to) => adapterMethods[(from, to)];

        public static object Convert(object from, Type targetType)
        {
            if (!adaptersLoaded)
                LoadAllAdapters();
            if (adapters.TryGetValue((from.GetType(), targetType), out var func))
                return func?.Invoke(from);
            return null;
        }
    }
}
