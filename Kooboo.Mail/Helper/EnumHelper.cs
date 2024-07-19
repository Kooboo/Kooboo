using System;
using System.Collections.Generic;

namespace Kooboo.Mail.Helper
{
    public static class EnumHelper<T>
        where T : struct, System.Enum
    {
        private static readonly Dictionary<T, string> _names;
        private static readonly Dictionary<string, T> _values;

        static EnumHelper()
        {
            string[] names = System.Enum.GetNames(typeof(T));
            T[] values = (T[])System.Enum.GetValues(typeof(T));

            _names = new Dictionary<T, string>(names.Length);
            _values = new Dictionary<string, T>(names.Length * 2);

            for (int i = 0; i < names.Length; i++)
            {
                _names[values[i]] = names[i];
                _values[names[i]] = values[i];
                _values[names[i].ToLower()] = values[i];
            }
        }

        public static bool TryParse(string s, bool ignoreCase, out T value)
        {
            if (string.IsNullOrEmpty(s))
            {
                value = default;
                return false;
            }

            return _values!.TryGetValue(ignoreCase ? s.ToLower() : s, out value);
        }

        public static T Parse(string s, bool ignoreCase)
        {
            if (TryParse(s, ignoreCase, out var res))
                return res;
            throw new ArgumentOutOfRangeException(nameof(s));
        }

        public static T Parse(string s, bool ignoreCase, T defaultValue)
        {
            return TryParse(s, ignoreCase, out var res) ? res : defaultValue;
        }

        public static string ToString(T value)
        {
            return _names!.TryGetValue(value, out var res) ? res : Convert.ToInt64(value).ToString();
        }
    }
}

