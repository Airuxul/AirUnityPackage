namespace Air.UnityGameCore.Runtime.UI
{
    /// <summary>
    /// UI 展示参数标记接口，用于页面或组件的展示入参。
    /// </summary>
    public interface IUIShowParam
    {
        /// <summary>
        /// 将当前页面数据转换为指定组件参数类型。
        /// </summary>
        /// <param name="componentParam">转换得到的组件参数，未匹配时为 default</param>
        /// <returns>是否支持该组件参数类型</returns>
        bool TryTranslate<T>(out T componentParam) where T : struct, IUIShowParam;
    }

    /// <summary>
    /// 示例：页面级参数，承载整页展示所需数据；组件通过 TryTranslate 获取自身参数。
    /// </summary>
    public struct StartShowParam : IUIShowParam
    {
        public string StartText;
        public float LeftTime;

        /// <inheritdoc />
        public bool TryTranslate<T>(out T componentParam) where T : struct, IUIShowParam
        {
            componentParam = default;
            if (typeof(T) == typeof(CountDownParam))
            {
                componentParam = (T)(object)new CountDownParam { LeftTime = LeftTime };
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// 倒计时组件所需参数，由页面级参数（如 StartShowParam）转换得到。
    /// </summary>
    public struct CountDownParam : IUIShowParam
    {
        public float LeftTime;
        public bool TryTranslate<T>(out T componentParam) where T : struct, IUIShowParam
        {
            throw new System.NotImplementedException();
        }
    }
}
