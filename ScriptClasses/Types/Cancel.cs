using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ScriptLib.Types
{
    [CategoryOrder("Common", 1), CategoryOrder("Misc", 2), CategoryOrder("BACVERint", 99)]
    public class Cancel : BaseType
    {
        [Category("Common")] public int CancelList { get; set; }
        public int Type { get; set; }
    }
}
