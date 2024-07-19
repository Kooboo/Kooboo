//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Linq;
using System.Text;

namespace Kooboo.IndexedDB
{
    public static class StringExtension
    {

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

        public static string ToValidFileName(this string input)
        {
            StringBuilder sb = new StringBuilder();

            char[] invalid = System.IO.Path.GetInvalidFileNameChars();

            for (int i = 0; i < input.Length; i++)
            {
                if (!invalid.Contains(input[i]))
                {
                    sb.Append(input[i]);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// generate a constant hashcode.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int GetHashCode32(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                s = " ";
            }
            s = s.ToLower();

            var chars = s.ToCharArray();
            var lastCharInd = chars.Length - 1;
            var num1 = 0x15051505;
            var num2 = num1;
            var ind = 0;
            while (ind <= lastCharInd)
            {
                var ch = chars[ind];
                var nextCh = ++ind > lastCharInd ? '\0' : chars[ind];
                num1 = (((num1 << 5) + num1) + (num1 >> 0x1b)) ^ (nextCh << 16 | ch);
                if (++ind > lastCharInd) break;
                ch = chars[ind];
                nextCh = ++ind > lastCharInd ? '\0' : chars[ind++];
                num2 = (((num2 << 5) + num2) + (num2 >> 0x1b)) ^ (nextCh << 16 | ch);
            }
            return num1 + num2 * 0x5d588b65;

        }

        // case insensitive compare values.
        public static bool IsSameValue(this string x, string y)
        {
            if (string.IsNullOrEmpty(x) && string.IsNullOrEmpty(y))
            {
                return true;
            }
            if (string.IsNullOrEmpty(x) || string.IsNullOrEmpty(y))
            {
                return false;
            }
            return x.ToLower() == y.ToLower();
        }

    }
}
