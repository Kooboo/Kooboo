using Kooboo.IndexedDB.Query;

namespace Kooboo.IndexedDB.Condition.Expression
{
    public class FilterNode : Node
    {
        public string Property { get; set; }
        public Comparer Comparer { get; set; }

        public ValueNode Value { get; set; }

        public override NodeType NodeType => NodeType.Filter;
    }
}