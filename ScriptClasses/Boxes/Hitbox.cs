using System.ComponentModel.DataAnnotations;

namespace ScriptLib
{
    public class Hitbox : Box
    {
        // Display order should start at 7 for known properties, 50 + # for Unknown# properties
        [Display(Order = 57)] public int Unknown7 { get; set; }
        [Display(Order = 58)] public int Unknown8 { get; set; }
        [Display(Order = 4)] public int NumberOfHits { get; set; }
        [Display(Order = 3)] public int HitType { get; set; }
        [Display(Order = 1)] public int JuggleLimit { get; set; }
        [Display(Order = 0)] public int JuggleIncrease { get; set; }
        [Display(Order = 5)] public int Flag4 { get; set; }
        [Display(Order = 2)] public int HitboxEffectIndex { get; set; }
        [Display(Order = 60)] public int Unknown10 { get; set; }
        [Display(Order = 61)] public int Unknown11 { get; set; }
        [Display(Order = 62)] public int Unknown12 { get; set; }
    }
}
