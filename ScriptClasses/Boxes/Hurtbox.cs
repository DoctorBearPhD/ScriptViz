using System.ComponentModel.DataAnnotations;

namespace ScriptLib
{
    public class Hurtbox : Box
    {
        [Display(Order = 57)] public int Unknown7 { get; set; }
        [Display(Order = 58)] public int Unknown8 { get; set; }
        [Display(Order = 59)] public int Unknown9 { get; set; }
        [Display(Order = 1)]  public int Flag1 { get; set; }
        [Display(Order = 2)]  public int Flag2 { get; set; }
        [Display(Order = 3)]  public int Flag3 { get; set; }
        [Display(Order = 4)]  public int Flag4 { get; set; }
        [Display(Order = 0)]  public int HitEffect { get; set; }
        [Display(Order = 60)] public int Unknown10 { get; set; }
        [Display(Order = 61)] public int Unknown11 { get; set; }
        [Display(Order = 62)] public float Unknown12 { get; set; }
        [Display(Order = 63)] public int Unknown13 { get; set; }
    }
}
