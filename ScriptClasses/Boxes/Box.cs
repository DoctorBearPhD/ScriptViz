﻿using Newtonsoft.Json;
using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ScriptLib
{
    [CategoryOrder("Common", 1), 
     CategoryOrder("BACVERint", 99)]
    public class Box
    {
        [Category("Common"),PropertyOrder(1),JsonProperty(Order = -2)] public int TickStart { get; set; }
        [Category("Common"), PropertyOrder(2), JsonProperty(Order = -2)] public int TickEnd { get; set; }

        [Category("BACVERint"), DefaultValue(0), JsonProperty(Order = -2)] public int? BACVERint1 { get; set; }
        [Category("BACVERint"), DefaultValue(0), JsonProperty(Order = -2)] public int? BACVERint2 { get; set; }
        [Category("BACVERint"), DefaultValue(0), JsonProperty(Order = -2)] public int? BACVERint3 { get; set; }
        [Category("BACVERint"), DefaultValue(0), JsonProperty(Order = -2)] public int? BACVERint4 { get; set; }

        [Category("Common"), JsonProperty(Order = -2)] public float X { get; set; }
        [Category("Common"), JsonProperty(Order = -2)] public float Y { get; set; }
        [Category("Common"), JsonProperty(Order = -2)] public float Z { get; set; }

        [Category("Common"), JsonProperty(Order = -2), PropertyOrder(3)]
        public float Width { get; set; }
        [Category("Common"), JsonProperty(Order = -2), PropertyOrder(4)]
        public float Height { get; set; }
    }
}
