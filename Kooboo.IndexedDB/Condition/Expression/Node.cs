using System.Collections.Generic;

namespace Kooboo.IndexedDB.Condition.Expression
{
    public abstract class Node
    {
        public abstract NodeType NodeType { get; }

        public IEnumerable<Node> GetNodes() => GetNodes(this);

        private IEnumerable<Node> GetNodes(Node expression)
        {
            if (expression is BinaryNode binaryExpression)
            {
                foreach (var item in GetNodes(binaryExpression.Left))
                {
                    yield return item;
                }

                foreach (var item in GetNodes(binaryExpression.Right))
                {
                    yield return item;
                }
            }
            else if (expression is FilterNode filterExpression)
            {
                yield return filterExpression;
                yield return filterExpression.Value;
            }
            else if (expression is ValueNode)
            {
                yield return expression;
            }
        }

        public static Node And(Node left, FilterNode right)
        {
            if (left == null) return right;

            return new BinaryNode
            {
                Left = left,
                Operand = Operand.And,
                Right = right
            };
        }
    }
}