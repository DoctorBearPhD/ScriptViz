using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ScriptLib
{
    public class HitboxEffects
    {
        [PropertyOrder(1)]  public int Index                            { get; set; }
        [PropertyOrder(2)]  public HitboxEffectType HIT_STAND           { get; set; }
        [PropertyOrder(3)]  public HitboxEffectType HIT_CROUCH          { get; set; }
        [PropertyOrder(4)]  public HitboxEffectType HIT_AIR             { get; set; }
        [PropertyOrder(5)]  public HitboxEffectType HIT_UNKNOWN         { get; set; }
        [PropertyOrder(6)]  public HitboxEffectType HIT_UNKNOWN2        { get; set; }
        [PropertyOrder(7)]  public HitboxEffectType GUARD_STAND         { get; set; }
        [PropertyOrder(8)]  public HitboxEffectType GUARD_CROUCH        { get; set; }
        [PropertyOrder(9)]  public HitboxEffectType GUARD_AIR           { get; set; }
        [PropertyOrder(10)] public HitboxEffectType GUARD_UNKNOWN       { get; set; }
        [PropertyOrder(11)] public HitboxEffectType GUARD_UNKNOWN2      { get; set; }
        [PropertyOrder(12)] public HitboxEffectType COUNTERHIT_STAND    { get; set; }
        [PropertyOrder(13)] public HitboxEffectType COUNTERHIT_CROUCH   { get; set; }
        [PropertyOrder(14)] public HitboxEffectType COUNTERHIT_AIR      { get; set; }
        [PropertyOrder(15)] public HitboxEffectType COUNTERHIT_UNKNOWN  { get; set; }
        [PropertyOrder(16)] public HitboxEffectType COUNTERHIT_UNKNOWN2 { get; set; }
        [PropertyOrder(17)] public HitboxEffectType UNKNOWN_STAND       { get; set; }
        [PropertyOrder(18)] public HitboxEffectType UNKNOWN_CROUCH      { get; set; }
        [PropertyOrder(19)] public HitboxEffectType UNKNOWN_AIR         { get; set; }
        [PropertyOrder(20)] public HitboxEffectType UNKNOWN_UNKNOWN     { get; set; }
        [PropertyOrder(21)] public HitboxEffectType UNKNOWN_UNKNOWN2    { get; set; }

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
