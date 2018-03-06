namespace ScriptLib
{
    public class BACFile : BaseFile
    {
        public MoveList[] MoveLists { get; set; }
        public HitboxEffects[] HitboxEffectses { get; set; }
        public int BACVER { get; set; }
    }
}