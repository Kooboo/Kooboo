//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.IndexedDB.Helper
{
    public static class StringHelper
    {
        public static bool IsSameValue(string x, string y)
        {
            if (string.IsNullOrWhiteSpace(x) && string.IsNullOrWhiteSpace(y))
            {
                return true;
            }

            if (string.IsNullOrWhiteSpace(x) || string.IsNullOrWhiteSpace(y))
            {
                return false;
            }
            return x.Trim().ToLower() == y.Trim().ToLower();
        }

        public static string ToValidFileName(string input)
        {
            if (string.IsNullOrEmpty(input))
            { return input; }

            StringBuilder sb = new StringBuilder();

            char[] invalid = System.IO.Path.GetInvalidFileNameChars();

            for (int i = 0; i < input.Length; i++)
            {
                if (!invalid.Contains(input[i]) && input[i] != '#')
                {
                    sb.Append(input[i]);
                }
            }
            string result = sb.ToString();

            if (!string.IsNullOrEmpty(result))
            {
                result = result.Replace("  ", " ");
                return result.Replace(" ", "_");
            }
            return string.Empty;
        }


        public static bool IsString(string input)
        {
            input = input.Trim();

            if (input.StartsWith("\"") && input.EndsWith("\""))
            {
                return true;
            }

            if (input.StartsWith("'") && input.EndsWith("'"))
            {
                return true;
            }

            return false;
        }

        public static string ReplaceIgnoreCase(string input, string oldValue, string newValue)
        {
            int index = input.IndexOf(oldValue, StringComparison.CurrentCultureIgnoreCase);

            if (index != -1)
            {
                string newstring = input.Substring(0, index);
                newstring += newValue;
                index = index + oldValue.Length;

                newstring += input.Substring(index);
                return newstring;
            }

            return input;
        }

        public static string TrimSpace(string input)
        {
            return input.Trim(SpaceChars);
        }

        private static char[] _space;
        private static char[] SpaceChars
        {
            /// The space characters, for the purposes of this specification, are
            /// U+0020 SPACE, "tab" (U+0009), "LF" (U+000A), "FF" (U+000C), and "CR" (U+000D). 
            get
            {
                if (_space == null)
                {
                    List<char> spacelist = new List<char>();
                    spacelist.Add('\u0020');
                    spacelist.Add('\u0009');
                    spacelist.Add('\u000a');
                    spacelist.Add('\u000c');
                    spacelist.Add('\u000d');
                    _space = spacelist.ToArray();
                }
                return _space;
            }
        }

        public static bool isSpaceCharacters(char chr)
        {
            return (chr == '\u0020' || chr == '\u0009' || chr == '\u000a' || chr == '\u000c' || chr == '\u000d');
        }


        public static bool Compare(Kooboo.IndexedDB.Query.Comparer Compare, string CurrentValue, string TargetValue)
        {
            if (TargetValue == null || CurrentValue == null)
            {
                return (TargetValue == null && CurrentValue == null);
            }

            switch (Compare)
            {
                case Query.Comparer.EqualTo:
                    return string.Compare(CurrentValue, TargetValue) == 0;

                case Query.Comparer.GreaterThan:
                    return string.Compare(CurrentValue, TargetValue) > 0;

                case Query.Comparer.GreaterThanOrEqual:
                    return string.Compare(CurrentValue, TargetValue) >= 0;

                case Query.Comparer.LessThan:
                    return string.Compare(CurrentValue, TargetValue) < 0;

                case Query.Comparer.LessThanOrEqual:
                    return string.Compare(CurrentValue, TargetValue) <= 0;

                case Query.Comparer.NotEqualTo:

                    return string.Compare(CurrentValue, TargetValue) != 0;

                case Query.Comparer.StartWith:
                    {
                        return CurrentValue.StartsWith(TargetValue);
                    }

                case Query.Comparer.Contains:
                    {
                        return CurrentValue.Contains(TargetValue); 
                    }

                default:
                    return false;
            }

        }

    }
}
