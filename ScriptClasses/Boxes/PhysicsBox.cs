using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ScriptLib
{
    [CategoryOrder("Common", 1), CategoryOrder("Unknown", 98), CategoryOrder("BACVERint", 99)]
    public class PhysicsBox : Box
    {
        [Category("Unknown")] public int Unknown1 { get; set; }
        [Category("Unknown")] public int Unknown2 { get; set; }
        [Category("Unknown")] public int Unknown3 { get; set; }
        [Category("Unknown")] public int Unknown4 { get; set; }
        [Category("Unknown")] public int Unknown5 { get; set; }
        [Category("Unknown")] public int Unknown6 { get; set; }
    }
}
