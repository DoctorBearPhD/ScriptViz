namespace ScriptLib.Types
{
    public class AutoCancel : BaseType
    {
        public object Condition { get; set; }
        public int MoveIndex { get; set; }
        public object MoveIndexName { get; set; }
        public int Unknown1 { get; set; }
        public int NumberOfInts { get; set; }
        public int Unknown2 { get; set; }
        public int Unknown3 { get; set; }
        public int Unknown4 { get; set; }
        public int Offset { get; set; }
        public int?[] Ints { get; set; }
    }
}
