namespace ScriptLib
{
    public class BACFile
    {
        public MoveList[] MoveLists { get; set; }
        public HitboxEffects[] HitboxEffectses { get; set; }
        public string RawUassetHeaderDontTouch { get; set; }
    }

    public class MoveList
    {
        public Move[] Moves { get; set; }
        public int Unknown1 { get; set; }
    }

    public class Move
    {
        public string Name { get; set; }
        public int Index { get; set; }
        public int FirstHitboxFrame { get; set; }
        public int LastHitboxFrame { get; set; }
        public int InterruptFrame { get; set; }
        public int TotalTicks { get; set; }
        public int ReturnToOriginalPosition { get; set; }
        public float Slide { get; set; }
        public float unk3 { get; set; }
        public float unk4 { get; set; }
        public float unk5 { get; set; }
        public float unk6 { get; set; }
        public float unk7 { get; set; }
        public int Flag { get; set; }
        public int unk9 { get; set; }
        public int numberOfTypes { get; set; }
        public int unk13 { get; set; }
        public int HeaderSize { get; set; }
        public int Unknown12 { get; set; }
        public int Unknown13 { get; set; }
        public int Unknown14 { get; set; }
        public int Unknown15 { get; set; }
        public int Unknown16 { get; set; }
        public int Unknown17 { get; set; }
        public float Unknown18 { get; set; }
        public int Unknown19 { get; set; }
        public int Unknown20 { get; set; }
        public int Unknown21 { get; set; }
        public int Unknown22 { get; set; }
        public AutoCancel[] AutoCancels { get; set; }
        public Type1[] Type1s { get; set; }
        public Force[] Forces { get; set; }
        public Cancel[] Cancels { get; set; }
        public Other[] Others { get; set; }
        public Hitbox[] Hitboxes { get; set; }
        public Hurtbox[] Hurtboxes { get; set; }
        public PhysicsBox[] PhysicsBoxes { get; set; }
        public Animation[] Animations { get; set; }
        public Type9[] Type9s { get; set; }
        public SoundEffect[] SoundEffects { get; set; }
        public VisualEffect[] VisualEffects { get; set; }
        public Position[] Positions { get; set; }
    }

    public class AutoCancel
    {
        public int TickStart { get; set; }
        public int TickEnd { get; set; }
        public object Condition { get; set; }
        public int MoveIndex { get; set; }
        public object MoveIndexName { get; set; }
        public int Unknown1 { get; set; }
        public int NumberOfInts { get; set; }
        public int Unknown2 { get; set; }
        public int Offset { get; set; }
        public int?[] Ints { get; set; }
    }

    public class Type1
    {
        public int TickStart { get; set; }
        public int TickEnd { get; set; }
        public int Flag1 { get; set; }
        public int Flag2 { get; set; }
    }

    public class Force
    {
        public int TickStart { get; set; }
        public int TickEnd { get; set; }
        public float Amount { get; set; }
        public object Flag { get; set; }
    }

    public class Cancel
    {
        public int TickStart { get; set; }
        public int TickEnd { get; set; }
        public int CancelList { get; set; }
        public int Type { get; set; }
    }

    public class Other
    {
        public int TickStart { get; set; }
        public int TickEnd { get; set; }
        public int Unknown1 { get; set; }
        public int Unknown2 { get; set; }
        public int NumberOfInts { get; set; }
        public int Offset { get; set; }
        public int[] Ints { get; set; }
    }

    public class Box
    {
        public int TickStart { get; set; }
        public int TickEnd { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
    }

    public class Hitbox : Box
    {
        public int Unknown1 { get; set; }
        public int Unknown2 { get; set; }
        public int Unknown3 { get; set; }
        public int Unknown4 { get; set; }
        public int Unknown5 { get; set; }
        public int Unknown6 { get; set; }
        public int Unknown7 { get; set; }
        public int Unknown8 { get; set; }
        public int NumberOfHits { get; set; }
        public int HitType { get; set; }
        public int JuggleLimit { get; set; }
        public int JuggleIncrease { get; set; }
        public int Flag4 { get; set; }
        public int HitboxEffectIndex { get; set; }
        public int Unknown10 { get; set; }
        public int Unknown11 { get; set; }
    }

