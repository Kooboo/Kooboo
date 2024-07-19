//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using static Kooboo.Mail.Imap.Commands.SearchCommand.SearchKeyword.Argument;

namespace Kooboo.Mail.Imap.Commands.SearchCommand
{
    public class CommandReader
    {
        internal string WholeText { get; set; }

        internal int CurrentIndex { get; set; } = 0;

        internal int TotalLen { get; set; }

        public CommandReader(string CommandText)
        {
            this.WholeText = CommandText;
            this.TotalLen = this.WholeText.Length;
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

        internal string ReadToEnd()
        {
            string value = this.WholeText.Substring(this.CurrentIndex);
            this.CurrentIndex = this.TotalLen;
            if (value != null)
            {
                value.Trim();
            }
            return null;
        }


        public void ReConsume(string token)
        {
            this._retoken = token;
        }

        private string _retoken { get; set; }

        public string ConsumeNextToken()
        {
            string token = null;

            if (this._retoken != null)
            {
                token = this._retoken;
                this._retoken = null;
                return token;
            }

            while (CurrentIndex < TotalLen)
            {
                var currentChar = this.WholeText[CurrentIndex];

                if (Lib.Helper.CharHelper.isSpaceCharacters(currentChar))
                {
                    if (token != null)
                    {
                        string value = token;
                        token = null;
                        return value;
                    }
                }
                else if (currentChar == '"' || currentChar == '\'')
                {
                    var value = ReadQuoted(currentChar, this.CurrentIndex);
                    token += value;
                }
                // todo, read the ""  quoted text. 
                else
                {
                    token += currentChar;
                }
                CurrentIndex += 1;
            }
            return token;
        }

        internal string ReadQuoted(char QuoteChar, int index)
        {
            string result = null;

            int orgindex = index;

            while (index < TotalLen)
            {
                var currentChar = this.WholeText[index];

                if (currentChar == QuoteChar)
                {
                    result += currentChar;
                    if (orgindex == index)
                    {
                        index += 1;
                        continue;
                    }
                    else
                    {
                        this.CurrentIndex = index;
                        return result;
                    }
                }
                else
                {
                    result += currentChar;
                }
                index += 1;
            }
            return null;
        }

        public SearchItem ReadItem()
        {
            var token = ConsumeNextToken();

            if (token != null)
            {
                var keyword = Keywords.Find(token);
                if (keyword == null)
                {
                    return ReadItem();
                }

                SearchItem result = new SearchItem() { Name = keyword.Name, AsCollection = keyword.UseAsCollection };

                if (keyword.Type == SearchType.NOT)
                {
                    result.Type = SearchType.NOT;
                    result.NOT = this.ReadItem();
                    return result;
                }
                else if (keyword.Type == SearchType.OR)
                {
                    result.Type = SearchType.OR;
                    result.OROne = this.ReadItem();
                    result.ORTwo = this.ReadItem();
                    return result;
                }
                else
                {
                    if (keyword.Arguments == null || keyword.Arguments.Count() == 0)
                    {
                        return result;
                    }
                    else
                    {
                        foreach (var item in keyword.Arguments)
                        {
                            var next = this.ConsumeNextToken();

                            if (!item.Optional)
                            {
                                var rightvalue = ToRightDataType(next, item.DataType);
                                if (rightvalue != null)
                                {
                                    result.Parameters.Add(item.FieldName, rightvalue);
                                }
                            }
                            else
                            {
                                var find = Keywords.Find(next);
                                if (find != null)
                                {
                                    this.ReConsume(next);
                                    return result;
                                }
                                else
                                {
                                    // convert to right DataType.  
                                    var rightvalue = ToRightDataType(next, item.DataType);
                                    result.Parameters.Add(item.FieldName, rightvalue);
                                }
                            }
                        }

                        return result;
                    }
                }
            }

            return null;
        }

        private object ToRightDataType(string value, SearchDataType type)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            if (type == SearchDataType.Text)
            {
                return value;
            }
            else if (type == SearchDataType.Number)
            {
                int intvalue = 0;

                if (int.TryParse(value, out intvalue))
                {
                    return intvalue;
                }
                else
                {
                    return null;
                }
            }
            else if (type == SearchDataType.Date)
            {
                return ImapHelper.ParseRfc2822Time(value);
            }
            return null;
        }

