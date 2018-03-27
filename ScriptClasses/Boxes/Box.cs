using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ScriptLib
{
    public class Box
    {
        [Category("Common"), JsonProperty(Order = -2), Display(Order = -99)] public int TickStart { get; set; }
        [Category("Common"), JsonProperty(Order = -2), Display(Order = -98)] public int TickEnd { get; set; }

        [Category("BACVERint"), DefaultValue(0), JsonProperty(Order = -2), Display(Order = 101)] public int? BACVERint1 { get; set; }
        [Category("BACVERint"), DefaultValue(0), JsonProperty(Order = -2), Display(Order = 102)] public int? BACVERint2 { get; set; }
        [Category("BACVERint"), DefaultValue(0), JsonProperty(Order = -2), Display(Order = 103)] public int? BACVERint3 { get; set; }
        [Category("BACVERint"), DefaultValue(0), JsonProperty(Order = -2), Display(Order = 104)] public int? BACVERint4 { get; set; }

        [Category("Common"), JsonProperty(Order = -2), Display(Order = -97)] public float X { get; set; }
        [Category("Common"), JsonProperty(Order = -2), Display(Order = -96)] public float Y { get; set; }
        [Category("Common"), JsonProperty(Order = -2), Display(Order = -95)] public float Z { get; set; }
        [Category("Common"), JsonProperty(Order = -2), Display(Order = -94)] public float Width { get; set; }
        [Category("Common"), JsonProperty(Order = -2), Display(Order = -93)] public float Height { get; set; }

        [Category("Unknown"), JsonProperty(Order = -2), Display(Order = 51)] public int Unknown1 { get; set; }
        [Category("Unknown"), JsonProperty(Order = -2), Display(Order = 52)] public int Unknown2 { get; set; }
        [Category("Unknown"), JsonProperty(Order = -2), Display(Order = 53)] public int Unknown3 { get; set; }
        [Category("Unknown"), JsonProperty(Order = -2), Display(Order = 54)] public int Unknown4 { get; set; }
        [Category("Unknown"), JsonProperty(Order = -2), Display(Order = 55)] public int Unknown5 { get; set; }
        [Category("Unknown"), JsonProperty(Order = -2), Display(Order = 56)] public int Unknown6 { get; set; }
    }
}
