namespace dotless.Core.Parser.Infrastructure.Nodes
{
    public class BooleanNode : Node
    {
        public bool Value { get; set; }

        public BooleanNode(bool value)
        {
            Value = value;
        }
    }
}
