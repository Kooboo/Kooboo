using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.IndexedDB.Condition;
using Kooboo.IndexedDB.Condition.Expression;
using Kooboo.IndexedDB.Query;

namespace Kooboo.IndexedDB.IndexRange
{
    public static class ExpressionExtensions
    {
        public static List<Range<byte[]>> GetRanges(this Node node, string name, IComparer<byte[]> comparer)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            name = name.ToLower();
            var ranges = MatchRange(name, node, comparer).ToList();

            if (!ranges.Any() || ranges.Any(a => a.Take == Range<byte[]>.Kind.All))
            {
                return null;
            }

            var containsNone = ranges.Any(a => a.Take == Range<byte[]>.Kind.None);
            return containsNone ? new List<Range<byte[]>>() : ranges;
        }

        /// <summary>
        /// get the range query collection of index fields.for looping.  this can be OrderBy fields or fields that has more sparnse. this only works for index fields. 
        /// after get, the related field or property item will be removed from the item collection.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="node"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        private static IEnumerable<Range<byte[]>> MatchRange(string name, Node node,
            IComparer<byte[]> comparer)
        {
            switch (node.NodeType)
            {
                case NodeType.Filter:
                    var filter = node as FilterNode;
                    yield return MatchFilter(name, filter);
                    break;

                case NodeType.Binary:
                    var binary = node as BinaryNode;
                    foreach (var item in MatchBinary(name, binary, comparer))
                    {
                        yield return item;
                    }

                    break;

                case NodeType.Value:
                    var value = node as ValueNode;
                    yield return MatchValue(value);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static Range<byte[]> MatchFilter(string name, FilterNode filter)
        {
            if (filter.Property.ToLower() != name)
            {
                return Range<byte[]>.TakeAll();
            }

            switch (filter.Comparer)
            {
                case Comparer.EqualTo:
                    return Range<byte[]>.only(filter.Value.ValueBytes);

                case Comparer.GreaterThan:
                    return Range<byte[]>.lowerBound(filter.Value.ValueBytes, true);

                case Comparer.GreaterThanOrEqual:
                    return Range<byte[]>.lowerBound(filter.Value.ValueBytes, false);

                case Comparer.LessThan:
                    return Range<byte[]>.upperBound(filter.Value.ValueBytes, true);

                case Comparer.LessThanOrEqual:
                    return Range<byte[]>.upperBound(filter.Value.ValueBytes, false);

                case Comparer.NotEqualTo:
                case Comparer.StartWith:
                case Comparer.Contains:
                    return Range<byte[]>.TakeAll();

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static IEnumerable<Range<byte[]>> MatchBinary(string name, BinaryNode binary,
            IComparer<byte[]> comparer)
        {
            var left = MatchRange(name, binary.Left, comparer).ToArray();
            var right = MatchRange(name, binary.Right, comparer).ToArray();

            return binary.Operand switch
            {
                Operand.And => left.ExceptRanges(right, comparer),
                Operand.Or => left.UnionRanges(right, comparer),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static Range<byte[]> MatchValue(ValueNode value)
        {
            return value.Boolean ? Range<byte[]>.TakeAll() : Range<byte[]>.TakeNone();
        }
    }
}