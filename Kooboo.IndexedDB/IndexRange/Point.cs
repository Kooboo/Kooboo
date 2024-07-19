using System;
using System.Collections.Generic;

namespace Kooboo.IndexedDB.IndexRange
{
    public class Point : IComparable<Point>
    {
        public Point(byte[] value, bool open, bool isStart, IComparer<byte[]> comparer)
        {
            Value = value;
            Open = open;
            IsStart = isStart;
            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        }

        public byte[] Value { get; }
        public bool Open { get; }
        public bool IsStart { get; }
        private IComparer<byte[]> Comparer { get; }

        public int CompareTo(Point other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            switch (IsStart)
            {
                case true when Value == null:
                    return -1;
                case false when Value == null:
                    return 1;
            }

            switch (other.IsStart)
            {
                case true when other.Value == null:
                    return 1;
                case false when other.Value == null:
                    return -1;
            }

            var result = Comparer.Compare(Value, other.Value);
            if (result != 0) return result;

            return IsStart switch
            {
                true => Open ? 1 : -1,
                false => Open ? -1 : 1
            };
        }
    }
}