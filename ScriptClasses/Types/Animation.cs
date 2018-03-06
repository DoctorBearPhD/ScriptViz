namespace ScriptLib.Types
{
    public class Animation : BaseType
    {
        public int Index { get; set; }
        public object Type { get; set; }
        public int FrameStart { get; set; }
        public int FrameEnd { get; set; }
        public int Unknown1 { get; set; }
        public int Unknown2 { get; set; }
    }
}
