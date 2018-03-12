using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ScriptLib.Types
{
    [CategoryOrder("Common", 1), CategoryOrder("Misc", 2), CategoryOrder("BACVERint", 99)]
    public class Type1 : BaseType
    {
        public int Flag1 { get; set; }
        public int Flag2 { get; set; }
    }
}
