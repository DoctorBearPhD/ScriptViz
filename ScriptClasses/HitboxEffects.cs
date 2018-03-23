using System.Collections.Generic;

namespace ScriptLib
{
    public class HitboxEffects
    {
        public int Index { get; set; }
        public HitboxEffectType HIT_STAND           { get; set; }
        public HitboxEffectType HIT_CROUCH          { get; set; }
        public HitboxEffectType HIT_AIR             { get; set; }
        public HitboxEffectType HIT_UNKNOWN         { get; set; }
        public HitboxEffectType HIT_UNKNOWN2        { get; set; }
        public HitboxEffectType GUARD_STAND         { get; set; }
        public HitboxEffectType GUARD_CROUCH        { get; set; }
        public HitboxEffectType GUARD_AIR           { get; set; }
        public HitboxEffectType GUARD_UNKNOWN       { get; set; }
        public HitboxEffectType GUARD_UNKNOWN2      { get; set; }
        public HitboxEffectType COUNTERHIT_STAND    { get; set; }
        public HitboxEffectType COUNTERHIT_CROUCH   { get; set; }
        public HitboxEffectType COUNTERHIT_AIR      { get; set; }
        public HitboxEffectType COUNTERHIT_UNKNOWN  { get; set; }
        public HitboxEffectType COUNTERHIT_UNKNOWN2 { get; set; }
        public HitboxEffectType UNKNOWN_STAND       { get; set; }
        public HitboxEffectType UNKNOWN_CROUCH      { get; set; }
        public HitboxEffectType UNKNOWN_AIR         { get; set; }
        public HitboxEffectType UNKNOWN_UNKNOWN     { get; set; }
        public HitboxEffectType UNKNOWN_UNKNOWN2    { get; set; }

        List<HitboxEffectType> mHitboxEffectTypes
        {
            get
            {
                if (IsEmpty()) return null;

                return new List<HitboxEffectType> {
                               HIT_STAND          ,
                               HIT_CROUCH         ,
                               HIT_AIR            ,
                               HIT_UNKNOWN        ,
                               HIT_UNKNOWN2       ,
                               GUARD_STAND        ,
                               GUARD_CROUCH       ,
                               GUARD_AIR          ,
                               GUARD_UNKNOWN      ,
                               GUARD_UNKNOWN2     ,
                               COUNTERHIT_STAND   ,
                               COUNTERHIT_CROUCH  ,
                               COUNTERHIT_AIR     ,
                               COUNTERHIT_UNKNOWN ,
                               COUNTERHIT_UNKNOWN2,
                               UNKNOWN_STAND      ,
                               UNKNOWN_CROUCH     ,
                               UNKNOWN_AIR        ,
                               UNKNOWN_UNKNOWN    ,
                               UNKNOWN_UNKNOWN2   };
            }
        }


        public List<HitboxEffectType> GetHitboxEffectTypes()
        {
            return mHitboxEffectTypes;
            //return this.GetType().GetProperties()
            //    .Where( p => p.PropertyType == typeof(HitboxEffectType) )
            //    .ToList();
        }

        public bool IsEmpty()
        {
            return HIT_STAND == null;
        }
    }
}
