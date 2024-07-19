//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using MimeKit;


namespace Kooboo.Mail.Utility
{
    public static class HeaderUtility
    {
        public static string EncodeField(string input, bool IsAddress = false)
        {
            if (ShouldEncode(input))
            {
                var wordEncoder = new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.B, Encoding.UTF8);

                if (IsAddress)
                {
                    int index = input.IndexOf("<");
                    int endindex = input.LastIndexOf(">");
                    if (index > 0 && endindex > index)
                    {
                        var word = input.Substring(0, index);
                        var encoded = wordEncoder.Encode(word);
                        return encoded + input.Substring(index);
                    }
                }
                return wordEncoder.Encode(input);

            }
            return input;
        }


        public static string MailKitEncodeAddressField(string input)
        {
            if (ShouldEncode(input))
            {
                if (InternetAddressList.TryParse(input, out var addresses))
                {
                    if (addresses != null)
                    {
                        foreach (var item in addresses)
                        {
                            var name = item.Name;
                            if (name != null)
                            {
                                name = name.Trim();
                                name = name.Trim('\'');
                                item.Name = name;
                            }
                        }
                    }

                    return addresses.ToString(true);
                }
            }

            return input;

        }


        private static bool ShouldEncode(string input)
        {
            if (input == null)
            {
                return false;
            }
            int len = input.Length;

            for (int i = 0; i < len; i++)
            {
                if (input[i] > 127)
                {
                    return true;
                }
            }
            return false;
        }

