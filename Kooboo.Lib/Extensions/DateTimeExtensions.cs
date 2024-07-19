namespace System
{
    public static class DateTimeExtensions
    {
        public static DateTime ToUtcKind(this DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTimeToUtc(dateTime);
        }

        public static long ToUnixEpochTicks(this DateTime dateTime)
        {
            return dateTime.Ticks - DateTime.UnixEpoch.Ticks;
        }
    }
}
