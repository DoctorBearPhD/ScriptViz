using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ScriptLib
{
    [CategoryOrder("Common", 1), CategoryOrder("Misc", 3),
     CategoryOrder("Unknown", 98), CategoryOrder("BACVERint", 99)]
    public class VisualEffect : Types.BaseType
    {
        [Category("Unknown")] public int Unknown1 { get; set; }
        [Category("Unknown")] public int Unknown2 { get; set; }
        [Category("Unknown")] public int Unknown3 { get; set; }
        public int Type { get; set; }
        [Category("Unknown")] public int Unknown5 { get; set; }
        public int AttachPoint { get; set; }
        [Category("Common")] public float X { get; set; }
        [Category("Common")] public float Y { get; set; }
        [Category("Common")] public float Z { get; set; }
        [Category("Unknown")] public int Unknown10 { get; set; }
        [Category("Common")] public float Size { get; set; }
        [Category("Unknown")] public float Unknown12 { get; set; }
    }
}
