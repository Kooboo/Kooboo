//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kooboo.Lib.Helper
{
    public static class StringHelper
    {

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

            return input.Substring(0, 50) + "...";
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
               
                if (start > len -1)
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
             
             while(i< len && IsNonBreakChar(input[i]))
            {
                i += 1; 
            }

            return input.Substring(start, i - start);  
        }

        private static bool IsNonBreakChar(char input)
        { 
            if (input < 128)
            {
                return  CharHelper.isAlphanumeric(input); 
            }

            return true;  
        }


        public static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }



    }
}
