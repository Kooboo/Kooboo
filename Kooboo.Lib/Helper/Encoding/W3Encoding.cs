//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Lib.Helper
{
    public class W3Encoding
    {
        public static string SystemDefaultEncoding = "UTF-8";

        public static Encoding SystemDefault = System.Text.Encoding.GetEncoding(SystemDefaultEncoding);

        private static Dictionary<string, string> _defaultEncoding;

        /// https://dev.w3.org/html5/spec-preview/parsing.html#concept-get-attributes-when-sniffing 
        public static Dictionary<string, string> DefaultEncodingSet
        {
            get
            {
                if (_defaultEncoding == null)
                {
                    _defaultEncoding = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                    _defaultEncoding.Add("ar", "UTF-8");
                    _defaultEncoding.Add("be", "ISO-8859-5");
                    _defaultEncoding.Add("bg", "windows-1251");
                    _defaultEncoding.Add("cs", "ISO-8859-2");
                    _defaultEncoding.Add("cy", "UTF-8");
                    _defaultEncoding.Add("fa", "UTF-8");
                    _defaultEncoding.Add("he", "windows-1255");
                    _defaultEncoding.Add("hr", "UTF-8");
                    _defaultEncoding.Add("hu", "ISO-8859-2");
                    _defaultEncoding.Add("ja", "Windows-31J");
                    _defaultEncoding.Add("kk", "UTF-8");
                    _defaultEncoding.Add("ko", "windows-949");
                    _defaultEncoding.Add("ku", "windows-1254");
                    _defaultEncoding.Add("lt", "windows-1257");
                    _defaultEncoding.Add("lv", "ISO-8859-13");
                    _defaultEncoding.Add("mk", "UTF-8");
                    _defaultEncoding.Add("or", "UTF-8");
                    _defaultEncoding.Add("pl", "ISO-8859-2");
                    _defaultEncoding.Add("ro", "UTF-8");
                    _defaultEncoding.Add("ru", "windows-1251");
                    _defaultEncoding.Add("sk", "windows-1250");
                    _defaultEncoding.Add("sl", "ISO-8859-2");
                    _defaultEncoding.Add("sr", "UTF-8");
                    _defaultEncoding.Add("th", "windows-874");
                    _defaultEncoding.Add("uk", "windows-1251");
                    _defaultEncoding.Add("vi", "UTF-8");
                    _defaultEncoding.Add("zh-CN", "GB18030");
                    _defaultEncoding.Add("zh-TW", "Big5");
                    _defaultEncoding.Add("", SystemDefaultEncoding);

                }
                return _defaultEncoding;
            }
        }

        public static string GetDefaultEncoding(string locale)
        {
            if (locale == null)
            {
                locale = "";
            }
            string encoding;

            if (DefaultEncodingSet.TryGetValue(locale, out encoding))
            {
                return encoding;
            }

            return null;

        }

        public static Encoding PreScanEncoding(byte[] input)
        {
            int len = input.Length;
            if (len > 1024)
            {
                len = 1024;  /// scan the first 1024 bytes as suggested. 
            }

            Func<int, byte> GetByte = (index) =>
            {
                if (index >= len)
                {
                    return 0;
                }
                return input[index];
            };

            //  When an algorithm requires a user agent to prescan a byte stream to determine its encoding, given some defined end condition, then it must run the following steps. These steps either abort unsuccessfully or return a character encoding.

            //Let position be a pointer to a byte in the input byte stream, initially pointing at the first byte.If at any point during these steps the user agent either runs out of bytes or reaches its end condition, then abort the prescan a byte stream to determine its encoding algorithm unsuccessfully.

            int position = 0;

        LOOP:
            //A sequence of bytes starting with: 0x3C 0x21 0x2D 0x2D(ASCII '<!--')
            if (GetByte(position) == 0x3C && GetByte(position + 1) == 0x21 && GetByte(position + 2) == 0x2D && GetByte(position + 3) == 0x2D)
            {
                //Advance the position pointer so that it points at the first 0x3E byte which is preceded by two 0x2D bytes(i.e.at the end of an ASCII '-->' sequence) and comes after the 0x3C byte that was found. (The two 0x2D bytes can be the same as the those in the '<!--' sequence.)
                while (true)
                {
                    position += 1;
                    if (GetByte(position) == 0x2D && GetByte(position + 1) == 0x2D && GetByte(position + 2) == 0x3E)
                    {
                        position = position + 3;
                        break;
                    }

                    if (position >= len)
                    {
                        return null;
                    }
                }

            }

            //A sequence of bytes starting with: 0x3C(<), 0x4D<m> or 0x6D(M), 0x45 or 0x65, 0x54 or 0x74, 0x41 or 0x61, and one of 0x09, 0x0A, 0x0C, 0x0D, 0x20, 0x2F(case-insensitive ASCII '<meta' followed by a space or slash) 
            if (GetByte(position) == 0x3C && (GetByte(position + 1) == 0x4D || GetByte(position + 1) == 0x6D)
                && (GetByte(position + 2) == 0x45 || GetByte(position + 2) == 0x65) && (GetByte(position + 3) == 0x54 || GetByte(position + 3) == 0x74) && (GetByte(position + 4) == 0x41 || GetByte(position + 4) == 0x61))
            {
                var follow = GetByte(position + 5);
                if (follow == 0x09 || follow == 0x0A || follow == 0x0C || follow == 0x0D || follow == 0x20 || follow == 0x2F)
                {
                    //Advance the position pointer so that it points at the next 0x09, 0x0A, 0x0C, 0x0D, 0x20, or 0x2F byte (the one in sequence of characters matched above).
                    position = position + 5;

                    //Let attribute list be an empty list of strings. 
                    //Let got pragma be false. 
                    //Let need pragma be null. 
                    //Let charset be the null value(which, for the purposes of this algorithm, is distinct from an unrecognised encoding or the empty string).
                    Dictionary<string, string> AttributeList = new Dictionary<string, string>();
                    bool GotPragma = false;
                    bool NeedPragma = false;
                    bool NeedPragmaIsNull = true;
                    string CharSet = null;

                Attributes:

                    //6. Attributes: Get an attribute and its value. If no attribute was sniffed, then jump to the processing step below. 
                    var attributevalue = GetAttribute(ref input, ref position, len);

                    if (attributevalue == null || (string.IsNullOrEmpty(attributevalue.AttributeName) && string.IsNullOrEmpty(attributevalue.AttributeValue)))
                    {
                        goto Processing;
                    }

                    //If the attribute's name is already in attribute list, then return to the step labeled attributes.
                    if (AttributeList.ContainsKey(attributevalue.AttributeName))
                    {
                        goto Attributes;
                    }
                    //Add the attribute's name to attribute list.
                    AttributeList.Add(attributevalue.AttributeName, attributevalue.AttributeValue);

                    //Run the appropriate step from the following list, if one applies:

                    //If the attribute's name is "http-equiv"
                    //If the attribute's value is "content-type", then set got pragma to true.
                    if (attributevalue.AttributeName == "http-equiv" && attributevalue.AttributeValue == "content-type")
                    {
                        GotPragma = true;

                    }

                    //If the attribute's name is "content"
                    //Apply the algorithm for extracting an encoding from a meta element, giving the attribute's value as the string to parse. If an encoding is returned, and if charset is still set to null, let charset be the encoding returned, and set need pragma to true.
                    if (attributevalue.AttributeName == "content")
                    {
                        var ExtractedEncoding = ExtractCharset(attributevalue.AttributeValue);
                        if (!string.IsNullOrEmpty(ExtractedEncoding) && string.IsNullOrEmpty(CharSet))
                        {
                            CharSet = ExtractedEncoding;
                            NeedPragma = true;
                            NeedPragmaIsNull = false;
                        }
                    }

                    //If the attribute's name is "charset"
                    //Let charset be the encoding corresponding to the attribute's value, and set need pragma to false.
                    if (attributevalue.AttributeName == "charset")
                    {
                        CharSet = attributevalue.AttributeValue;
                        NeedPragma = false;
                        NeedPragmaIsNull = false;
                    }

                    //Return to the step labeled attributes.
                    goto Attributes;

                Processing:
                    {

                        //  Processing:
                        // If need pragma is null, then jump to the step below labeled next byte.
                        //  If need pragma is true but got pragma is false, then jump to the step below labeled next byte.
                        if (NeedPragmaIsNull || (NeedPragma && !GotPragma))
                        {
                            goto NextByte;
                        }

                        // If charset is a UTF - 16 encoding, change the value of charset to UTF - 8.
                        //The term a UTF-16 encoding refers to any variant of UTF-16: self-describing UTF-16 with a BOM, ambiguous UTF-16 without a BOM, raw UTF-16LE, and raw UTF-16BE.
                        if (!string.IsNullOrEmpty(CharSet))
                        {
                            var lower = CharSet.ToLower();
                            if (lower == "utf-16" || lower == "utf-16le" || lower == "utf-16be")
                            {
                                CharSet = "UTF-8";
                            }

                            //   If charset is not a supported character encoding, then jump to the step below labeled next byte. 
                            // Abort the prescan a byte stream to determine its encoding algorithm, returning the encoding given by charset.
                            var encoding = System.Text.Encoding.GetEncoding(CharSet);
                            if (encoding == null)
                            {
                                goto NextByte;
                            }

                            return encoding;
                        }


                    }


                }

            }

            //A sequence of bytes starting with: 0x3C 0x21(ASCII '<!')
            //A sequence of bytes starting with: 0x3C 0x2F(ASCII '</')
            //A sequence of bytes starting with: 0x3C 0x3F(ASCII '<?')
            //Advance the position pointer so that it points at the first 0x3E byte (ASCII >) that comes after the 0x3C byte that was found.
            if ((GetByte(position) == 0x3C && GetByte(position + 1) == 0x21) || (GetByte(position) == 0x3C && GetByte(position + 1) == 0x2F) || (GetByte(position) == 0x3C && GetByte(position + 1) == 0x3F))
            {
                position += 1;
                while (true)
                {
                    position += 1;
                    if (GetByte(position) == 0x3E || position > len)
                    {
                        break;
                    }
                }
            }


        NextByte:

            position += 1;
            if (position >= len)
            {
                return null;
            }
            goto LOOP;

        }

        private static byte GetByte(ref byte[] input, int index, int len)
        {
            if (index > len)
            {
                return 0;
            }
            return input[index];
        }


        //  2.7.4 Extracting encodings from meta elements 
        //The algorithm for extracting an encoding from a meta element, given a string s, is as follows.It either returns an encoding or nothing.
        public static String ExtractCharset(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return null;
            }
            int len = s.Length;

            //Let position be a pointer into s, initially pointing at the start of the string. 
            int position = 0;
        //Loop: Find the first seven characters in s after position that are an ASCII case-insensitive match for the word "charset".If no such match is found, return nothing and abort these steps.
        Loop:
            position = s.IndexOf("charset", position);
            if (position < 0)
            {
                return null;
            }
            else
            {
                position = position + 7;
            }

            // Skip any U + 0009, U + 000A, U + 000C, U + 000D, or U+0020 characters that immediately follow the word "charset"(there might not be any). 
            if (position >= len) { return null; }
            char currentchar = s[position];
            while (currentchar == '\u0009' || currentchar == '\u000A' || currentchar == '\u000C' || currentchar == '\u000D' || currentchar == '\u0020')
            {
                position += 1;
                if (position >= len) { return null; }
                currentchar = s[position];
            }

            //If the next character is not a "="(U + 003D), then move position to point just before that next character, and jump back to the step labeled loop.
            if (currentchar != '\u003D')
            {
                goto Loop;
            }

            /// Skip any U + 0009, U + 000A, U + 000C, U + 000D, or U+0020 characters that immediately follow the equals sign(there might not be any).

            position += 1;
            if (position >= len) { return null; }
            currentchar = s[position];
            while (currentchar == '\u0009' || currentchar == '\u000A' || currentchar == '\u000C' || currentchar == '\u000D' || currentchar == '\u0020')
            {
                position += 1;
                if (position >= len) { return null; }
                currentchar = s[position];
            }

            /// Process the next character as follows:

            //If it is a """ (U+0022) and there is a later """(U + 0022) in s
            // If it is a "'"(U + 0027) and there is a later "'"(U + 0027) in s
            if (currentchar == '\u0022' || currentchar == '\u0027')
            {
                string charset = string.Empty;
                var end = currentchar;
                position += 1;
                if (position >= len) { return null; }
                currentchar = s[position];
                while (currentchar != end)
                {
                    charset += currentchar.ToString();
                    position += 1;
                    if (position >= len) { return null; }
                    currentchar = s[position];
                }
                return charset;
                //   Return the encoding corresponding to the string between this character and the next earliest occurrence of this character.
                //   If it is an unmatched """ (U+0022)
                //If it is an unmatched "'"(U + 0027)
                //If there is no next character
                //Return nothing.
            }
            else
            {
                //Otherwise Return the encoding corresponding to the string from this character to the first U + 0009, U + 000A, U + 000C, U + 000D, U + 0020, or U+003B character or the end of s, whichever comes first.
                string charset = string.Empty;

                while (true)
                {
                    if (currentchar == '\u0009' || currentchar == '\u000A' || currentchar == '\u000C' || currentchar == '\u000D' || currentchar == '\u0020' || currentchar == '\u003B')
                    {
                        return charset;
                    }
                    charset += currentchar.ToString();
                    position += 1;
                    if (position >= len) { return charset; }
                    currentchar = s[position];
                }
            }


        }

        public static bool IsValidEncoding(string EncodingName)
        {
            return System.Text.Encoding.GetEncoding(EncodingName) != null;
        }

        public static Encoding UnicodeBom(byte[] input)
        {
            // For each of the rows in the following table, starting with the first one and going down, if there are as many or more bytes available than the number of bytes in the first column, and the first bytes of the file match the bytes given in the first column, then return the encoding given in the cell in the second column of that row, with the confidence certain, and abort these steps:

            //Bytes in Hexadecimal Encoding
            //FE FF   Big - endian UTF - 16
            //FF FE   Little - endian UTF - 16
            //EF BB BF UTF-8
            //This step looks for Unicode Byte Order Marks(BOMs).

            Encoding encoding = null;
            if (input[0] == 0xFF && input[1] == 0xFF)
            {
                encoding = Encoding.BigEndianUnicode;
            }
            else if (input[0] == 0xFF && input[0] == 0xFF)
            {
                encoding = Encoding.UTF8;
            }
            else if (input[0] == 0xFF && input[1] == 0xBB && input[2] == 0xBF)
            {
                encoding = encoding = Encoding.UTF8;
            }

            // Due to some description from w3, we need to change UTF-16 into UTF8. 
            if (encoding != null)
            {
                return Encoding.UTF8;
            }
            return null;
        }

        public static Attribute GetAttribute(ref byte[] Input, ref int Index, int Len)
        {

            /// 1. If the byte at position is one of 0x09(ASCII TAB), 0x0A(ASCII LF), 0x0C(ASCII FF), 0x0D(ASCII CR), 0x20(ASCII space), or 0x2F(ASCII /) then advance position to the next byte and redo this step.
            var currentbyte = GetByte(ref Input, Index, Len);
            while (currentbyte == 0x09 || currentbyte == 0x0A || currentbyte == 0x0C || currentbyte == 0x0D || currentbyte == 0x20 || currentbyte == 0x2F)
            {
                Index += 1;
                currentbyte = GetByte(ref Input, Index, Len);
            }

            // 2. If the byte at position is 0x3E(ASCII >), then abort the get an attribute algorithm.There isn't one.
            if (currentbyte == 0x3E)
            {
                return null;
            }

            /// 3.   Otherwise, the byte at position is the start of the attribute name.Let attribute name and attribute value be the empty string.
            string AttributeName = string.Empty;
            string AttributeValue = string.Empty;

        ///  4.Attribute name: Process the byte at position as follows:

        GetAttributeName:
            //If it is 0x3D(ASCII =), and the attribute name is longer than the empty string
            //Advance position to the next byte and jump to the step below labeled value.

            if (currentbyte == 0x3D && !string.IsNullOrEmpty(AttributeName))
            {
                Index += 1;
                currentbyte = GetByte(ref Input, Index, Len);
                goto AttributeValue;
            }

            //If it is 0x09(ASCII TAB), 0x0A(ASCII LF), 0x0C(ASCII FF), 0x0D(ASCII CR), or 0x20(ASCII space)
            //Jump to the step below labeled spaces.
            else if (currentbyte == 0x09 || currentbyte == 0x0A || currentbyte == 0x0C || currentbyte == 0x0D || currentbyte == 0x20)
            {
                goto Space;
            }

            //If it is 0x2F(ASCII /) or 0x3E(ASCII >)
            //Abort the get an attribute algorithm. The attribute's name is the value of attribute name, its value is the empty string.

            else if (currentbyte == 0x2F || 0x2F == 0x3E)
            {
                if (!string.IsNullOrEmpty(AttributeName))
                {
                    return new Attribute() { AttributeName = AttributeName, AttributeValue = AttributeValue };
                }
            }
            else if (Index > Len)//to prevent dead cycle,if Index>Len and byte is cycle
            {
                return null;
            }

            //If it is in the range 0x41(ASCII A) to 0x5A(ASCII Z)
            //Append the Unicode character with code point b+0x20 to attribute name(where b is the value of the byte at position). (This converts the input to lowercase.)
            //if (currentbyte >= 0x41 && currentbyte <= 0x5A)
            //{
            //    currentbyte = (byte)(currentbyte + 0x20); 
            //}


            //Anything else
            //Append the Unicode character with the same code point as the value of the byte at position) to attributename. (It doesn't actually matter how bytes outside the ASCII range are handled here, since only ASCII characters can contribute to the detection of a character encoding.)
            AttributeName += Convert.ToChar(currentbyte).ToString().ToLower();


            //5.   Advance position to the next byte and return to the previous step.
            Index += 1;
            currentbyte = GetByte(ref Input, Index, Len);


            goto GetAttributeName;


        Space:

            //Spaces: If the byte at position is one of 0x09(ASCII TAB), 0x0A(ASCII LF), 0x0C(ASCII FF), 0x0D(ASCII CR), or 0x20(ASCII space) then advance position to the next byte, then, repeat this step.

            while (currentbyte == 0x09 || currentbyte == 0x0A || currentbyte == 0x0C || currentbyte == 0x0D || currentbyte == 0x20)
            {
                Index += 1;
                currentbyte = GetByte(ref Input, Index, Len);
            }

            /// If the byte at position is not 0x3D(ASCII =), abort the get an attribute algorithm. The attribute's name is the value of attribute name, its value is the empty string.

            if (currentbyte != 0x3D)
            {
                if (!string.IsNullOrEmpty(AttributeName))
                {
                    return new Attribute { AttributeName = AttributeName, AttributeValue = AttributeValue };
                }
            }

            // Advance position past the 0x3D(ASCII =) byte.
            Index += 1;
            currentbyte = GetByte(ref Input, Index, Len);

        AttributeValue:

            // 9.Value: If the byte at position is one of 0x09(ASCII TAB), 0x0A(ASCII LF), 0x0C(ASCII FF), 0x0D(ASCII CR), or 0x20(ASCII space) then advance position to the next byte, then, repeat this step.
            while (currentbyte == 0x09 || currentbyte == 0x0A || currentbyte == 0x0C || currentbyte == 0x0D || currentbyte == 0x20)
            {
                Index += 1;
                currentbyte = GetByte(ref Input, Index, Len);
            }

            /// 10  Process the byte at position as follows:

            //If it is 0x22(ASCII ") or 0x27 (ASCII ')
            //Let b be the value of the byte at position.
            if (currentbyte == 0x22 || currentbyte == 0x27)
            {
                var B = currentbyte;
            //Quote loop: Advance position to the next byte.
            QuoteLoop:

                Index += 1;
                currentbyte = GetByte(ref Input, Index, Len);

                //  If the value of the byte at position is the value of b, then advance position to the next byte and abort the "get an attribute" algorithm.The attribute's name is the value of attribute name, and its value is the value of attribute value.
                if (currentbyte == B)
                {
                    Index += 1;
                    return new Attribute { AttributeName = AttributeName, AttributeValue = AttributeValue };
                }
                else
                {
                    /// Otherwise, if the value of the byte at position is in the range 0x41(ASCII A) to 0x5A(ASCII Z), then append a Unicode character to attribute value whose code point is 0x20 more than the value of the byte at position.
                    //if (currentbyte >= 0x41 && currentbyte <= 0x5A)
                    //{
                    //    currentbyte = (byte)(currentbyte + 0x20);
                    //}
                    // Otherwise, append a Unicode character to attribute value whose code point is the same as the value of the byte at position.
                    AttributeValue += Convert.ToChar(currentbyte).ToString().ToLower();
                }

                //Return to the step above labeled quote loop.
                goto QuoteLoop;

            }

            //If it is 0x3E(ASCII >)
            //Abort the get an attribute algorithm. The attribute's name is the value of attribute name, its value is the empty string.
            if (currentbyte == 0x3E)
            {
                return new Attribute { AttributeName = AttributeName, AttributeValue = AttributeValue };
            }

            //If it is in the range 0x41(ASCII A) to 0x5A(ASCII Z)
            //Append the Unicode character with code point b+0x20 to attribute value(where b is the value of the byte at position).Advance position to the next byte.
            //if (currentbyte >= 0x41 && currentbyte <= 0x5A)
            //{
            //    currentbyte = (byte)(currentbyte + 0x20);
            //}

            //Anything else
            //Append the Unicode character with the same code point as the value of the byte at position) to attribute value.Advance position to the next byte.
            AttributeValue += Convert.ToChar(currentbyte).ToString().ToLower();
            Index += 1;
            currentbyte = GetByte(ref Input, Index, Len);

            //11.Process the byte at position as follows:

            while (currentbyte != 0)
            {
                //If it is 0x09(ASCII TAB), 0x0A(ASCII LF), 0x0C(ASCII FF), 0x0D(ASCII CR), 0x20(ASCII space), or 0x3E(ASCII >) 
                //Abort the get an attribute algorithm. The attribute's name is the value of attribute name and its value is the value of attribute value.
                if (currentbyte == 0x09 || currentbyte == 0x0A || currentbyte == 0x0C || currentbyte == 0x0D || currentbyte == 0x20 || currentbyte == 0x3E)
                {
                    return new Attribute { AttributeName = AttributeName, AttributeValue = AttributeValue };
                }

                //If it is in the range 0x41(ASCII A) to 0x5A(ASCII Z)
                //  Append the Unicode character with code point b+0x20 to attribute value(where b is the value of the byte at position).
                //if (currentbyte >= 0x41 && currentbyte <= 0x5A)
                //{
                //    currentbyte = (byte)(currentbyte + 0x20);
                //}

                /// Append the Unicode character with the same code point as the value of the byte at position) to attribute value.
                AttributeValue += Convert.ToChar(currentbyte).ToString().ToLower();

                /// Advance position to the next byte and return to the previous step.
                Index += 1;
                currentbyte = GetByte(ref Input, Index, Len);

            }

            return null;
        }

        public class Attribute
        {
            public string AttributeName { get; set; }

            public string AttributeValue { get; set; }
        }

    }
}
