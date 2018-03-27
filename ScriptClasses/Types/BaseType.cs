using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ScriptLib.Types
{
    public class BaseType
    {
        [JsonProperty(Order = -2), Display(Order = -99)] public int TickStart { get; set; }
        [JsonProperty(Order = -2), Display(Order = -98)] public int TickEnd { get; set; }

        [DefaultValue(0)]
        [JsonProperty(Order = -2), Display(Order = 101)] public int? BACVERint1 { get; set; }

        [DefaultValue(0)]
        [JsonProperty(Order = -2), Display(Order = 102)] public int? BACVERint2 { get; set; }

        [DefaultValue(0)]
        [JsonProperty(Order = -2), Display(Order = 103)] public int? BACVERint3 { get; set; }

        [DefaultValue(0)]
        [JsonProperty(Order = -2), Display(Order = 104)] public int? BACVERint4 { get; set; }
    }
}