    public class Hurtbox : Box
    {
        public int Unknown1 { get; set; }
        public int Unknown2 { get; set; }
        public int Unknown3 { get; set; }
        public int Unknown4 { get; set; }
        public int Unknown5 { get; set; }
        public int Unknown6 { get; set; }
        public int Unknown7 { get; set; }
        public int Unknown8 { get; set; }
        public int Unknown9 { get; set; }
        public int Flag1 { get; set; }
        public int Flag2 { get; set; }
        public int Flag3 { get; set; }
        public int Flag4 { get; set; }
        public int HitEffect { get; set; }
        public int Unknown10 { get; set; }
        public int Unknown11 { get; set; }
        public float Unknown12 { get; set; }
    }

    public class PhysicsBox : Box
    {
        public int Unknown1 { get; set; }
        public int Unknown2 { get; set; }
        public int Unknown3 { get; set; }
        public int Unknown4 { get; set; }
        public int Unknown5 { get; set; }
        public int Unknown6 { get; set; }
    }

    public class Animation
    {
        public int TickStart { get; set; }
        public int TickEnd { get; set; }
        public int Index { get; set; }
        public object Type { get; set; }
        public int FrameStart { get; set; }
        public int FrameEnd { get; set; }
        public int Unknown1 { get; set; }
        public int Unknown2 { get; set; }
    }

    public class Type9
    {
        public int TickStart { get; set; }
        public int TickEnd { get; set; }
        public int Unknown1 { get; set; }
        public int Unknown2 { get; set; }
        public float Unknown3 { get; set; }
    }

    public class SoundEffect
    {
        public int TickStart { get; set; }
        public int TickEnd { get; set; }
        public int Unknown1 { get; set; }
        public int Unknown2 { get; set; }
        public int Unknown3 { get; set; }
        public int Unknown4 { get; set; }
        public int Unknown5 { get; set; }
        public int Unknown6 { get; set; }
    }

    public class VisualEffect
    {
        public int TickStart { get; set; }
        public int TickEnd { get; set; }
        public int Unknown1 { get; set; }
        public int Unknown2 { get; set; }
        public int Unknown3 { get; set; }
        public int Type { get; set; }
        public int Unknown5 { get; set; }
        public int AttachPoint { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public int Unknown10 { get; set; }
        public float Size { get; set; }
        public float Unknown12 { get; set; }
    }

    public class Position
    {
        public int TickStart { get; set; }
        public int TickEnd { get; set; }
        public float Movement { get; set; }
        public int Flag { get; set; }
    }

    public class HitboxEffects
    {
        public int Index { get; set; }
        public HitType HIT_STAND { get; set; }
        public HitType HIT_CROUCH { get; set; }
        public HitType HIT_AIR { get; set; }
        public HitType HIT_UNKNOWN { get; set; }
        public HitType HIT_UNKNOWN2 { get; set; }
        public HitType GUARD_STAND { get; set; }
        public HitType GUARD_CROUCH { get; set; }
        public HitType GUARD_AIR { get; set; }
        public HitType GUARD_UNKNOWN { get; set; }
        public HitType GUARD_UNKNOWN2 { get; set; }
        public HitType COUNTERHIT_STAND { get; set; }
        public HitType COUNTERHIT_CROUCH { get; set; }
        public HitType COUNTERHIT_AIR { get; set; }
        public HitType COUNTERHIT_UNKNOWN { get; set; }
        public HitType COUNTERHIT_UNKNOWN2 { get; set; }
        public HitType UNKNOWN_STAND { get; set; }
        public HitType UNKNOWN_CROUCH { get; set; }
        public HitType UNKNOWN_AIR { get; set; }
        public HitType UNKNOWN_UNKNOWN { get; set; }
        public HitType UNKNOWN_UNKNOWN2 { get; set; }
    }

    public class HitType
    {
        public int Type { get; set; }
        public int Index { get; set; }
        public int DamageType { get; set; }
        public int Unused1 { get; set; }
        public int NumberOfType1 { get; set; }
        public int NumberOfType2 { get; set; }
        public int Unused2 { get; set; }
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
        public Type1s2[] Type1s { get; set; }
        public Type2s[] Type2s { get; set; }
    }

    public class Type1s2
    {
        public int Unknown1 { get; set; }
        public int SoundType { get; set; }
        public int Unknown3 { get; set; }
        public int Unknown4 { get; set; }
    }

    public class Type2s
    {
        public int EffectType1 { get; set; }
        public int EffectType2 { get; set; }
        public int EffectType3 { get; set; }
        public int Unknown4 { get; set; }
        public int EffectPosition { get; set; }
        public int Unknown6 { get; set; }
        public int Unknown7 { get; set; }
        public int Unknown8 { get; set; }
        public int Unknown9 { get; set; }
        public float Size { get; set; }
        public int Unknown11 { get; set; }
    }
}
