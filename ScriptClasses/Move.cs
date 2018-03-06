using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ScriptLib.Types;

namespace ScriptLib
{
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

        List<PropertyInfo> gen;

        public List<PropertyInfo> GetGeneralProperties()
        {
            gen = (typeof(Move)).GetProperties().Where(
                p => p.PropertyType == typeof(string)
                  || p.PropertyType == typeof(int)
                  || p.PropertyType == typeof(float)
                  ).ToList();
            
            return gen;
        }

        public List<PropertyInfo> GetListProperties()
        {
            var lists = this.GetType().GetProperties().Except(GetGeneralProperties()).ToList();

            return lists;
        }

        public PropertyInfo[] GetAllProperties()
        {
            return this.GetType().GetProperties();
        }

        public int GetGeneralPropertiesOffset()
        {
            return gen != null ? gen.Count - 1 : this.GetGeneralProperties().Count - 1;
        }
    }
}
