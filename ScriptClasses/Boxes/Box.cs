using Newtonsoft.Json;

namespace ScriptLib
{
    public class Box
    {
        [JsonProperty(Order = -2)] public int TickStart { get; set; }
        [JsonProperty(Order = -2)] public int TickEnd { get; set; }
        [JsonProperty(Order = -2)] public int? BACVERint1 { get; set; }
        [JsonProperty(Order = -2)] public int? BACVERint2 { get; set; }
        [JsonProperty(Order = -2)] public int? BACVERint3 { get; set; }
        [JsonProperty(Order = -2)] public int? BACVERint4 { get; set; }
        [JsonProperty(Order = -2)] public float X { get; set; }
        [JsonProperty(Order = -2)] public float Y { get; set; }
        [JsonProperty(Order = -2)] public float Z { get; set; }
        [JsonProperty(Order = -2)] public float Width { get; set; }
        [JsonProperty(Order = -2)] public float Height { get; set; }
    }
}
