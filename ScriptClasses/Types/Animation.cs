using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ScriptLib.Types
{
    [CategoryOrder("Common", 1), CategoryOrder("Misc", 2), CategoryOrder("Unknown", 98), CategoryOrder("BACVERint", 99)]
    public class Animation : BaseType
    {
        [Category("Common")] public int Index { get; set; }
        public object Type { get; set; }
        public int FrameStart { get; set; }
        public int FrameEnd { get; set; }
        [Category("Unknown")] public int Unknown1 { get; set; }
        [Category("Unknown")] public int Unknown2 { get; set; }
    }
}
