using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ScriptLib.Types
{
    public class BaseType
    {
        [Category("Common"), PropertyOrder(1), JsonProperty(Order = -2)] public int TickStart { get; set; }
        [Category("Common"), PropertyOrder(2), JsonProperty(Order = -2)] public int TickEnd { get; set; }

        [Category("BACVERint"), DefaultValue(0), JsonProperty(Order = -2)] public int? BACVERint1 { get; set; }
        [Category("BACVERint"), DefaultValue(0), JsonProperty(Order = -2)] public int? BACVERint2 { get; set; }
        [Category("BACVERint"), DefaultValue(0), JsonProperty(Order = -2)] public int? BACVERint3 { get; set; }
        [Category("BACVERint"), DefaultValue(0), JsonProperty(Order = -2)] public int? BACVERint4 { get; set; }
    }
}
