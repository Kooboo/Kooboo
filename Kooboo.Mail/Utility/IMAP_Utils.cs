using System;
using System.IO;
using System.Text;

namespace Kooboo.Mail.Utility
{
    /// <summary>
    /// Provides utility methods for IMAP.
    /// </summary>
    public class IMAP_Utils
    {

        #region method ParseDate

        /// <summary>
        /// Parses IMAP date time from string.
        /// </summary>
        /// <param name="date">DateTime string.</param>
        /// <returns>Returns parsed date-time value.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>date</b> is null reference.</exception>
        //public static DateTime ParseDate(string date)
        //{
        //          if(date == null){
        //              throw new ArgumentNullException("date");
        //          }

        //          /* RFC 3501. IMAP date format. 
        //	    date-time       = DQUOTE date-day-fixed "-" date-month "-" date-year SP time SP zone DQUOTE
        //		date            = day-month-year
        //              date-day-fixed  = (SP DIGIT) / 2DIGIT
        //                              ; Fixed-format version of date-day
        //              date-month      = "Jan" / "Feb" / "Mar" / "Apr" / "May" / "Jun" /
        //                                "Jul" / "Aug" / "Sep" / "Oct" / "Nov" / "Dec"
        //		time            = 2DIGIT ":" 2DIGIT ":" 2DIGIT
        //	*/
        //          if(date.IndexOf('-') > -1){
        //              try{
        //                  return DateTime.ParseExact(date.Trim(),new string[]{"d-MMM-yyyy","d-MMM-yyyy HH:mm:ss zzz"},System.Globalization.DateTimeFormatInfo.InvariantInfo,System.Globalization.DateTimeStyles.None);
        //              }
        //              catch{
        //                  throw new ArgumentException("Argument 'date' value '" + date + "' is not valid IMAP date.");
        //              }
        //          }
        //          else{
        //              return  LumiSoft.Net.MIME.MIME_Utils.ParseRfc2822DateTime(date);
        //          }
        //}

        #endregion

        #region static DateTimeToString

        /// <summary>
        /// Converts date time to IMAP date time string.
        /// </summary>
        /// <param name="date">DateTime to convert.</param>
        /// <returns></returns>
        public static string DateTimeToString(DateTime date)
        {
            string retVal = "";
            retVal += date.ToString("dd-MMM-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            retVal += " " + date.ToString("zzz", System.Globalization.CultureInfo.InvariantCulture).Replace(":", "");

            return retVal;
        }

        #endregion


        #region static method Encode_IMAP_UTF7_String

        /// <summary>
        /// Encodes specified data with IMAP modified UTF7 encoding. Defined in RFC 3501 5.1.3.  Mailbox International Naming Convention.
        /// Example: öö is encoded to &amp;APYA9g-.
        /// </summary>
        /// <param name="text">Text to encode.</param>
        /// <returns></returns>
        public static string Encode_IMAP_UTF7_String(string text)
        {
            /* RFC 3501 5.1.3.  Mailbox International Naming Convention
				In modified UTF-7, printable US-ASCII characters, except for "&",
				represent themselves; that is, characters with octet values 0x20-0x25
				and 0x27-0x7e.  The character "&" (0x26) is represented by the
				two-octet sequence "&-".

				All other characters (octet values 0x00-0x1f and 0x7f-0xff) are
				represented in modified BASE64, with a further modification from
				[UTF-7] that "," is used instead of "/".  Modified BASE64 MUST NOT be
				used to represent any printing US-ASCII character which can represent
				itself.
				
				"&" is used to shift to modified BASE64 and "-" to shift back to
				US-ASCII.  There is no implicit shift from BASE64 to US-ASCII, and
				null shifts ("-&" while in BASE64; note that "&-" while in US-ASCII
				means "&") are not permitted.  However, all names start in US-ASCII,
				and MUST end in US-ASCII; that is, a name that ends with a non-ASCII
				ISO-10646 character MUST end with a "-").
			*/

            // Base64 chars, except '/' is replaced with ','
            char[] base64Chars = new char[]{
                'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
                'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
                '0','1','2','3','4','5','6','7','8','9','+',','
            };

            MemoryStream retVal = new MemoryStream();
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                // The character "&" (0x26) is represented by the two-octet sequence "&-".
                if (c == '&')
                {
                    retVal.Write(new byte[] { (byte)'&', (byte)'-' }, 0, 2);
                }
                // It is allowed char, don't need to encode
                else if (c >= 0x20 && c <= 0x25 || c >= 0x27 && c <= 0x7E)
                {
                    retVal.WriteByte((byte)c);
                }
                // Not allowed char, encode it
                else
                {
                    // Superfluous shifts are not allowed. 
                    // For example: öö may not encoded as &APY-&APY-, but must be &APYA9g-.

                    // Get all continuous chars that need encoding and encode them as one block
                    MemoryStream encodeBlock = new MemoryStream();
                    for (int ic = i; ic < text.Length; ic++)
                    {
                        char cC = text[ic];

                        // Allowed char
                        if (cC >= 0x20 && cC <= 0x25 || cC >= 0x27 && cC <= 0x7E)
                        {
                            break;
                        }
                        else
                        {
                            encodeBlock.WriteByte((byte)((cC & 0xFF00) >> 8));
                            encodeBlock.WriteByte((byte)(cC & 0xFF));
                            i = ic;
                        }
                    }

                    // Ecode block
                    byte[] encodedData = Net_Utils.Base64EncodeEx(encodeBlock.ToArray(), base64Chars, false);
                    retVal.WriteByte((byte)'&');
                    retVal.Write(encodedData, 0, encodedData.Length);
                    retVal.WriteByte((byte)'-');
                }
            }

            return System.Text.Encoding.Default.GetString(retVal.ToArray());
        }

