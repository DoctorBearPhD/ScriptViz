using System.ComponentModel.DataAnnotations;

namespace ScriptLib.Types
{
    public class Animation : BaseType
    {
        [Display(Order = 1)]  public int Index { get; set; }
        [Display(Order = 2)]  public object Type { get; set; }
        [Display(Order = 3)]  public int FrameStart { get; set; }
        [Display(Order = 4)]  public int FrameEnd { get; set; }
        [Display(Order = 51)] public int Unknown1 { get; set; }
        [Display(Order = 52)] public int Unknown2 { get; set; }
    }
}
