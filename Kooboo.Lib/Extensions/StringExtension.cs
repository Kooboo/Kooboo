//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Kooboo.Extensions
{
    public static class StringExtension
    {
        public static bool IsFont(this string urlinput)
        {
            string extension = Kooboo.Lib.Helper.UrlHelper.FileExtension(urlinput);

            if (string.IsNullOrEmpty(extension))
            {
                return false;
            }
            else
            {
                return IsFontExtension(extension);
            }
        }


        private static bool IsFontExtension(this string fileExtension)
        {
            if (string.IsNullOrEmpty(fileExtension))
            {
                return false;
            }

            fileExtension = fileExtension.ToLower();

            if (fileExtension.Contains("tt"))
            {
                return true;
            }

            if (fileExtension.Contains("svg#"))
            {
                return true;
            }

            if (fileExtension.Contains("eot"))
            {
                return true;
            }

            if (fileExtension.Contains("woff"))
            {
                return true;
            }

            return false;
        }

        public static Guid ToHashGuid(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                input = string.Empty;
            }
            input = input.ToLower();

            byte[] bytes = Encoding.UTF8.GetBytes(input);

            // create the md5 hash
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(bytes);

            // convert the hash to a Guid
            return new Guid(data);
        }


        public static List<string> SplitToList(this string input, string seperator)
        {
            List<string> result = new List<string>();

            if (string.IsNullOrEmpty(input))
            {
                return result;
            }

            int totallen = input.Length;
            int seperatorlen = seperator.Length;

            int currentposition = 0;
            int currentindex = input.IndexOf(seperator, StringComparison.OrdinalIgnoreCase);

            while (currentindex >= 0)
            {
                int len = currentindex - currentposition;
                if (len > 0)
                {
                    result.Add(input.Substring(currentposition, len));
                }
                else
                {
                    result.Add("");
                }

                currentposition = currentindex + seperatorlen;

                if (currentposition > totallen - 1)
                {
                    break;
                }
                currentindex = input.IndexOf(seperator, currentposition, StringComparison.OrdinalIgnoreCase);
            }

            if (totallen > currentposition)
            {
                result.Add(input.Substring(currentposition));
            }

            return result;

        }

        public static bool ContainsAllParts(this string CurrentSegment, List<string> PartialParts)
        {
            foreach (var item in PartialParts)
            {
                if (string.IsNullOrEmpty(item))
                {
                    continue;
                }

                var index = CurrentSegment.IndexOf(item, StringComparison.OrdinalIgnoreCase);
                if (index == -1)
                {
                    return false;
                }

                if (index + item.Length > CurrentSegment.Length)
                {
                    CurrentSegment = string.Empty;
                }
                else
                {
                    CurrentSegment = CurrentSegment.Substring(index + item.Length);
                }
            }

            return true;
        }


        public static string RemoveRoutingCurlyBracket(this string input)
        {
            int leftindex = input.IndexOf("{");
            if (leftindex >= 0)
            {
                int rightindex = input.IndexOf("}", leftindex);
                if (rightindex >= 0)
                {
                    return input.Substring(0, leftindex + 1) + input.Substring(rightindex);
                }

                return input;
            }

            return input;
        }

        public static string RemoveCurlyBracketContent(this string input)
        {
            string pattern = "{.*?}";
            string back = Regex.Replace(input, pattern, "{}");
            return back;
        }

        public static bool IsAsciiDigit(this string numberstring)
        {
            if (string.IsNullOrEmpty(numberstring))
            {
                return false;
            }

            return numberstring.ToCharArray().Where(item => !IsAsciiDigit(item))
                .All(item => item == '.' || item == ',');
        }

        private static bool IsAsciiDigit(char chr)
        {
            //0-9, acsii 48-57. 
            return (chr >= 48 && chr <= 57);
        }

        /// <summary>
        /// get the prefix digit part of a number string, like 15px; 
        /// </summary>
        /// <param name="numberstring"></param>
        /// <returns></returns>
        public static int GetDigitPart(this string numberstring)
        {
            string digit = string.Empty;

            for (int i = 0; i < numberstring.Length; i++)
            {
                if (IsAsciiDigit(numberstring[i]))
                {
                    digit += numberstring[i].ToString();
                }
                else
                {
                    break;
                }
            }

            if (string.IsNullOrEmpty(digit))
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(digit);
            }
        }

        public static bool EqualsOrNullEmpty(this string str1, string str2, StringComparison comparisonType)
        {
            return String.Compare(str1 ?? "", str2 ?? "", comparisonType) == 0;
        }


        public static Position GetPosition(this string input, int index)
        {
            if (index <= 0)
            {
                return null;
            }

            if (index > input.Length)
            {
                return null;
            }

            int linecount = 0;
            int columncount = 0;

            for (int i = 0; i < index; i++)
            {
                var current = input[i];
                if (current == '\n')
                {
                    linecount += 1;
                    columncount = 0;
                }
                else
                {
                    columncount += 1;
                }
            }

            Position pos = new Position();
            pos.Line = linecount;
            pos.Column = columncount;

            return pos;
        }


        public static string ToValidPath(this string input)
        {
            StringBuilder sb = new StringBuilder();

            char[] invalidpath = System.IO.Path.GetInvalidPathChars();

            for (int i = 0; i < input.Length; i++)
            {
                if (!invalidpath.Contains(input[i]))
                {
                    sb.Append(input[i]);
                }
            }

            return sb.ToString();
        }

        public static bool isInvalidPath(this string input)
        {
            char[] invalidpath = System.IO.Path.GetInvalidPathChars();

            for (int i = 0; i < input.Length; i++)
            {
                if (invalidpath.Contains(input[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool isInvalidFileName(this string input)
        {
            char[] invalid = System.IO.Path.GetInvalidFileNameChars();

            for (int i = 0; i < input.Length; i++)
            {
                if (invalid.Contains(input[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public static string ToCamelCaseName(this string pascalCaseName)
        {
            return pascalCaseName[0].ToString().ToLower() + pascalCaseName.Substring(1);
        }
    }

    public class Position
    {
        public int Line { get; set; }
        public int Column { get; set; }
    }
}