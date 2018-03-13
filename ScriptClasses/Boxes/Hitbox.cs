using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ScriptLib
{
    [CategoryOrder("Common", 1), CategoryOrder("Misc", 2), CategoryOrder("Unknown", 98), CategoryOrder("BACVERint", 99)]
    public class Hitbox : Box
    {
        [Category("Unknown")]
        public int Unknown7 { get; set; }

        [Category("Unknown")]
        public int Unknown8 { get; set; }

        public int NumberOfHits { get; set; }
        public int HitType { get; set; }
        public int JuggleLimit { get; set; }
        public int JuggleIncrease { get; set; }
        public int Flag4 { get; set; }

        [Category("Common")]
        public int HitboxEffectIndex { get; set; }

        [Category("Unknown"), PropertyOrder(10)]
        public int Unknown10 { get; set; }

        [Category("Unknown"), PropertyOrder(11)]
        public int Unknown11 { get; set; }

        [Category("Unknown"), PropertyOrder(12)]
        public int Unknown12 { get; set; }
    }
}
