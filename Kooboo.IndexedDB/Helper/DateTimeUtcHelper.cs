using System;

namespace Kooboo.IndexedDB.Helper
{
    // This is kept here because of a bug... of wrong datetime implementation...
    public class DateTimeUtcHelper
    {
        private static int UtcFlagInt = 128;//biggest bit(128) as the utc tag,2^7

        public static byte[] ToBytes(DateTime time)
        {
            var bytes = BitConverter.GetBytes(time.Ticks);
            //AddUtcBit(bytes, time); 
            return bytes;
        }

        public static DateTime ToDateTime(byte[] bytes)
        {
            // var isUtc= RemoveUtcBit(bytes);
            var datetick = BitConverter.ToInt64(bytes, 0);

            if (datetick < DateTime.MinValue.Ticks || datetick > DateTime.MaxValue.Ticks)
            {
                // this might be the bug...
                if (RemoveUtcBit(bytes))
                {
                    datetick = BitConverter.ToInt64(bytes, 0);
                    if (datetick >= DateTime.MinValue.Ticks && datetick <= DateTime.MaxValue.Ticks)
                    {
                        return new DateTime(datetick);
                    }
                }
            }
            else
            {
                return new DateTime(datetick);
            }

            return default(DateTime);
            // return isUtc ? new DateTime(datetick, DateTimeKind.Utc) : new DateTime(datetick);
        }

        private static void AddUtcBit(byte[] bytes, DateTime time)
        {
            if (time.Kind == DateTimeKind.Utc)
            {
                var lastByte = bytes[bytes.Length - 1];
                //use biggest bit(128) as the utc tag
                bytes[bytes.Length - 1] = (byte)(lastByte + UtcFlagInt);
            }
        }

        private static bool RemoveUtcBit(byte[] bytes)
        {
            var lastByte = bytes[bytes.Length - 1];
            var isUtc = lastByte > UtcFlagInt;
            if (isUtc)
            {
                bytes[bytes.Length - 1] = (byte)(lastByte - UtcFlagInt);
            }

            return isUtc;
        }

        public static bool Compare(Kooboo.IndexedDB.Query.Comparer Compare, DateTime DateA, DateTime DateB)
        {
            switch (Compare)
            {
                case Query.Comparer.EqualTo:
                    return EqualDate(DateA, DateB);

                case Query.Comparer.GreaterThan:
                    return DateA > DateB;

                case Query.Comparer.GreaterThanOrEqual:
                    return DateA >= DateB || EqualDate(DateA, DateB);

                case Query.Comparer.LessThan:
                    return DateA < DateB;

                case Query.Comparer.LessThanOrEqual:
                    return DateA <= DateB || EqualDate(DateA, DateB);

                case Query.Comparer.NotEqualTo:

                    return !EqualDate(DateA, DateB);

                default:
                    return false;
            }

        }

        public static bool EqualDate(DateTime DateA, DateTime DateB)
        {
            if (DateA.Year == DateB.Year && DateA.DayOfYear == DateB.DayOfYear && DateA.Hour == DateB.Hour && DateA.Minute == DateB.Minute && DateA.Second == DateB.Second)
            {
                return true;
            }
            return false;
        }
    }
}
