using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Mail.MassMailing
{
    public static class HeaderHelper
    {

        // return header and the remaining raw message.
        public static MTAHeaderResult ScanHeader(string RawMessageBody)
        {
            if (RawMessageBody == null)
            {
                return null;
            }

            var bodyPartIndex = RawMessageBody.IndexOf("\r\n\r\n");

            if (bodyPartIndex == -1)
            {
                bodyPartIndex = RawMessageBody.IndexOf("\n\n");
            }

            if (bodyPartIndex == -1)
            {
                return null;
            }

            var header = RawMessageBody.Substring(0, bodyPartIndex);

            if (header.StartsWith("\r\n"))
            {
                header = header.Substring(2);
            }
            else if (header.StartsWith("\n"))
            {
                header = header.Substring(1);
            }

            var bodyPart = RawMessageBody.Substring(bodyPartIndex);  // Incl \r\n\r\n. 

            HeaderScanner headerScanner = new HeaderScanner(header);

            var line = headerScanner.ConsumeLine();


            MTAHeader mtaHeader = new MTAHeader();

            List<LineResult> linesToRemove = new List<LineResult>();

            while (line != null && line.Value != null)
            {
                var (key, value) = ParseHeader(line.Value);

                if (key != null && value != null)
                {
                    var lowerKey = key.ToLower();

                    if (lowerKey == "from")
                    {
                        mtaHeader.From = value;
                    }
                    else if (lowerKey == "to")
                    {
                        mtaHeader.To = value;
                    }
                    else if (lowerKey == "subject")
                    {
                        mtaHeader.Subject = value;
                    }
                    else if (lowerKey == "date")
                    {
                        mtaHeader.Date = value;
                        linesToRemove.Add(line);
                    }
                    else if (lowerKey == "x-command")
                    {
                        mtaHeader.Command = XCommand.ParseHeaderLine(value);
                        linesToRemove.Add(line);
                    }
                }
                line = headerScanner.ConsumeLine();
            }

            var newHeader = RemoveLine(linesToRemove, header);

            return new MTAHeaderResult() { MtaHeader = mtaHeader, NewHeader = newHeader, Body = bodyPart };
        }

        public static string RemoveLine(List<LineResult> lines, string OriginalText)
        {
            int currentRead = 0;

            string result = null;

            foreach (var item in lines.OrderBy(o => o.Start))
            {
                if (item.Start > currentRead)
                {
                    result += OriginalText.Substring(currentRead, item.Start - currentRead);
                    currentRead = item.End + 1;
                }
                else if (item.Start == currentRead)
                {
                    currentRead = item.End + 1;
                }
            }

            if (currentRead < OriginalText.Length)
            {
                result += OriginalText.Substring(currentRead, OriginalText.Length - currentRead);
            }
            return result;
        }

        private static (string, string) ParseHeader(string headerLine)
        {
            if (string.IsNullOrWhiteSpace(headerLine))
            {
                return (null, null);
            }

            int Indexer = headerLine.IndexOf(":");

            if (Indexer > -1)
            {
                var Key = headerLine.Substring(0, Indexer);

                var value = headerLine.Substring(Indexer + 1);

                if (Key != null && value != null)
                {
                    return (Key.Trim(), value.Trim());
                }
            }
            return (null, null);
        }

    }


}


//https://www.rfc-editor.org/rfc/rfc5322#section-2.2
/*
Header Fields

   Header fields are lines beginning with a field name, followed by a
   colon (":"), followed by a field body, and terminated by CRLF.  A
   field name MUST be composed of printable US-ASCII characters (i.e.,
  characters that have values between 33 and 126, inclusive), except
   colon.  A field body may be composed of printable US-ASCII characters
   as well as the space (SP, ASCII value 32) and horizontal tab (HTAB,
  ASCII value 9) characters (together known as the white space
   characters, WSP).  A field body MUST NOT include CR and LF except
   when used in "folding" and "unfolding", as described in section
   2.2.3.  All field bodies MUST conform to the syntax described in
   sections 3 and 4 of this specification.

2.2.1.  Unstructured Header Field Bodies

   Some field bodies in this specification are defined simply as
   "unstructured" (which is specified in section 3.2.5 as any printable
   US-ASCII characters plus white space characters) with no further
   restrictions.  These are referred to as unstructured field bodies.
   Semantically, unstructured field bodies are simply to be treated as a
   single line of characters with no further processing (except for
   "folding" and "unfolding" as described in section 2.2.3).

2.2.2.  Structured Header Field Bodies

   Some field bodies in this specification have a syntax that is more
   restrictive than the unstructured field bodies described above.
   These are referred to as "structured" field bodies.  Structured field
   bodies are sequences of specific lexical tokens as described in
   sections 3 and 4 of this specification.  Many of these tokens are
   allowed (according to their syntax) to be introduced or end with
   comments (as described in section 3.2.2) as well as the white space
   characters, and those white space characters are subject to "folding"
   and "unfolding" as described in section 2.2.3.  Semantic analysis of
   structured field bodies is given along with their syntax.

2.2.3.  Long Header Fields

   Each header field is logically a single line of characters comprising
   the field name, the colon, and the field body.  For convenience
   however, and to deal with the 998/78 character limitations per line,
  the field body portion of a header field can be split into a
   multiple-line representation; this is called "folding".The general
   rule is that wherever this specification allows for folding white
   space (not simply WSP characters), a CRLF may be inserted before any
   WSP.




Resnick                     Standards Track                     [Page 8]

RFC 5322                Internet Message Format             October 2008


   For example, the header field:

   Subject: This is a test

   can be represented as:

   Subject: This
    is a test

      Note: Though structured field bodies are defined in such a way
      that folding can take place between many of the lexical tokens
      (and even within some of the lexical tokens), folding SHOULD be
      limited to placing the CRLF at higher-level syntactic breaks.  For
      instance, if a field body is defined as comma-separated values, it
      is recommended that folding occur after the comma separating the
      structured items in preference to other places where the field
      could be folded, even if it is allowed elsewhere.

   The process of moving from this folded multiple-line representation
   of a header field to its single line representation is called
   "unfolding".  Unfolding is accomplished by simply removing any CRLF
   that is immediately followed by WSP.  Each header field should be
   treated in its unfolded form for further syntactic and semantic
   evaluation.  An unfolded header field has no length restriction and
   therefore may be indeterminately long.

*/