        public static string EncodeFieldB(string input, bool IsAddress = false)
        {
            if (ShouldEncode(input))
            {
                var wordEncoder = new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.B, Encoding.UTF8);

                if (IsAddress)
                {
                    int index = input.IndexOf("<");
                    int endindex = input.LastIndexOf(">");
                    if (index > 0 && endindex > index)
                    {
                        var word = input.Substring(0, index);
                        var encoded = wordEncoder.Encode(word);
                        return encoded + input.Substring(index);
                    }
                }
                return wordEncoder.Encode(input);
            }
            return input;
        }


        public static string ExtraID(string ContentOrMessageId)
        {
            if (string.IsNullOrWhiteSpace(ContentOrMessageId))
            {
                return null;
            }

            ContentOrMessageId = ContentOrMessageId.Trim();

            if (ContentOrMessageId.StartsWith("<") && ContentOrMessageId.EndsWith(">"))
            {
                ContentOrMessageId = ContentOrMessageId.Substring(1, ContentOrMessageId.Length - 2);
            }

            return ContentOrMessageId;
        }
    }

    public enum MIME_EncodedWordEncoding
    {
        /// <summary>
        /// The "B" encoding. Defined in RFC 2047 (section 4.1).
        /// </summary>
        Q,

        /// <summary>
        /// The "Q" encoding. Defined in RFC 2047 (section 4.2).
        /// </summary>
        B
    }



    public class MIME_Encoding_EncodedWord
    {
        private MIME_EncodedWordEncoding m_Encoding;
        private Encoding m_pCharset = null;
        private bool m_Split = true;

        //private static readonly Regex encodedword_regex = new Regex(@"=\?(((?<charset>.*?)\*.*?)|(?<charset>.*?))\?(?<encoding>[qQbB])\?(?<value>.*?)\?=(?<whitespaces>\s*)",RegexOptions.IgnoreCase);
        private static readonly Regex encodedword_regex = new Regex(@"\=\?(?<charset>\S+?)\?(?<encoding>[qQbB])\?(?<value>.+?)\?\=(?<whitespaces>\s*)", RegexOptions.IgnoreCase);

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="encoding">Encoding to use to encode text.</param>
        /// <param name="charset">Charset to use for encoding. If not sure UTF-8 is strongly recommended.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>charset</b> is null reference.</exception>
        public MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding encoding, Encoding charset)
        {
            if (charset == null)
            {
                throw new ArgumentNullException("charset");
            }

            m_Encoding = encoding;
            m_pCharset = charset;
        }


        /// <summary>
        /// Encodes specified text if it contains 8-bit chars, otherwise text won't be encoded.
        /// </summary>
        /// <param name="text">Text to encode.</param>
        /// <returns>Returns encoded text.</returns>
        public string Encode(string text)
        {
            if (MustEncode(text))
            {
                return EncodeS(m_Encoding, m_pCharset, m_Split, text);
            }
            else
            {
                return text;
            }
        }






        /// <summary>
        /// Checks if specified text must be encoded.
        /// </summary>
        /// <param name="text">Text to encode.</param>
        /// <returns>Returns true if specified text must be encoded, otherwise false.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>text</b> is null reference.</exception>
        public static bool MustEncode(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            // Encoding is needed only for non-ASCII chars.

            foreach (char c in text)
            {
                if (c > 127)
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Encodes specified text if it contains 8-bit chars, otherwise text won't be encoded.
        /// </summary>
        /// <param name="encoding">Encoding to use to encode text.</param>
        /// <param name="charset">Charset to use for encoding. If not sure UTF-8 is strongly recommended.</param>
        /// <param name="split">If true, words are splitted after 75 chars.</param>
        /// <param name="text">Text to encode.</param>
        /// <returns>Returns encoded text.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>charset</b> or <b>text</b> is null reference.</exception>
        public static string EncodeS(MIME_EncodedWordEncoding encoding, Encoding charset, bool split, string text)
        {
            if (charset == null)
            {
                throw new ArgumentNullException("charset");
            }
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            /* RFC 2047 2.
                encoded-word = "=?" charset "?" encoding "?" encoded-text "?="
             
                An 'encoded-word' may not be more than 75 characters long, including
                'charset', 'encoding', 'encoded-text', and delimiters.  If it is
                desirable to encode more text than will fit in an 'encoded-word' of
                75 characters, multiple 'encoded-word's (separated by CRLF SPACE) may
                be used.
             
               RFC 2231 (updates syntax)
                encoded-word := "=?" charset ["*" language] "?" encoded-text "?="
            */

            if (MustEncode(text))
            {
                List<string> parts = new List<string>();
                if (split)
                {
                    int index = 0;
                    // We just split text to 30 char words, then if some chars encoded, we don't exceed 75 chars lenght limit.
                    while (index < text.Length)
                    {
                        int countReaded = Math.Min(30, text.Length - index);
                        parts.Add(text.Substring(index, countReaded));
                        index += countReaded;
                    }
                }
                else
                {
                    parts.Add(text);
                }

                StringBuilder retVal = new StringBuilder();
                for (int i = 0; i < parts.Count; i++)
                {
                    string part = parts[i];
                    byte[] data = charset.GetBytes(part);

                    #region B encode

                    if (encoding == MIME_EncodedWordEncoding.B)
                    {
                        retVal.Append("=?" + charset.WebName + "?B?" + Convert.ToBase64String(data) + "?=");
                    }

                    #endregion

                    #region Q encode

                    else
                    {
                        retVal.Append("=?" + charset.WebName + "?Q?");
                        int stored = 0;
                        foreach (byte b in data)
                        {
                            string val = null;
                            // We need to encode byte. Defined in RFC 2047 4.2.
                            if (b > 127 || b == '=' || b == '?' || b == '_' || b == ' ')
                            {
                                val = "=" + b.ToString("X2");
                            }
                            else
                            {
                                val = ((char)b).ToString();
                            }

                            retVal.Append(val);
                            stored += val.Length;
                        }
                        retVal.Append("?=");
                    }

                    #endregion

                    if (i < (parts.Count - 1))
                    {
                        retVal.Append("\r\n ");
                    }
                }

                return retVal.ToString();
            }
            else
            {
                return text;
            }
        }



        /// <summary>
        /// Gets or sets if long words(over 75 char) are splitted.
        /// </summary>
        public bool Split
        {
            get { return m_Split; }

            set { m_Split = value; }
        }

    }

}
