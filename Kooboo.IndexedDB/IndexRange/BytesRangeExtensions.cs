using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.IndexedDB.IndexRange
{
    public static class RangeExtensions
    {
        public static IEnumerable<Range<byte[]>> ExceptRanges(this Range<byte[]>[] left,
            Range<byte[]>[] right, IComparer<byte[]> comparer)
        {
            if (left == null) throw new ArgumentNullException(nameof(left));
            if (right == null) throw new ArgumentNullException(nameof(right));
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));

            if (left.Any(a => a.Take == Range<byte[]>.Kind.All))
            {
                return right;
            }

            if (right.Any(a => a.Take == Range<byte[]>.Kind.All))
            {
                return left;
            }

            if (left.Union(right).Any(a => a.Take == Range<byte[]>.Kind.None))
            {
                return new[] { Range<byte[]>.TakeNone() };
            }

            var points = ToPoints(left, right, comparer)
                .Where(w => w.IsBetweenSpan)
                .Select(s => s.Point)
                .ToArray();

            return GetRanges(points);
        }

        public static IEnumerable<Range<byte[]>> UnionRanges(this Range<byte[]>[] left,
            Range<byte[]>[] right, IComparer<byte[]> comparer)
        {
            if (left == null) throw new ArgumentNullException(nameof(left));
            if (right == null) throw new ArgumentNullException(nameof(right));
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));

            if (left.Any(a => a.Take == Range<byte[]>.Kind.None))
            {
                return right;
            }

            if (right.Any(a => a.Take == Range<byte[]>.Kind.None))
            {
                return left;
            }

            if (left.Union(right).Any(a => a.Take == Range<byte[]>.Kind.All))
            {
                return new[] { Range<byte[]>.TakeAll() };
            }

            var points = ToPoints(left, right, comparer)
                .Where(w => !w.IsBetweenSpan)
                .Select(s => s.Point)
                .ToArray();

            return GetRanges(points);
        }

        private static IEnumerable<Range<byte[]>> GetRanges(IEnumerable<Point> points)
        {
            var result = new List<Range<byte[]>>();
            points = points.OrderBy(o => o).ToArray();
            Point lower = null;
            Point upper = null;
            var lastPoint = points.LastOrDefault();

            foreach (var point in points)
            {
                if (point.IsStart)
                {
                    if (lower != null && upper != null)
                    {
                        result.Add(new Range<byte[]>
                        {
                            upper = upper.Value,
                            upperOpen = upper.Open,
                            lower = lower.Value,
                            lowerOpen = lower.Open
                        });

                        lower = upper = null;
                    }

                    lower ??= point;
                }
                else
                {
                    upper = point;

                    if (point == lastPoint)
                    {
                        result.Add(new Range<byte[]>
                        {
                            upper = upper.Value,
                            upperOpen = upper.Open,
                            lower = lower?.Value,
                            lowerOpen = lower == null ? false : lower.Open
                        });
                    }
                }
            }

            return result;
        }

        private static IEnumerable<(bool IsBetweenSpan, Point Point)> ToPoints(IEnumerable<Range<byte[]>> left,
            IEnumerable<Range<byte[]>> right, IComparer<byte[]> comparer)
        {
            var leftSpans = left.Select(s => new Span(s, comparer)).ToArray();
            var rightSpans = right.Select(s => new Span(s, comparer)).ToArray();
            var leftPoints = leftSpans.SelectMany(s => s.ToPoints(rightSpans));
            var rightPoints = rightSpans.SelectMany(s => s.ToPoints(leftSpans));
            return leftPoints.Union(rightPoints);
        }
    }
}