using System.Linq;

namespace Kooboo.IndexedDB.Condition.ColumnScan
{
    public class ValueNode : Node
    {
        private readonly byte[] _value;
        public static ValueNode True { get; } = new(new byte[] { 1 });

        public ValueNode(byte[] value)
        {
            _value = value;
        }

        public bool Value => _value?.Any(a => a > 0) ?? false;

        protected override NodeType NodeType => NodeType.Value;
    }
}