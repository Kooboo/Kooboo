using System;
using System.Linq;
using System.Runtime.Serialization;

namespace LibSass.Types
{
    public enum SassListSeparator
    {
        [EnumMember(Value = ",")]
        Comma,

        [EnumMember(Value = " ")]
        Space
    }

    public static class SassEnumExtensions
    {
        public static string ToEnumString<T>(this T instance)
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("Must be enum type", nameof(instance));

            string enumString = instance.ToString();
            var field = typeof(T).GetField(enumString);

            // instance can be a number that was cast to T, instead of a named value, or could be a combination of flags instead of a single value
            var attr = (EnumMemberAttribute)field?.GetCustomAttributes(typeof(EnumMemberAttribute), false).SingleOrDefault();

            if (attr != null) // if there's no EnumMember attr, use the default value
                enumString = attr.Value;

            return enumString;
        }
    }
}
