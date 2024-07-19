using System.Collections.Generic;
using System.Linq;

namespace Kooboo.IndexedDB.IndexRange
{
    public class Span
    {
        private readonly Point _start;
        private readonly Point _end;

        public Span(Range<byte[]> range, IComparer<byte[]> comparer)
        {
            _start = new Point(range.lower, range.lowerOpen, true, comparer);
            _end = new Point(range.upper, range.upperOpen, false, comparer);
        }

        public IEnumerable<(bool IsBetweenOther, Point Point)> ToPoints(Span[] other)
        {
            yield return (other.Any(a => a.Contains(_start)), _start);
            yield return (other.Any(a => a.Contains(_end)), _end);
        }

        internal bool Contains(Point point)
        {
            return point.CompareTo(_start) > 0 && _end.CompareTo(point) > 0;
        }
    }
}