//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Kooboo.Lib.Helper
{
    public static class StringHelper
    {
        public static bool ContainChinese(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;
            string pattern = "[\u4e00-\u9fbb]";
            return Regex.IsMatch(input, pattern);
        }

        public static string GetSummary(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            if (input.Length <= 50)
            {
                return input;
            }

            return input[..50] + "...";
        }

        public static string GetSummary(string input, int length)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            if (input.Length <= length)
            {
                return input;
            }

            return input[..length] + "...";
        }

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

        public static string GetUniqueBoundary()
        {
            return IDHelper.NewLongId().ToString();
        }

        public static string ToValidTextKey(string input)
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
            return sb.ToString();
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

        public static string ToValidUserNames(string input)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                var current = input[i];

                if (Helper.CharHelper.isAlphanumeric(current) || current == '_' || current == '-')
                {
                    sb.Append(current);
                }
                else
                {
                    //sb.Append('~'); 
                }
            }
            return sb.ToString();
        }

        public static bool IsValidUserName(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            for (int i = 0; i < input.Length; i++)
            {
                var current = input[i];

                if (Helper.CharHelper.isAlphanumeric(current) || current == '_' || current == '-')
                {
                    // continue; 
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsUserNamePrefixReserved(string username)
        {
            var lower = username.ToLower();
            if (lower.StartsWith("_mx") || lower.StartsWith("_dmarc") || lower.StartsWith("_acme-challenge") || lower.StartsWith("_domainkey"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///  check whether this is a string value or not. 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
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

            if (input.StartsWith('`') && input.EndsWith('`'))
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

        /// <summary>
        /// The space characters, for the purposes of this specification, are
        /// U+0020 SPACE, "tab" (U+0009), "LF" (U+000A), "FF" (U+000C), and "CR" (U+000D).
        /// </summary>
        /// <param name="chr"></param>
        /// <returns></returns>
        public static bool isSpaceCharacters(char chr)
        {
            return (chr == '\u0020' || chr == '\u0009' || chr == '\u000a' || chr == '\u000c' || chr == '\u000d');
        }


        public static string SementicSubString(string input, int start, int count)
        {
            int len = input.Length;

            if (start > len - 1)
            {
                return null;
            }

            var currentchar = input[start];
            while (isSpaceCharacters(currentchar))
            {
                start = start + 1;

                if (start > len - 1)
                {
                    return null;
                }
                currentchar = input[start];
            }

            if (start + count >= len - 1)
            {
                return input.Substring(start);
            }

            int i = start + count;

            while (i < len && IsNonBreakChar(input[i]))
            {
                i += 1;
            }

            return input.Substring(start, i - start);
        }

        private static bool IsNonBreakChar(char input)
        {
            if (input < 128)
            {
                return CharHelper.isAlphanumeric(input);
            }

            return true;
        }


        public static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }


        public static List<FindResult> FindText(
            string body,
            string keyword,
            bool isRegex = false,
            bool ignoreCase = true
            )
        {
            List<FindResult> result = new List<FindResult>();

            if (isRegex)
            {
                var matches = Regex.Matches(body, keyword, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
                foreach (Match item in matches)
                {
                    result.Add(GetFindResult(body, item.Index, item.Value));
                }
            }
            else
            {

                var seperators = " ,.;\r\n".ToCharArray();

                using (StringReader reader = new StringReader(body))
                {
                    string line;
                    int counter = 0;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var stringComparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
                        var index = line.IndexOf(keyword, stringComparison);
                        if (index > -1)
                        {
                            FindResult lineresult = new FindResult();
                            lineresult.LineNumber = counter;

                            int start = index - 20;
                            if (start < 0)
                            {
                                start = 0;
                            }
                            int startindex = start;

                            if (index < 10)
                            {
                                startindex = 0;
                            }
                            else
                            {
                                int findstartindex = line.IndexOfAny(seperators, start);
                                if (findstartindex > start && findstartindex < index)
                                {
                                    startindex = findstartindex;
                                }
                            }


                            int endindex = index + keyword.Length;
                            int next = endindex + 5;
                            if (next < line.Length)
                            {
                                int findendindex = line.IndexOfAny(seperators, next);
                                if (findendindex > next)
                                {
                                    next = findendindex;
                                }
                            }
                            else
                            {
                                next = line.Length;
                            }

                            var subsummary = line.Substring(startindex, next - startindex);
                            lineresult.Summary = subsummary;
                            result.Add(lineresult);
                        }

                        counter += 1;
                    }
                }
            }


            if (result.Any())
            {
                return result;
            }
            else
            {
                return null;
            }

        }

        private static FindResult GetFindResult(string content, int index, string matched)
        {
            int currentLine = 0;
            int summaryLeftOffset = 0;
            bool summaryLeftIndexIsFind = false;
            for (int i = index - 1; i >= 0; i--)
            {
                var @char = content[i];
                if (@char == '\n')
                {
                    currentLine++;
                    summaryLeftIndexIsFind = true;
                }

                if (!summaryLeftIndexIsFind)
                {
                    summaryLeftOffset++;
                }

                if (summaryLeftOffset >= 10)
                {
                    summaryLeftIndexIsFind = true;
                }
            }

            int summaryRightOffset = 0;

            for (int i = index + matched.Length; i < content.Length; i++)
            {
                var @char = content[i];
                if (@char == '\n' || summaryRightOffset >= 10)
                {
                    break;
                }
                summaryRightOffset++;
            }

            return new FindResult
            {
                LineNumber = currentLine,
                Summary = content.Substring(index - summaryLeftOffset, summaryLeftOffset + matched.Length + summaryRightOffset),
                Start = summaryLeftOffset,
                End = summaryLeftOffset + matched.Length
            };
        }
        public static bool IsBase64String(string base64)
        {
            if (base64 == null)
            {
                return false;
            }

            try
            {
                Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
                return Convert.TryFromBase64String(base64, buffer, out int bytesParsed);
            }
            catch (Exception)
            {

            }
            return false;
        }

        public static bool IsValidBracketKey(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            for (int i = 0; i < input.Length; i++)
            {
                var currentChar = input[i];
                if (!char.IsLetterOrDigit(currentChar) && currentChar != '-' && currentChar != '_' && currentChar != '.' && currentChar != '(' && currentChar != ')')
                {
                    return false;
                }
            }
            return true;
        }



        public static List<string> FindValues(string input, string start, char[] ends)
        {

            if (string.IsNullOrWhiteSpace(input) || string.IsNullOrEmpty(start))
            {
                return null;
            }

            List<string> result = new List<string>();

            int startLen = start.Length;
            int inputLen = input.Length;
            bool foundStart = false;
            string temp = null;

            for (int i = 0; i < input.Length; i++)
            {
                if (!foundStart)
                {
                    // find the start. 
                    if (input[i] == start[0])
                    {
                        var check = lookUp(i);
                        if (check == start)
                        {
                            foundStart = true;
                            temp = null;
                        }
                    }
                }
                else
                {
                    // find till the end...
                    if (ends.Contains(input[i]))
                    {
                        if (!string.IsNullOrEmpty(temp))
                        {
                            result.Add(temp);
                            foundStart = false;
                        }
                    }

                }

            }

            return result;

            string lookUp(int startI)
            {
                if (inputLen > startI + startLen)
                {
                    string found = "";
                    for (int i = 0; i < startLen; i++)
                    {
                        found += input[startI + i];
                    }

                    return found;
                }
                return null;
            }

        }

        public static string SplitToLines(string base64String, int lineLen = 76)
        {
            StringBuilder sb = new StringBuilder();

            int totalLen = base64String.Length;

            int currentIndex = 0;

            while (currentIndex < totalLen)
            {
                var nextIndex = currentIndex + lineLen;
                if (nextIndex >= totalLen)
                {
                    var sub = base64String.Substring(currentIndex);
                    sb.Append(sub);
                    currentIndex = nextIndex;
                    break;
                }
                else
                {
                    var sub = base64String.Substring(currentIndex, nextIndex - currentIndex);
                    sb.Append(sub);
                    sb.Append(Environment.NewLine);
                }
                currentIndex = nextIndex;
            }

            if (currentIndex < totalLen)
            {
                var sub = base64String.Substring(currentIndex, totalLen - currentIndex);

                sb.Append(sub);
            }

            return sb.ToString();
        }

        public static string[] SplitToParts(string text, int maxChar = 5000)
        {
            var fragments = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var result = new List<string>();
            var sb = new StringBuilder();
            for (int i = 0; i < fragments.Length; i++)
            {
                var fragment = fragments[i];

                if (i == fragments.Length - 1)
                {
                    sb.Append(fragment);
                    result.Add(sb.ToString());
                    break;
                }

                if (sb.Length + fragment.Length > maxChar)
                {
                    result.Add(sb.ToString());
                    sb.Clear();
                }
                sb.Append(fragment);
                sb.Append(' ');
            }

            return [.. result];
        }
    }

    public class FindResult
    {
        public int LineNumber { get; set; }

        public string Summary { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
    }
}
