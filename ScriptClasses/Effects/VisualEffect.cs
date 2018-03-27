using System.ComponentModel.DataAnnotations;

namespace ScriptLib
{
    public class VisualEffect : Types.BaseType
    {
        [Display(Order = 51)] public int Unknown1 { get; set; }
        [Display(Order = 52)] public int Unknown2 { get; set; }
        [Display(Order = 53)] public int Unknown3 { get; set; }

        [Display(Order = 2)] public int Type { get; set; }

        [Display(Order = 55)] public int Unknown5 { get; set; }

        [Display(Order = 0)] public int AttachPoint { get; set; }
        [Display(Order = -97)] public float X { get; set; }
        [Display(Order = -96)] public float Y { get; set; }
        [Display(Order = -95)] public float Z { get; set; }

        [Display(Order = 60)] public int Unknown10 { get; set; }

        [Display(Order = 1)] public float Size { get; set; }

        [Display(Order = 62)] public float Unknown12 { get; set; }
    }
}
