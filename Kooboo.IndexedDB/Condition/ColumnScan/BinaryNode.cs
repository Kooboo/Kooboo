namespace Kooboo.IndexedDB.Condition.ColumnScan
{
    public class BinaryNode : Node
    {
        public Node Left { get; set; }
        public Operand Operand { get; set; }
        public Node Right { get; set; }
        protected override NodeType NodeType => NodeType.Binary;
    }
}