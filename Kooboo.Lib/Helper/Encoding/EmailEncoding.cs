//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Lib.Helper.EncodingHelper
{
    public static class EmailEncoding
    {
        public static EmailEncodingResult ScanCharSet(byte[] input)
        {
            int len = input.Length;

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

            while (position < len)
            {

                // CHARSET  67/99, 72/104,  65/97,  82/114,  83/115,  69/101,  84/116
                if ((GetByte(position) == 67 || GetByte(position) == 99) && (GetByte(position + 1) == 72 || GetByte(position + 1) == 104) && (GetByte(position + 2) == 65 || GetByte(position + 2) == 97) && (GetByte(position + 3) == 82 || GetByte(position + 3) == 114) && (GetByte(position + 4) == 83 || GetByte(position + 4) == 115) && (GetByte(position + 5) == 69 || GetByte(position + 5) == 101) && (GetByte(position + 6) == 84 || GetByte(position + 6) == 116))
                {
                    // charset found... get the value. 

                    int start = position;
                    int end = -1;
                    string value = GetCharSetValue(input, position + 7, len, ref end);

                    if (value != null && end > -1 && end > start)
                    {
                        int charlen = end - start + 1;

                        var charbytes = new byte[charlen];

                        System.Buffer.BlockCopy(input, position, charbytes, 0, charlen);

                        string charsettext = System.Text.Encoding.ASCII.GetString(charbytes);

                        return new EmailEncodingResult() { CharSetText = charsettext, Charset = value };

                    }

                }

                else if (GetByte(position) == 13 && GetByte(position + 1) == 10 && GetByte(position + 2) == 13 && GetByte(position + 3) == 10)
                {
                    return null;

                }
                position += 1;
            }


            return null;

        }



        public static string GetCharSetValue(byte[] Input, int Index, int Len, ref int end)
        {

            Func<int, byte> GetByte = (index) =>
            {
                if (index >= Len)
                {
                    return 0;
                }
                return Input[index];
            };


            /// 1. If the byte at position is one of 0x09(ASCII TAB), 0x0A(ASCII LF), 0x0C(ASCII FF), 0x0D(ASCII CR), 0x20(ASCII space), or 0x2F(ASCII /) then advance position to the next byte and redo this step.
            var currentbyte = GetByte(Index);
            while (currentbyte == 0x09 || currentbyte == 0x0A || currentbyte == 0x0C || currentbyte == 0x0D || currentbyte == 0x20 || currentbyte == 0x2F)
            {
                Index += 1;
                currentbyte = GetByte(Index);
            }

            // the = sign.. 
            if (currentbyte == 0x3D)
            {
                Index += 1;
                currentbyte = GetByte(Index);

                /// 1. If the byte at position is one of 0x09(ASCII TAB), 0x0A(ASCII LF), 0x0C(ASCII FF), 0x0D(ASCII CR), 0x20(ASCII space), or 0x2F(ASCII /) then advance position to the next byte and redo this step. 
                while (currentbyte == 0x09 || currentbyte == 0x0A || currentbyte == 0x0C || currentbyte == 0x0D || currentbyte == 0x20 || currentbyte == 0x2F)
                {
                    Index += 1;
                    currentbyte = GetByte(Index);
                }

                //If it is 0x22(ASCII ") or 0x27 (ASCII ')
                //Let b be the value of the byte at position.
                if (currentbyte == 0x22 || currentbyte == 0x27)
                {
                    var B = currentbyte;

                    List<byte> bytes = new List<byte>();

                    Index += 1;
                    currentbyte = GetByte(Index);

                    while (currentbyte != B && currentbyte != 0)
                    {
                        bytes.Add(currentbyte);
                        Index += 1;
                        currentbyte = GetByte(Index);
                    }

                    if (bytes.Count() > 0)
                    {
                        end = Index;
                        return System.Text.Encoding.ASCII.GetString(bytes.ToArray()).Trim();
                    }
                }

                else
                {
                    List<byte> bytes = new List<byte>();

                    if (isCharSet(currentbyte))
                    {
                        bytes.Add(currentbyte);

                        while (currentbyte != 0)
                        {
                            Index += 1;
                            currentbyte = GetByte(Index);

                            if (isCharSet(currentbyte))
                            {
                                bytes.Add(currentbyte);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    if (bytes.Count() > 0)
                    {
                        end = Index - 1;
                        return System.Text.Encoding.ASCII.GetString(bytes.ToArray()).Trim();
                    }
                }

            }


            return null;
        }


        /// <summary>
        /// The lowercase ASCII letters are the characters in the range lowercase ASCII letters. a-z
        /// </summary>
        /// <param name="chr"></param>
        /// <returns></returns>
        public static bool isLowercaseAscii(byte chr)
        {
            //a-z, ascii 61-122. 
            return (chr >= 97 && chr <= 122);
        }

        /// <summary>
        /// The uppercase ASCII letters are the characters in the range uppercase ASCII letters. A-Z
        /// </summary>
        /// <param name="chr"></param>
        /// <returns></returns>
        public static bool isUppercaseAscii(byte chr)
        {
            return (chr >= 65 && chr <= 90);
        }

        /// <summary>
        /// The ASCII digits are the characters in the range ASCII digits.
        /// </summary>
        /// <param name="chr"></param>
        /// <returns></returns>
        public static bool isAsciiDigit(byte chr)
        {
            //0-9, acsii 48-57. 
            return (chr >= 48 && chr <= 57);
        }

        /// <summary>
        /// The alphanumeric ASCII characters are those that are either uppercase ASCII letters, lowercase ASCII letters, or ASCII digits.
        /// </summary>
        /// <param name="chr"></param>
        /// <returns></returns>
        public static bool isCharSet(byte chr)
        {
            if (isUppercaseAscii(chr) || isLowercaseAscii(chr) || isAsciiDigit(chr))
            {
                return true;
            }
            // - or _
            if (chr == 45 || chr == 95)
            {
                return true;
            }
            return false;
        }


    }

    public class EmailEncodingResult
    {
        public string Charset { get; set; }

        // the exactly text to be replace.. 
        public string CharSetText { get; set; }
    }
}
