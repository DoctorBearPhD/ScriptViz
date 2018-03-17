using System.ComponentModel;
using Newtonsoft.Json;

namespace ScriptLib
{
    public class HitboxEffectType
    {
        [JsonIgnore]
        [ReadOnly(true)] public string Name { get; set; }

        public int Type { get; set; }
        public int Index { get; set; }
        public int DamageType { get; set; }
        [Browsable(false)] public int Unused1 { get; set; }
        public int NumberOfType1 { get; set; }
        public int NumberOfType2 { get; set; }
        [Browsable(false)] public int Unused2 { get; set; }
        public int Damage { get; set; }
        public int Stun { get; set; }
        public int Index9 { get; set; }
        public int EXBuildAttacker { get; set; }
        public int EXBuildDefender { get; set; }
        public int Index12 { get; set; }
        public int HitStunFramesAttacker { get; set; }
        public int HitStunFramesDefender { get; set; }
        public int FuzzyEffect { get; set; }
        public int RecoveryAnimationFramesDefender { get; set; }
        public int Index17 { get; set; }
        public int Index18 { get; set; }
        public int Index19 { get; set; }
        public float KnockBack { get; set; }
        public float FallSpeed { get; set; }
        public int Index22 { get; set; }
        public int Index23 { get; set; }
        public int Index24 { get; set; }
        public int Index25 { get; set; }
        public int OffsetToStartOfType1 { get; set; }
        public int OffsetToStartOfType2 { get; set; }
        public HitboxFX.Type1[] Type1s { get; set; }
        public HitboxFX.Type2[] Type2s { get; set; }
    }
}
