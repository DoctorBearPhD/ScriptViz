using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ScriptLib
{
    [CategoryOrder("Common", 1), CategoryOrder("Misc", 2), CategoryOrder("Unknown", 98), CategoryOrder("BACVERint", 99)]
    public class Hurtbox : Box
    {
        [Category("Unknown"), PropertyOrder(7)] public int Unknown7 { get; set; }
        [Category("Unknown"), PropertyOrder(8)] public int Unknown8 { get; set; }
        [Category("Unknown"), PropertyOrder(9)] public int Unknown9 { get; set; }
        public int Flag1 { get; set; }
        public int Flag2 { get; set; }
        public int Flag3 { get; set; }
        public int Flag4 { get; set; }
        [Category("Common")] public int HitEffect { get; set; }
        [Category("Unknown"), PropertyOrder(10)] public int Unknown10 { get; set; }
        [Category("Unknown"), PropertyOrder(11)] public int Unknown11 { get; set; }
        [Category("Unknown"), PropertyOrder(12)] public float Unknown12 { get; set; }
        [Category("Unknown"), PropertyOrder(13)] public int Unknown13 { get; set; }
    }
}
