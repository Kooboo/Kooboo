using System;
using System.Text;

namespace Kooboo.Mail.Extension
{
    public static class StringExtension
    {
        private static readonly Random _random = new Random();

        public static bool MatchPrefix(this string source, string destination, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            return source.StartsWith(destination, comparison);
        }

        public static string Add0x20Bits(this string s)
        {
            char[] res = new char[s.Length];

            for (int i = 0; i < s.Length; i++)
            {
                bool isLower = _random.Next(0, 100) > 50;

                char current = s[i];

                if (!isLower && current is >= 'A' and <= 'Z')
                {
                    current = (char)(current + 0x20);
                }
                else if (isLower && current is >= 'a' and <= 'z')
                {
                    current = (char)(current - 0x20);
                }

                res[i] = current;
            }

            return new string(res);
        }

        public static int IndexOfWithQuoting(this string s, char value, int startIndex = 0)
        {
            var inQuote = false;

            for (var i = startIndex; i < s.Length; i++)
            {
                if (s[i] == '\\') // ignore escape char and escaped char
                {
                    i++;
                }
                else if (!inQuote && s[i] == value)
                {
                    return i;
                }
                else if (s[i] == '"')
                {
                    inQuote = !inQuote;
                }
            }

            return -1;
        }

        public static string FromMasterfileLabelRepresentation(this string s)
        {
            _ = s ?? throw new ArgumentNullException(nameof(s));

            var sb = new StringBuilder();

            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '\\')
                {
                    if (s.Length <= i + 1)
                    {
                        throw new FormatException("Escape character at end of string");
                    }

                    i++;
                    if (s[i] >= '0' && s[i] <= '9')
                    {
                        if (s.Length < i + 3)
                        {
                            throw new FormatException("Partial escape character at end of string");
                        }

                        sb.Append((char)byte.Parse(s.Substring(i, 3)));
                        i += 2;
                    }
                    else
                    {
                        sb.Append(s[i]);
                    }
                }
                else
                {
                    sb.Append(s[i]);
                }
            }

            return sb.ToString();
        }

        public static string ToMasterfileLabelRepresentation(this string s, bool encodeDots = false)
        {
            _ = s ?? throw new ArgumentNullException(nameof(s));

            var sb = new StringBuilder();

            for (var i = 0; i < s.Length; i++)
            {
                switch (s[i])
                {
                    case < ' ':
                    case > '\x7e':
                        sb.Append(@"\" + ((byte)s[i]).ToString("000"));
                        break;
                    case '"':
                    case '(':
                    case ')':
                    case ';':
                    case '\\':
                        sb.Append(@"\" + (char)s[i]);
                        break;
                    case '.':
                        sb.Append(encodeDots ? @"\." : ".");
                        break;
                    default:
                        sb.Append(s[i]);
                        break;
                }
            }

            return sb.ToString();
        }
    }
}

