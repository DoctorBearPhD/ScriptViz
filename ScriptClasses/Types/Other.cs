using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ScriptLib.Types
{
    public class Other : BaseType
    {
        [Display(Order = 51)] public int Unknown1 { get; set; }
        [Display(Order = 52)] public int Unknown2 { get; set; }
        [Browsable(false)] public int NumberOfInts { get; set; }
        [Browsable(false)] public int Offset { get; set; }
        public int[] Ints { get; set; }
    }
}
