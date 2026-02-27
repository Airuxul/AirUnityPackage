using Newtonsoft.Json;

namespace TimelineExporter
{
    /// <summary>
    /// Loads TimelineData from JSON (Resources or file path).
    /// Uses Newtonsoft.Json to avoid Unity JsonUtility's depth limit (10) with nested childTracks.
    /// </summary>
    public static class TimelineDataLoader
    {
        private static readonly JsonSerializerSettings Settings = new()
        {
            MaxDepth = 64
        };

        public static TimelineData LoadFromJson(string json)
        {
            return JsonConvert.DeserializeObject<TimelineData>(json, Settings);
        }
    }
}
