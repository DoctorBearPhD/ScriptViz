using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ScriptLib
{
    [ExpandableObject]
    [CategoryOrder("Common", 1), CategoryOrder("Misc", 3), CategoryOrder("Types", 4), CategoryOrder("Unknowns", 99) ]
    public class HitboxEffectType
    {
        [Category("Common"), PropertyOrder(2)] public int Type { get; set; }
        [Category("Common"), PropertyOrder(1)] public int Index { get; set; }
        [Category("Common"), PropertyOrder(3)] public int DamageType { get; set; }
        [Browsable(false)] public int Unused1 { get; set; }
        [Category("Types")] public int NumberOfType1 { get; set; }
        [Category("Types")] public int NumberOfType2 { get; set; }
        [Browsable(false)] public int Unused2 { get; set; }
        [Category("Common"), PropertyOrder(4)] public int Damage { get; set; }
        [Category("Common"), PropertyOrder(5)] public int Stun { get; set; }
        [Category("Unknowns"), PropertyOrder(1)] public int Index9 { get; set; }
        [Category("Common"), PropertyOrder(8)] public int EXBuildAttacker { get; set; }
        [Category("Common"), PropertyOrder(9)] public int EXBuildDefender { get; set; }
        [Category("Unknowns")] public int Index12 { get; set; }
        [Category("Common"), PropertyOrder(10)] public int HitStunFramesAttacker { get; set; }
        [Category("Common"), PropertyOrder(11)] public int HitStunFramesDefender { get; set; }
        [Category("Common"), PropertyOrder(13)] public int FuzzyEffect { get; set; }
        [Category("Common"), PropertyOrder(12)] public int RecoveryAnimationFramesDefender { get; set; }
        [Category("Unknowns")] public int Index17 { get; set; }
        [Category("Unknowns")] public int Index18 { get; set; }
        [Category("Unknowns")] public int Index19 { get; set; }
        [Category("Common"), PropertyOrder(6)] public float KnockBack { get; set; }
        [Category("Common"), PropertyOrder(7)] public float FallSpeed { get; set; }
        [Category("Unknowns")] public int Index22 { get; set; }
        [Category("Unknowns")] public int Index23 { get; set; }
        [Category("Unknowns")] public int Index24 { get; set; }
        [Category("Unknowns")] public int Index25 { get; set; }
        [Browsable(false)] public int OffsetToStartOfType1 { get; set; }
        [Browsable(false)] public int OffsetToStartOfType2 { get; set; }
        [Category("Types")] public HitboxFX.Type1[] Type1s { get; set; }
        [Category("Types")] public HitboxFX.Type2[] Type2s { get; set; }
    }
}
