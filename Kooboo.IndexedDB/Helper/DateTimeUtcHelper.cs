using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.IndexedDB.Helper
{
    public class DateTimeUtcHelper
    {
        private static int UtcFlagInt = 128;//biggest bit(128) as the utc tag,2^7

        public static byte[] ToBytes(DateTime time)
        {
            var bytes = BitConverter.GetBytes(time.Ticks);
            AddUtcBit(bytes, time);

            return bytes;
        }

        public static DateTime ToDateTime(byte[] bytes)
        {
            var isUtc= RemoveUtcBit(bytes);
            var datetick = BitConverter.ToInt64(bytes, 0);

            return isUtc ? new DateTime(datetick, DateTimeKind.Utc)
                        : new DateTime(datetick);
        }

        private static void AddUtcBit(byte[] bytes,DateTime time)
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
    }
}
