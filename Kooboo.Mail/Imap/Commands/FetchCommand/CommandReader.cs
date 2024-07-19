//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Mail.Imap.Commands.FetchCommand
{
    // ABNF Syntax Format.
    //    fetch       = "FETCH" SP sequence-set SP("ALL" / "FULL" / "FAST" /
    //                  fetch-att / "(" fetch-att*(SP fetch-att) ")") 
    //fetch-att       = "ENVELOPE" / "FLAGS" / "INTERNALDATE" /
    //                  "RFC822" [".HEADER" / ".SIZE" / ".TEXT"] /
    //                  "BODY" ["STRUCTURE"] / "UID" /
    //                  "BODY" section["<" number "." nz - number ">"] /
    //                  "BODY.PEEK" section["<" number "." nz - number ">"]

    public class CommandReader
    {
        internal string WholeText { get; set; }

        public string Arguments { get; set; }

        public string SequenceSet { get; set; }

        internal int CurrentIndex { get; set; } = 0;

        private int _totallen;
        internal int TotalLen
        {

            get
            {
                if (_totallen == default(int))
                {
                    if (this.WholeText != null)
                    {
                        _totallen = this.WholeText.Length;
                    }
                }
                return _totallen;

            }
            set
            {
                _totallen = value;
            }
        }

        public CommandReader(string CommandText)
        {
            this.WholeText = CommandText;
            this.TotalLen = this.WholeText.Length;
            this.ParseSequenceSet();
        }

        internal void ParseSequenceSet()
        {
            string seq = null;
            while (CurrentIndex < TotalLen)
            {
                var currentChar = this.WholeText[CurrentIndex];

                if (Lib.Helper.CharHelper.IsAscii(currentChar))
                {
                    return; // ASCII char only for body, not for sequence... this is an error... 
                }

                if (Lib.Helper.CharHelper.isSpaceCharacters(currentChar))
                {
                    if (seq != null)
                    {
                        this.SequenceSet = seq;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    seq += currentChar;
                }
                CurrentIndex += 1;
            }

            if (!string.IsNullOrEmpty(this.SequenceSet))
            {
                string newwholetext = this.WholeText.Substring(this.CurrentIndex);
                this.CurrentIndex = 0;

                if (string.IsNullOrEmpty(newwholetext))
                {
                    newwholetext = "FULL";
                }

                newwholetext = newwholetext.Trim();
                if (newwholetext.StartsWith("("))
                {
                    if (newwholetext.EndsWith(")"))
                    {
                        newwholetext = newwholetext.Substring(1, newwholetext.Length - 2);
                    }
                    else
                    {
                        newwholetext = newwholetext.Substring(1);
                    }
                }

                this.TotalLen = newwholetext.Length;
                this.WholeText = newwholetext;
            }
        }

        internal bool IsLookupCharEnd(int currentindex)
        {
            while (currentindex < TotalLen)
            {
                var currentChar = this.WholeText[currentindex];

                if (currentChar == '[' || currentChar == '(' || currentChar == '<')
                {
                    return false;
                }
                if (Lib.Helper.CharHelper.IsAscii(currentChar))
                {
                    return true;
                }
                currentindex += 1;
            }
            return true;
        }

        internal string ReadToNext(char StartChar, char EndChar, int index)
        {
            string result = null;

            while (index < TotalLen)
            {
                var currentChar = this.WholeText[index];

                if (currentChar == StartChar)
                {
                    index += 1;
                    continue;
                }
                else if (currentChar == EndChar)
                {
                    this.CurrentIndex = index;
                    return result;
                }
                else
                {
                    result += currentChar;
                }
                index += 1;
            }
            return null;
        }

        public DataItem ReadDataItem()
        {
            DataItem item = new DataItem();
            bool inSection = false;

            int startRead = this.CurrentIndex;

            while (CurrentIndex < TotalLen)
            {
                var currentChar = this.WholeText[CurrentIndex];

                if (Lib.Helper.CharHelper.isSpaceCharacters(currentChar))
                {
                    if (!inSection)
                    {
                        if (item.Name == null)
                        {
                            CurrentIndex += 1;
                            continue;
                        }
                        else
                        {
                            return EmitItem(item, startRead);
                        }
                    }
                    else
                    {
                        //TOCHECK: section part...????  start another section?? can a body has multiple sections? and how they seperated?
                        CurrentIndex += 1;
                        continue;
                    }
                }
                else
                {
                    if (!inSection)
                    {
                        if (currentChar == '[')
                        {
                            if (item.Name == null)
                            {
                                throw new Exception("[ must be part of body");
                            }
                            inSection = true;
                            item.Section = new DataItem.BodySection();
                        }
                        else if (currentChar == '<')
                        {
                            //read partial.
                            var parts = this.ReadToNext('<', '>', this.CurrentIndex);
                            var separators = ",;.".ToCharArray();
                            var partialParts = parts.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                            if (partialParts.Count() > 0)
                            {
                                int offset = 0;
                                if (int.TryParse(partialParts[0], out offset))
                                {
                                    item.Partial = new DataItem.Part();
                                    item.Partial.OffSet = offset;

                                    if (partialParts.Count() > 1)
                                    {
                                        int count = 0;

                                        if (int.TryParse(partialParts[1], out count))
                                        {
                                            item.Partial.Count = count;
                                        }
                                    }
                                }
                            }

                        }
                        else if (Lib.Helper.CharHelper.isAlphanumeric(currentChar) || currentChar == '.')
                        {
                            item.Name += currentChar;
                        }
                    }
                    else
                    {
                        if (currentChar == ']') // section end mark... 
                        {
                            inSection = false;
                        }
                        else
                        {
                            if (currentChar == '(')
                            {
                                // read para. 
                                var paras = this.ReadToNext('(', ')', this.CurrentIndex);
                                if (!string.IsNullOrWhiteSpace(paras))
                                {
                                    item.Section.Paras = paras.Split(' ').ToList();
                                }
                            }
                            else
                            {
                                item.Section.PartSpecifier += currentChar;
                            }
                        }
                    }
                    CurrentIndex += 1;
                }
            }

            if (item.Name != null)
            {
                return EmitItem(item, startRead);
            }
            return null;
        }

        private DataItem EmitItem(DataItem item, int startread)
        {
            string text = this.WholeText.Substring(startread, this.CurrentIndex - startread);
            text = text.Trim();
            item.FullItemName = text;
            return item;
        }

        public List<DataItem> ReadAllDataItems()
        {
            List<DataItem> items = new List<DataItem>();
            var next = this.ReadDataItem();

            while (next != null)
            {
                items.Add(next);

                next = this.ReadDataItem();
            }

            return items;
        }

        public class DataItem
        {
            // Data item or Marco name
            public string Name { get; set; }

            // for body...    
            public BodySection Section { get; set; }

            public Part Partial { get; set; }

            public class Part
            {
                public int OffSet { get; set; }

                public int Count { get; set; }
            }

            public class BodySection
            {
                public string PartSpecifier { get; set; }

                public List<string> Paras
                {
                    get; set;
                }
            }

            public string FullItemName
            {
                get; set;
            }

            public bool SinglePartBodyHeader
            { get; set; }

            public string SinglePartHeaderText { get; set; }


        }
    }
}


//6.4.5.  FETCH Command

//   Arguments:  sequence set
//               message data item names or macro

//   Responses:  untagged responses: FETCH

//   Result:     OK - fetch completed
//               NO - fetch error: can't fetch that data
//               BAD - command unknown or arguments invalid

//      The FETCH command retrieves data associated with a message in the
//      mailbox.The data items to be fetched can be either a single atom
//      or a parenthesized list.

//      Most data items, identified in the formal syntax under the
//      msg-att-static rule, are static and MUST NOT change for any
//      particular message.Other data items, identified in the formal
//      syntax under the msg-att-dynamic rule, MAY change, either as a
//      result of a STORE command or due to external events.

//           For example, if a client receives an ENVELOPE for a
//           message when it already knows the envelope, it can
//           safely ignore the newly transmitted envelope.

//      There are three macros which specify commonly-used sets of data
//      items, and can be used instead of data items.A macro must be
//      used by itself, and not in conjunction with other macros or data
//      items.





//Crispin Standards Track[Page 54]

//RFC 3501                         IMAPv4 March 2003


//      ALL
//         Macro equivalent to: (FLAGS INTERNALDATE RFC822.SIZE ENVELOPE)

//      FAST
//         Macro equivalent to: (FLAGS INTERNALDATE RFC822.SIZE)

//      FULL
//         Macro equivalent to: (FLAGS INTERNALDATE RFC822.SIZE ENVELOPE
//         BODY)

//      The currently defined data items that can be fetched are:

//      BODY
//         Non-extensible form of BODYSTRUCTURE.

//      BODY[<section>]<<partial>>
//         The text of a particular body section.The section
//         specification is a set of zero or more part specifiers
//         delimited by periods.A part specifier is either a part number
//         or one of the following: HEADER, HEADER.FIELDS,
//         HEADER.FIELDS.NOT, MIME, and TEXT.  An empty section
//         specification refers to the entire message, including the
//         header.

//         Every message has at least one part number.  Non-[MIME-IMB]
//         messages, and non-multipart[MIME - IMB] messages with no
//         encapsulated message, only have a part 1.

//         Multipart messages are assigned consecutive part numbers, as
//         they occur in the message.  If a particular part is of type
//         message or multipart, its parts MUST be indicated by a period
//         followed by the part number within that nested multipart part.

//         A part of type MESSAGE/RFC822 also has nested part numbers,
//         referring to parts of the MESSAGE part's body.

//         The HEADER, HEADER.FIELDS, HEADER.FIELDS.NOT, and TEXT part
//         specifiers can be the sole part specifier or can be prefixed by
//         one or more numeric part specifiers, provided that the numeric
//         part specifier refers to a part of type MESSAGE/RFC822.The
//         MIME part specifier MUST be prefixed by one or more numeric
//         part specifiers.

//         The HEADER, HEADER.FIELDS, and HEADER.FIELDS.NOT part
//         specifiers refer to the [RFC-2822] header of the message or of
//         an encapsulated [MIME-IMT] MESSAGE/RFC822 message.
//         HEADER.FIELDS and HEADER.FIELDS.NOT are followed by a list of
//         field-name (as defined in [RFC-2822]) names, and return a



//Crispin                     Standards Track[Page 55]


//RFC 3501                         IMAPv4 March 2003


//         subset of the header.The subset returned by HEADER.FIELDS
//         contains only those header fields with a field-name that
//         matches one of the names in the list; similarly, the subset
//         returned by HEADER.FIELDS.NOT contains only the header fields
//         with a non-matching field-name.The field-matching is
//         case-insensitive but otherwise exact.Subsetting does not
//         exclude the [RFC-2822] delimiting blank line between the header
//         and the body; the blank line is included in all header fetches,
//         except in the case of a message which has no body and no blank
//         line.

//         The MIME part specifier refers to the[MIME - IMB] header for
//         this part.

//         The TEXT part specifier refers to the text body of the message,
//         omitting the [RFC-2822] header.

//            Here is an example of a complex message with some of its
//            part specifiers:

//       HEADER     ([RFC-2822] header of the message)
//       TEXT       ([RFC-2822]
//text body of the message) MULTIPART/MIXED
//       1          TEXT/PLAIN
//       2          APPLICATION/OCTET-STREAM
//       3          MESSAGE/RFC822
//       3.HEADER   ([RFC-2822]
//header of the message)
//       3.TEXT     ([RFC-2822]
//text body of the message) MULTIPART/MIXED
//       3.1        TEXT/PLAIN
//       3.2        APPLICATION/OCTET-STREAM
//       4          MULTIPART/MIXED
//       4.1        IMAGE/GIF
//       4.1.MIME   ([MIME-IMB]
//header for the IMAGE/GIF)
//       4.2        MESSAGE/RFC822
//       4.2.HEADER ([RFC-2822]
//header of the message)
//       4.2.TEXT   ([RFC-2822]
//text body of the message) MULTIPART/MIXED
//       4.2.1      TEXT/PLAIN
//       4.2.2      MULTIPART/ALTERNATIVE
//       4.2.2.1    TEXT/PLAIN
//       4.2.2.2    TEXT/RICHTEXT


//         It is possible to fetch a substring of the designated text.
//         This is done by appending an open angle bracket ("<"), the
//         octet position of the first desired octet, a period, the
//         maximum number of octets desired, and a close angle bracket
//         (">") to the part specifier.  If the starting octet is beyond
//         the end of the text, an empty string is returned.




//Crispin Standards Track [Page 56]

//RFC 3501                         IMAPv4 March 2003


//         Any partial fetch that attempts to read beyond the end of the
//         text is truncated as appropriate.  A partial fetch that starts
//         at octet 0 is returned as a partial fetch, even if this
//         truncation happened.

//            Note: This means that BODY []<0.2048> of a 1500-octet message
//            will return BODY []<0> with a literal of size 1500, not
//            BODY [].

//            Note: A substring fetch of a HEADER.FIELDS or
//            HEADER.FIELDS.NOT part specifier is calculated after
//            subsetting the header.

//         The \Seen flag is implicitly set; if this causes the flags to
//         change, they SHOULD be included as part of the FETCH responses.

//      BODY.PEEK [<section>]<<partial>>
//         An alternate form of BODY [<section>]
//that does not implicitly
//         set the \Seen flag.

//      BODYSTRUCTURE
//         The[MIME - IMB] body structure of the message.This is computed
//         by the server by parsing the[MIME - IMB] header fields in the
//          [RFC - 2822] header and[MIME - IMB] headers.

//          ENVELOPE
//             The envelope structure of the message.This is computed by the
//         server by parsing the[RFC - 2822] header into the component
//         parts, defaulting various fields as necessary.

//      FLAGS
//         The flags that are set for this message.

//      INTERNALDATE
//         The internal date of the message.

//      RFC822
//         Functionally equivalent to BODY[], differing in the syntax of
//         the resulting untagged FETCH data (RFC822 is returned).

//      RFC822.HEADER
//         Functionally equivalent to BODY.PEEK[HEADER], differing in the
//         syntax of the resulting untagged FETCH data(RFC822.HEADER is
//         returned).

//      RFC822.SIZE
//         The[RFC - 2822] size of the message.




// Crispin                     Standards Track                    [Page 57]

//RFC 3501                         IMAPv4 March 2003



//       RFC822.TEXT
//          Functionally equivalent to BODY[TEXT], differing in the syntax

//          of the resulting untagged FETCH data (RFC822.TEXT is returned).

//      UID
//         The unique identifier for the message.


//   Example:    C: A654 FETCH 2:4 (FLAGS BODY[HEADER.FIELDS(DATE FROM)])
//               S: * 2 FETCH....
//               S: * 3 FETCH....
//               S: * 4 FETCH....
//               S: A654 OK FETCH completed