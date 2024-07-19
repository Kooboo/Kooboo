//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Lib.Helper
{
    public static class CharHelper
    {

        //The White_Space characters are those that have the Unicode property "White_Space" in the Unicode PropList.txt data file. [UNICODE]
        //This should not be confused with the "White_Space" value (abbreviated "WS") of the "Bidi_Class" property in the Unicode.txt data file.
        //The control characters are those whose Unicode "General_Category" property has the value "Cc" in the Unicode UnicodeData.txt data file. [UNICODE]

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


        /// <summary>
        /// The lowercase ASCII letters are the characters in the range lowercase ASCII letters. a-z
        /// </summary>
        /// <param name="chr"></param>
        /// <returns></returns>
        public static bool isLowercaseAscii(char chr)
        {
            //a-z, ascii 61-122. 
            return (chr >= 97 && chr <= 122);
        }

        /// <summary>
        /// The uppercase ASCII letters are the characters in the range uppercase ASCII letters. A-Z
        /// </summary>
        /// <param name="chr"></param>
        /// <returns></returns>
        public static bool isUppercaseAscii(char chr)
        {
            return (chr >= 65 && chr <= 90);
        }

        public static bool IsAscii(char input)
        {
            return isUppercaseAscii(input) || isLowercaseAscii(input);
        }

        public static bool IsNumber(string numberString)
        {
            return isAsciiDigit(numberString);
        }

        public static bool isAsciiDigit(string numberstring)
        {
            if (string.IsNullOrEmpty(numberstring))
            {
                return false;
            }

            foreach (var item in numberstring.ToCharArray())
            {
                if (!isAsciiDigit(item))
                {
                    if (item != '.' && item != ',')
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// The ASCII digits are the characters in the range ASCII digits.
        /// </summary>
        /// <param name="chr"></param>
        /// <returns></returns>
        public static bool isAsciiDigit(char chr)
        {
            //0-9, acsii 48-57. 
            return (chr >= 48 && chr <= 57);
        }

        /// <summary>
        /// The alphanumeric ASCII characters are those that are either uppercase ASCII letters, lowercase ASCII letters, or ASCII digits.
        /// </summary>
        /// <param name="chr"></param>
        /// <returns></returns>
        public static bool isAlphanumeric(char chr)
        {
            return (isUppercaseAscii(chr) || isLowercaseAscii(chr) || isAsciiDigit(chr));
        }

        /// <summary>
        /// The ASCII hex digits are the characters in the ranges ASCII digits, U+0041 LATIN CAPITAL LETTER A to U+0046 LATIN CAPITAL LETTER F, and U+0061 LATIN SMALL LETTER A to U+0066 LATIN SMALL LETTER F.
        /// </summary>
        /// <param name="chr"></param>
        public static bool isAsciiHexDigit(char chr)
        {
            if (isAsciiDigit(chr))
            {
                return true;
            }

            // if (chr >= 41 && chr <= 46)
            if (chr >= 65 && chr <= 70)
            {
                return true;
            }

            if (chr >= 97 && chr <= 102)
            //  if (chr >= 61 && chr <= 66)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// The uppercase ASCII hex digits are the characters in the ranges ASCII digits and U+0041 LATIN CAPITAL LETTER A to U+0046 LATIN CAPITAL LETTER F only.
        /// </summary>
        /// <param name="chr"></param>
        /// <returns></returns>
        public static bool isUppercaseAsciiHexDigit(char chr)
        {
            if (isAsciiDigit(chr))
            {
                return true;
            }

            // if (chr >= 41 && chr <= 46)
            if (chr >= 65 && chr <= 70)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// The lowercase ASCII hex digits are the characters in the ranges ASCII digits and U+0061 LATIN SMALL LETTER A to U+0066 LATIN SMALL LETTER F only.
        /// </summary>
        /// <param name="chr"></param>
        /// <returns></returns>
        public static bool isLowercaseAsciiHexDigits(char chr)
        {
            if (isAsciiDigit(chr))
            {
                return true;
            }

            //if (chr >= 61 && chr <= 66)
            if (chr >= 97 && chr <= 102)
            {
                return true;
            }

            return false;
        }
    }
}