        #endregion

        #region static method Decode_IMAP_UTF7_String

        /// <summary>
        /// Decodes IMAP modified UTF7 encoded data. Defined in RFC 3501 5.1.3.  Mailbox International Naming Convention.
        /// Example: &amp;APYA9g- is decoded to öö.
        /// </summary>
        /// <param name="text">Text to encode.</param>
        /// <returns></returns>
        public static string Decode_IMAP_UTF7_String(string text)
        {
            /* RFC 3501 5.1.3.  Mailbox International Naming Convention
				In modified UTF-7, printable US-ASCII characters, except for "&",
				represent themselves; that is, characters with octet values 0x20-0x25
				and 0x27-0x7e.  The character "&" (0x26) is represented by the
				two-octet sequence "&-".

				All other characters (octet values 0x00-0x1f and 0x7f-0xff) are
				represented in modified BASE64, with a further modification from
				[UTF-7] that "," is used instead of "/".  Modified BASE64 MUST NOT be
				used to represent any printing US-ASCII character which can represent
				itself.
				
				"&" is used to shift to modified BASE64 and "-" to shift back to
				US-ASCII.  There is no implicit shift from BASE64 to US-ASCII, and
				null shifts ("-&" while in BASE64; note that "&-" while in US-ASCII
				means "&") are not permitted.  However, all names start in US-ASCII,
				and MUST end in US-ASCII; that is, a name that ends with a non-ASCII
				ISO-10646 character MUST end with a "-").
			*/

            // Base64 chars, except '/' is replaced with ','
            char[] base64Chars = new char[]{
                'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
                'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
                '0','1','2','3','4','5','6','7','8','9','+',','
            };

            StringBuilder retVal = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                // Encoded block or escaped &
                if (c == '&')
                {
                    int endingPos = -1;
                    // Read encoded block
                    for (int b = i + 1; b < text.Length; b++)
                    {
                        // - marks block end
                        if (text[b] == '-')
                        {
                            endingPos = b;
                            break;
                        }
                        // Invalid & sequence, just treat it as '&' char and not like shift.
                        // &....&, but must be &....-
                        else if (text[b] == '&')
                        {
                            break;
                        }
                    }

                    // If no ending -, invalid encoded block. Treat it like it is
                    if (endingPos == -1)
                    {
                        // Just let main for to handle other chars after &
                        retVal.Append(c);
                    }
                    // If empty block, then escaped &
                    else if (endingPos - i == 1)
                    {
                        retVal.Append(c);
                        // Move i over '-'
                        i++;
                    }
                    // Decode block
                    else
                    {
                        // Get encoded block
                        byte[] encodedBlock = System.Text.Encoding.Default.GetBytes(text.Substring(i + 1, endingPos - i - 1));

                        // Convert to UTF-16 char						
                        byte[] decodedData = Net_Utils.Base64DecodeEx(encodedBlock, base64Chars);
                        char[] decodedChars = new char[decodedData.Length / 2];
                        for (int iC = 0; iC < decodedChars.Length; iC++)
                        {
                            decodedChars[iC] = (char)(decodedData[iC * 2] << 8 | decodedData[(iC * 2) + 1]);
                        }

                        // Decode data
                        retVal.Append(decodedChars);

                        // Move i over '-'
                        i += encodedBlock.Length + 1;
                    }
                }
                // Normal byte
                else
                {
                    retVal.Append(c);
                }
            }

            return retVal.ToString();
        }

        #endregion

        #region static method EncodeMailbox

        /// <summary>
        /// Encodes mailbox name.
        /// </summary>
        /// <param name="mailbox">Mailbox name.</param>
        /// <param name="encoding">Mailbox name encoding mechanism.</param>
        /// <returns>Renturns encoded mailbox name.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>mailbox</b> is null reference.</exception>
        public static string EncodeMailbox(string mailbox, IMAP_Mailbox_Encoding encoding)
        {
            if (mailbox == null)
            {
                throw new ArgumentNullException("mailbox");
            }

            /* RFC 6855 3.
                quoted        = DQUOTE *uQUOTED-CHAR DQUOTE
                uQUOTED-CHAR  = QUOTED-CHAR / UTF8-2 / UTF8-3 / UTF8-4
            */

            if (encoding == IMAP_Mailbox_Encoding.ImapUtf7)
            {
                return "\"" + IMAP_Utils.Encode_IMAP_UTF7_String(mailbox) + "\"";
            }
            else if (encoding == IMAP_Mailbox_Encoding.ImapUtf8)
            {
                return "\"" + mailbox + "\"";
            }
            else
            {
                return "\"" + mailbox + "\"";
            }
        }

        #endregion

        #region static method DecodeMailbox

        /// <summary>
        /// Decodes mailbox name.
        /// </summary>
        /// <param name="mailbox">Mailbox name.</param>
        /// <returns>Returns decoded mailbox name.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>mailbox</b> is null reference.</exception>
        public static string DecodeMailbox(string mailbox)
        {
            if (mailbox == null)
            {
                throw new ArgumentNullException("mailbox");
            }

            /* RFC 5738 3.
                string        =/ utf8-quoted
                utf8-quoted   = "*" DQUOTE *UQUOTED-CHAR DQUOTE
                UQUOTED-CHAR  = QUOTED-CHAR / UTF8-2 / UTF8-3 / UTF8-4
            */

            // UTF-8 mailbox name.
            if (mailbox.StartsWith("*\""))
            {
                return mailbox.Substring(2, mailbox.Length - 3);
            }
            else
            {
                return Decode_IMAP_UTF7_String(TextUtils.UnQuoteString(mailbox));
            }
        }

        #endregion 
    }
}
