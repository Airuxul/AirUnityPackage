using System.Collections.Generic;
using UnityEngine;

namespace GraphProcessor
{
    /// <summary>
    /// Custom type conversions for graph port connections.
    /// </summary>
    public class CustomTypeAdapter : ITypeAdapter
    {
        public static Vector4 ConvertFloatToVector4(float from) => new Vector4(from, from, from, from);

        public static float ConvertVector4ToFloat(Vector4 from) => from.x;

        public static Vector2 ConvertVector3ToVector2(Vector3 from) => new Vector2(from.x, from.y);
        public static Vector3 ConvertVector2ToVector3(Vector2 from) => new Vector3(from.x, from.y, 0f);

        public static Vector2 ConvertVector4ToVector2(Vector4 from) => new Vector2(from.x, from.y);
        public static Vector4 ConvertVector2ToVector4(Vector2 from) => new Vector4(from.x, from.y, 0f, 0f);

        public static Vector3 ConvertVector4ToVector3(Vector4 from) => new Vector3(from.x, from.y, from.z);
        public static Vector4 ConvertVector3ToVector4(Vector3 from) => new Vector4(from.x, from.y, from.z, 0f);

        public override IEnumerable<(System.Type, System.Type)> GetIncompatibleTypes()
        {
            yield return (typeof(ConditionalLink), typeof(object));
            yield return (typeof(RelayNode.PackedRelayData), typeof(object));
        }
    }
}
