using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ScriptLib.Types
{
    [CategoryOrder("Common", 1), CategoryOrder("Misc", 2), CategoryOrder("Unknown", 98), CategoryOrder("BACVERint", 99)]
    public class Other : BaseType
    {
        [Category("Unknown")] public int Unknown1 { get; set; }
        [Category("Unknown")] public int Unknown2 { get; set; }
        public int NumberOfInts { get; set; }
        [Browsable(false)] public int Offset { get; set; }
        [ExpandableObject] public int[] Ints { get; set; }
    }
}
