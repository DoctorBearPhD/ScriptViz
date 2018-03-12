using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ScriptLib
{
    [CategoryOrder("Common", 1), CategoryOrder("Misc", 2), CategoryOrder("BACVERint", 99)]
    public class Position : Types.BaseType
    {
        [Category("Common")] public float Movement { get; set; }
        public int Flag { get; set; }
    }
}
