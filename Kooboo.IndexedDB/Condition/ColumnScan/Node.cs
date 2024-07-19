using System;
using System.Linq;
using Kooboo.IndexedDB.Columns;
using Kooboo.IndexedDB.Query;

namespace Kooboo.IndexedDB.Condition.ColumnScan
{
    public abstract class Node
    {
        protected abstract NodeType NodeType { get; }

        public static Node FromExpression(Func<string, IColumn> getColumn, Expression.Node node,
            string[] notInColumnIndexes)
        {
            if (node is null) return ValueNode.True;

            switch (node.NodeType)
            {
                case NodeType.Filter:
                    var filter = node as Expression.FilterNode;
                    if (notInColumnIndexes.Contains(filter.Property)) return ValueNode.True;
                    var column = getColumn(filter.Property);
                    if (column == null) throw new Exception("filter field must be  column with fixed len");

                    return new FilterNode
                    {
                        ColumnName = column.FieldName,
                        RelativeStartPosition = column.RelativePosition,
                        Length = column.Length,
                        Evaluator = ColumnEvaluator.GetEvaluator(column.DataType, filter.Comparer,
                            filter.Value.ValueBytes,
                            column.Length)
                    };

                case NodeType.Binary:
                    var binary = node as Expression.BinaryNode;

                    return new BinaryNode
                    {
                        Left = FromExpression(getColumn, binary.Left, notInColumnIndexes),
                        Operand = binary.Operand,
                        Right = FromExpression(getColumn, binary.Right, notInColumnIndexes)
                    };

                case NodeType.Value:
                    var value = node as Expression.ValueNode;
                    return new ValueNode(value.ValueBytes);

                default:
                    throw new ArgumentOutOfRangeException();
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

        public bool Match(long position, Func<long, int, int, byte[]> getColumn)
        {
            switch (NodeType)
            {
                case NodeType.Filter:
                    var filter = (FilterNode)this;
                    var value = getColumn(position, filter.RelativeStartPosition, filter.Length);
                    return filter.Evaluator.isMatch(value);

                case NodeType.Binary:
                    var binary = (BinaryNode)this;
                    var left = binary.Left.Match(position, getColumn);

                    return binary.Operand switch
                    {
                        Operand.And => left && binary.Right.Match(position, getColumn),
                        Operand.Or => left || binary.Right.Match(position, getColumn),
                        _ => throw new ArgumentOutOfRangeException()
                    };

                case NodeType.Value:
                    return ((ValueNode)this).Value;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}