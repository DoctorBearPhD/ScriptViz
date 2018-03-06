namespace ScriptLib
{
    public class BACFile : BaseFile
    {
        [Newtonsoft.Json.JsonProperty(Order = 1)]
        public MoveList[] MoveLists { get; set; }

        [Newtonsoft.Json.JsonProperty(Order = 2)]
        public HitboxEffects[] HitboxEffectses { get; set; }

        [Newtonsoft.Json.JsonProperty(Order = 99)]
        public int BACVER { get; set; }
    }
}