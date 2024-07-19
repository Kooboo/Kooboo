namespace Kooboo.IndexedDB.Condition.Expression
{
    public class BinaryNode : Node
    {
        public Node Left { get; set; }
        public Operand Operand { get; set; }
        public Node Right { get; set; }

        public override NodeType NodeType => NodeType.Binary;
    }
}