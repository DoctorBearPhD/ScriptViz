using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ScriptLib.Types
{
    public class AutoCancel : BaseType
    {
        [Display(Order = 2)] public object Condition { get; set; }
        [Display(Order = 0)] public int MoveIndex { get; set; }
        [Display(Order = 1)] public object MoveIndexName { get; set; }
        [Display(Order = 51)] public int Unknown1 { get; set; }
        [Browsable(false)] public int NumberOfInts { get; set; }
        [Display(Order = 52)] public int Unknown2 { get; set; }
        [Display(Order = 53)] public int Unknown3 { get; set; }
        [Display(Order = 54)] public int Unknown4 { get; set; }
        [Browsable(false)] public int Offset { get; set; }
        public int?[] Ints { get; set; }
    }
}
