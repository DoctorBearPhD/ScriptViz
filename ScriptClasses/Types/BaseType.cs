using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ScriptLib.Types
{
    public class BaseType
    {
        [JsonProperty(Order = -2)] public int TickStart { get; set; }
        [JsonProperty(Order = -2)] public int TickEnd { get; set; }

        [DefaultValue(0)]
        [JsonProperty(Order = -2)] public int? BACVERint1 { get; set; }

        [DefaultValue(0)]
        [JsonProperty(Order = -2)] public int? BACVERint2 { get; set; }

        [DefaultValue(0)]
        [JsonProperty(Order = -2)] public int? BACVERint3 { get; set; }

        [DefaultValue(0)]
        [JsonProperty(Order = -2)] public int? BACVERint4 { get; set; }
    }
}
