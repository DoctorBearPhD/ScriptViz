using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ScriptLib.Types
{
    [CategoryOrder("Common", 1), CategoryOrder("Misc", 2), CategoryOrder("BACVERint", 99)]
    public class Force : BaseType
    {
        [Category("Common")] public float Amount { get; set; }
        public object Flag { get; set; }
    }
}
