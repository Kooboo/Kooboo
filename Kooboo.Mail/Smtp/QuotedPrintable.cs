//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Text;
using MimeKit.Encodings;

namespace Kooboo.Mail.Smtp
{
    public class QuotedPrintable
    {
        private const int MaxLineLength = 76;
        private const string SoftLineBreak = "=\r\n";

        private static MimeKit.Encodings.QuotedPrintableEncoder coder = new MimeKit.Encodings.QuotedPrintableEncoder();

        public static string Encode(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            var bytes = Encoding.UTF8.GetBytes(input);

            MimeKit.Encodings.QuotedPrintableEncoder encoder = new MimeKit.Encodings.QuotedPrintableEncoder(76);

            var output = new byte[encoder.EstimateOutputLength(bytes.Length)];
            var outputLength = encoder.Flush(bytes, 0, bytes.Length, output);

            return Encoding.UTF8.GetString(output, 0, outputLength);
        }

        public static string Decode(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            var buffer = Encoding.UTF8.GetBytes(input);

            var decoder = new QuotedPrintableDecoder();
            var length = decoder.EstimateOutputLength(buffer.Length);
            var decoded = new byte[length];
            var n = decoder.Decode(buffer, 0, buffer.Length, decoded);

            return Encoding.UTF8.GetString(decoded, 0, n);

        }

        [Obsolete]
        public static string Encode_old(string str)
        {

            var bytes = Encoding.UTF8.GetBytes(str);

            var builder = new StringBuilder();
            var lineLength = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                var ch = bytes[i];
                if ((IsPrintableChar(ch) && !IsEqualChar(ch)))
                {
                    // See Rule #2, printable characters
                    if (lineLength + 1 >= MaxLineLength)
                    {
                        // See Rule #5, max 76 line length
                        if (i + 1 < bytes.Length && !IsLinebreak(bytes[i + 1]))
                        {
                            // Next character is not line break or not end of content, need soft line break
                            builder.Append(SoftLineBreak);
                            lineLength = 0;
                        }
                    }
                    builder.Append((char)ch);
                    lineLength++;
                }
                else if (IsSpaceChar(ch))
                {
                    // See Rule #3, space and tab
                    if (lineLength + 1 >= MaxLineLength)
                    {
                        // See Rule #5, max 76 line length and line could not end with space character
                        // If over max length, use soft break
                        builder.Append(SoftLineBreak);
                        lineLength = 0;
                    }
                    builder.Append((char)ch);
                    lineLength++;
                }
                else if (IsLinebreak(ch))
                {
                    // See Rule #4
                    builder.Append((char)ch);
                    lineLength = 0;
                }
                else
                {
                    // See Rule #1
                    if (lineLength + 3 >= MaxLineLength)
                    {
                        if (i + 1 < bytes.Length && !IsLinebreak(bytes[i + 1]))
                        {
                            // Next character is not line break or not end of content, need soft line break
                            builder.Append(SoftLineBreak);
                            lineLength = 0;
                        }
                    }

                    // Encode to upper case HEX string
                    builder.Append('=').Append(ch.ToString("X2").ToUpper());
                    lineLength += 3;
                }
            }

            return builder.ToString();
        }

        static bool IsEqualChar(byte ch)
        {
            return ch == 61;
        }

        static bool IsPrintableChar(byte ch)
        {
            return (ch >= 33 && ch <= 126);
        }

        static bool IsSpaceChar(byte ch)
        {
            return (ch == 32 || ch == 9);
        }

        static bool IsLinebreak(byte ch)
        {
            return (ch == 13 || ch == 10);
        }
    }
}


/*
      Rule #1: (General 8-bit representation) Any octet, except those
      indicating a line break according to the newline convention of the
      canonical(standard) form of the data being encoded, may be
      represented by an "=" followed by a two digit hexadecimal
      representation of the octet's value.  The digits of the
      hexadecimal alphabet, for this purpose, are "0123456789ABCDEF".
      Uppercase letters must be used when sending hexadecimal data,
      though a robust implementation may choose to recognize lowercase
      letters on receipt.  Thus, for example, the value 12 (ASCII form
      feed) can be represented by "=0C", and the value 61 (ASCII EQUAL
      SIGN) can be represented by "=3D".  Except when the following
      rules allow an alternative encoding, this rule is mandatory.

      Rule #2: (Literal representation) Octets with decimal values of 33
      through 60 inclusive, and 62 through 126, inclusive, MAY be
      represented as the ASCII characters which correspond to those
      octets (EXCLAMATION POINT through LESS THAN, and GREATER THAN
      through TILDE, respectively).

      Rule #3: (White Space): Octets with values of 9 and 32 MAY be
      represented as ASCII TAB (HT) and SPACE characters, respectively,
      but MUST NOT be so represented at the end of an encoded line. Any
      TAB (HT) or SPACE characters on an encoded line MUST thus be
      followed on that line by a printable character.  In particular, an
      "=" at the end of an encoded line, indicating a soft line break
      (see rule #5) may follow one or more TAB (HT) or SPACE characters.
      It follows that an octet with value 9 or 32 appearing at the end
      of an encoded line must be represented according to Rule #1.  This
      rule is necessary because some MTAs (Message Transport Agents,
      programs which transport messages from one user to another, or
      perform a part of such transfers) are known to pad lines of text
      with SPACEs, and others are known to remove "white space"
      characters from the end of a line.  Therefore, when decoding a
      Quoted-Printable body, any trailing white space on a line must be
      deleted, as it will necessarily have been added by intermediate
      transport agents.

      Rule #4 (Line Breaks): A line break in a text body, independent of
      what its representation is following the canonical representation
      of the data being encoded, must be represented by a (RFC 822) line
      break, which is a CRLF sequence, in the Quoted-Printable encoding.
      Since the canonical representation of types other than text do not
      generally include the representation of line breaks, no hard line
      breaks (i.e.  line breaks that are intended to be meaningful and
      to be displayed to the user) should occur in the quoted-printable
      encoding of such types.  Of course, occurrences of "=0D", "=0A",
      "0A=0D" and "=0D=0A" will eventually be encountered.  In general,
      however, base64 is preferred over quoted-printable for binary
      data.

      Note that many implementations may elect to encode the local
      representation of various content types directly, as described in
      Appendix G.  In particular, this may apply to plain text material
      on systems that use newline conventions other than CRLF
      delimiters. Such an implementation is permissible, but the
      generation of line breaks must be generalized to account for the
      case where alternate representations of newline sequences are
      used.

      Rule #5 (Soft Line Breaks): The Quoted-Printable encoding REQUIRES
      that encoded lines be no more than 76 characters long. If longer
      lines are to be encoded with the Quoted-Printable encoding, 'soft'
      line breaks must be used. An equal sign as the last character on a
      encoded line indicates such a non-significant ('soft') line break
      in the encoded text. Thus if the "raw" form of the line is a
      single unencoded line that says:

          Now's the time for all folk to come to the aid of
          their country.

      This can be represented, in the Quoted-Printable encoding, as

          Now's the time =
          for all folk to come=
           to the aid of their country.

      This provides a mechanism with which long lines are encoded in
      such a way as to be restored by the user agent.  The 76 character
      limit does not count the trailing CRLF, but counts all other
      characters, including any equal signs.
*/