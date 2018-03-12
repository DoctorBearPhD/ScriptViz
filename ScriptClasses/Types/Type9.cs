using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ScriptLib.Types
{
    [CategoryOrder("Common", 1), CategoryOrder("Unknown", 98), CategoryOrder("BACVERint", 99)]
    public class Type9 : BaseType
    {
        [Category("Unknown")] public int Unknown1 { get; set; }
        [Category("Unknown")] public int Unknown2 { get; set; }
        [Category("Unknown")] public float Unknown3 { get; set; }
    }
}
