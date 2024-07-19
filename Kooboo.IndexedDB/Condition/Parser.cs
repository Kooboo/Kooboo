using Kooboo.IndexedDB.Condition.Expression;

namespace Kooboo.IndexedDB.Condition
{
    public static class Parser
    {
        public static Node Parse(string condition)
        {
            if (string.IsNullOrWhiteSpace(condition)) return null;
            var iterator = new Iterator(condition);
            iterator.TrimStart();
            return Analyze(iterator, null, null);
        }

        static Node Analyze(Iterator iterator, Node left, Operand? operand)
        {
            if (iterator.Current == '(')
            {
                iterator.Next();
                iterator.TrimStart();
                var result = Analyze(iterator, null, null);
                result = Merge(left, operand, result);
                if (iterator.Current != ')') throw new ConditionParseException(iterator.Position);
                iterator.Next();
                iterator.TrimStart();
                if (iterator.End || iterator.Current == ')') return result;
                if (!iterator.IsOperand()) throw new ConditionParseException(iterator.Position);
                var mOperand = iterator.ExtractOperand();
                result = Analyze(iterator, result, mOperand);
                return result;
            }
            else
            {
                var filter = GetFilter(iterator);
                var result = Merge(left, operand, filter);
                if (iterator.End || iterator.Current == ')') return result;
                if (!iterator.IsOperand()) throw new ConditionParseException(iterator.Position);
                var mOperand = iterator.ExtractOperand();

                switch (mOperand)
                {
                    case Operand.And:
                        var right = Analyze(iterator, filter, mOperand);
                        result = Merge(left, operand, right);
                        break;
                    case Operand.Or:
                        result = Analyze(iterator, result, mOperand);
                        break;
                    default:
                        throw new ConditionParseException(iterator.Position);
                }

                return result;
            }
        }

        private static FilterNode GetFilter(Iterator iterator)
        {
            var filter = new FilterNode();
            if (!iterator.IsValue()) throw new ConditionParseException(iterator.Position);
            filter.Property = iterator.ExtractValue(true).Value.ToString();
            if (!iterator.IsComparer()) throw new ConditionParseException(iterator.Position);
            filter.Comparer = iterator.ExtractComparer();
            if (!iterator.IsValue()) throw new ConditionParseException(iterator.Position);
            filter.Value = iterator.ExtractValue();
            return filter;
        }

        static Node Merge(Node left, Operand? operand, Node right)
        {
            if (operand.HasValue)
            {
                return new BinaryNode
                {
                    Left = left,
                    Operand = operand.Value,
                    Right = right
                };
            }

            return left ?? right;
        }
    }
}