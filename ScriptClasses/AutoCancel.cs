namespace ScriptLib
{
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
}
