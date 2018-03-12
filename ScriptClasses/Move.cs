using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using ScriptLib.Types;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ScriptLib
{
    [CategoryOrder("Common", 1), CategoryOrder("Types", 2), CategoryOrder("Misc", 3), 
     CategoryOrder("Unknown", 98), CategoryOrder("BACVERint", 99)]
    public class Move
    {
        [Category("Common")] public string Name { get; set; }
        [Category("Common")] public int Index { get; set; }
        [Category("Common")] public int FirstHitboxFrame { get; set; }
        [Category("Common")] public int LastHitboxFrame { get; set; }
        [Category("Common")] public int InterruptFrame { get; set; }
        [Category("Common")] public int TotalTicks { get; set; }
        public int ReturnToOriginalPosition { get; set; }
        public float Slide { get; set; }
        [Category("Unknown"), PropertyOrder(-1)] public float unk3 { get; set; }
        [Category("Unknown"), PropertyOrder(-1)] public float unk4 { get; set; }
        [Category("Unknown"), PropertyOrder(-1)] public float unk5 { get; set; }
        [Category("Unknown"), PropertyOrder(-1)] public float unk6 { get; set; }
        [Category("Unknown"), PropertyOrder(-1)] public float unk7 { get; set; }
        public int Flag { get; set; }
        [Category("Unknown"), PropertyOrder(-1)] public int unk9 { get; set; }
        [Browsable(false)] public int numberOfTypes { get; set; }
        [Category("Unknown")] public int unk13 { get; set; }
        [Browsable(false)] public int HeaderSize { get; set; }
        [Category("Unknown")] public int Unknown12 { get; set; }
        [Category("Unknown")] public int Unknown13 { get; set; }
        [Category("Unknown")] public int Unknown14 { get; set; }
        [Category("Unknown")] public int Unknown15 { get; set; }
        [Category("Unknown")] public int Unknown16 { get; set; }
        [Category("Unknown")] public int Unknown17 { get; set; }
        [Category("Unknown")] public float Unknown18 { get; set; }
        [Category("Unknown")] public int Unknown19 { get; set; }
        [Category("Unknown")] public int Unknown20 { get; set; }
        [Category("Unknown")] public int Unknown21 { get; set; }
        [Category("Unknown")] public int Unknown22 { get; set; }
        [Category("Types")]public AutoCancel[] AutoCancels { get; set; }
        [Category("Types")]public Type1[] Type1s { get; set; }
        [Category("Types")]public Force[] Forces { get; set; }
        [Category("Types")]public Cancel[] Cancels { get; set; }
        [Category("Types")]public Other[] Others { get; set; }
        [Category("Types")]public Hitbox[] Hitboxes { get; set; }
        [Category("Types")]public Hurtbox[] Hurtboxes { get; set; }
        [Category("Types")]public PhysicsBox[] PhysicsBoxes { get; set; }
        [Category("Types")]public Animation[] Animations { get; set; }
        [Category("Types")]public Type9[] Type9s { get; set; }
        [Category("Types")]public SoundEffect[] SoundEffects { get; set; }
        [Category("Types")]public VisualEffect[] VisualEffects { get; set; }
        [Category("Types")]public Position[] Positions { get; set; }

        List<PropertyInfo> _gen;

        public List<PropertyInfo> GetGeneralProperties()
        {
            _gen = (typeof(Move)).GetProperties().Where(
                p => p.PropertyType == typeof(string)
                  || p.PropertyType == typeof(int)
                  || p.PropertyType == typeof(float)
                  ).ToList();
            
            return _gen;
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
            return _gen != null ? _gen.Count - 1 : this.GetGeneralProperties().Count - 1;
        }
    }
}