        public List<SearchItem> ReadAllDataItems()
        {
            List<SearchItem> items = new List<SearchItem>();
            var next = this.ReadItem();

            while (next != null && next.Name != null)
            {
                items.Add(next);

                next = this.ReadItem();
            }
            return items;
        }

    }
}




//The defined search keys are as follows.Refer to the Formal
//      Syntax section for the precise syntactic definitions of the
//      arguments.

//      <sequence set>
//         Messages with message sequence numbers corresponding to the
//         specified message sequence number set.

//      ALL
//         All messages in the mailbox; the default initial key for
//         ANDing.

//      ANSWERED
//         Messages with the \Answered flag set.

//Crispin Standards Track[Page 50]

//RFC 3501                         IMAPv4 March 2003

//      BCC<string>
//         Messages that contain the specified string in the envelope
//         structure's BCC field.

//      BEFORE<date>
//         Messages whose internal date(disregarding time and timezone)
//         is earlier than the specified date.

//      BODY<string>
//         Messages that contain the specified string in the body of the
//         message.

//      CC<string>
//         Messages that contain the specified string in the envelope
//         structure's CC field.

//      DELETED
//         Messages with the \Deleted flag set.

//      DRAFT
//         Messages with the \Draft flag set.

//      FLAGGED
//         Messages with the \Flagged flag set.

//      FROM<string>
//         Messages that contain the specified string in the envelope
//         structure's FROM field.

//      HEADER<field-name> <string>
//         Messages that have a header with the specified field-name(as
//         defined in [RFC-2822]) and that contains the specified string
//         in the text of the header(what comes after the colon).  If the
//         string to search is zero-length, this matches all messages that
//         have a header line with the specified field-name regardless of
//         the contents.

//      KEYWORD<flag>
//         Messages with the specified keyword flag set.

//      LARGER<n>
//         Messages with an [RFC-2822] size larger than the specified
//         number of octets.

//      NEW
//         Messages that have the \Recent flag set but not the \Seen flag.
//         This is functionally equivalent to "(RECENT UNSEEN)".

//Crispin Standards Track[Page 51]

//RFC 3501                         IMAPv4 March 2003

//      NOT<search-key>
//         Messages that do not match the specified search key.

//      OLD
//         Messages that do not have the \Recent flag set.This is
//         functionally equivalent to "NOT RECENT" (as opposed to "NOT
//         NEW").

//      ON<date>
//         Messages whose internal date(disregarding time and timezone)
//         is within the specified date.

//      OR<search-key1> <search-key2>
//         Messages that match either search key.

//      RECENT
//         Messages that have the \Recent flag set.

//      SEEN
//         Messages that have the \Seen flag set.

//      SENTBEFORE<date>
//         Messages whose[RFC - 2822] Date: header (disregarding time and
//         timezone) is earlier than the specified date.

//      SENTON<date>
//         Messages whose[RFC - 2822] Date: header(disregarding time and
//          timezone) is within the specified date.

//       SENTSINCE<date>

//          Messages whose [RFC-2822] Date: header (disregarding time and
//          timezone) is within or later than the specified date.

//       SINCE<date>
//          Messages whose internal date(disregarding time and timezone)
//         is within or later than the specified date.

//      SMALLER<n>
//         Messages with an[RFC - 2822] size smaller than the specified
//          number of octets. 

// Crispin                     Standards Track                    [Page 52]

//RFC 3501                         IMAPv4 March 2003


//       SUBJECT<string>
//          Messages that contain the specified string in the envelope

//          structure's SUBJECT field.


//       TEXT<string>
//          Messages that contain the specified string in the header or
//          body of the message.

//       TO<string>
//          Messages that contain the specified string in the envelope

//          structure's TO field. 
//       UID<sequence set>
//          Messages with unique identifiers corresponding to the specified
//          unique identifier set.  Sequence set ranges are permitted.

//       UNANSWERED
//          Messages that do not have the \Answered flag set.

//       UNDELETED
//          Messages that do not have the \Deleted flag set.

//       UNDRAFT
//          Messages that do not have the \Draft flag set.

//       UNFLAGGED
//          Messages that do not have the \Flagged flag set.

//       UNKEYWORD<flag>
//          Messages that do not have the specified keyword flag set.

//       UNSEEN
//          Messages that do not have the \Seen flag set.