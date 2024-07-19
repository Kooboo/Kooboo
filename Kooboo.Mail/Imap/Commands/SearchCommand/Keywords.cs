//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.Mail.Imap.Commands.SearchCommand
{
    public static class Keywords
    {
        private static List<SearchKeyword> _list;

        public static List<SearchKeyword> List
        {
            get
            {
                if (_list == null)
                {
                    _list = new List<SearchKeyword>();

                    string keywordsWithPara = "ALL ANSWERED DELETED DRAFT FLAGGED NEW OLD RECENT SEEN UNANSWERED UNDELETED UNDRAFT UNFLAGGED UNSEEN";

                    string[] words = keywordsWithPara.Split(' ');
                    foreach (var item in words)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            _list.Add(new SearchKeyword() { Name = item });
                        }
                    }

                    string oneStringkey = "BCC BODY CC FROM SUBJECT TEXT TO";

                    foreach (var item in oneStringkey.Split(' '))
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            SearchKeyword textkey = new SearchKeyword();
                            textkey.Name = item;
                            textkey.Arguments = new List<SearchKeyword.Argument>();
                            textkey.Arguments.Add(new SearchKeyword.Argument() { FieldName = "STRING", DataType = SearchKeyword.Argument.SearchDataType.Text });
                            _list.Add(textkey);
                        }
                    }

                    string dateKey = "BEFORE SENTBEFORE SENTON SENTSINCE SINCE ON";

                    foreach (var item in dateKey.Split(' '))
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            SearchKeyword datekey = new SearchKeyword() { Name = item, UseAsCollection = true };
                            datekey.Arguments = new List<SearchKeyword.Argument>();
                            datekey.Arguments.Add(new SearchKeyword.Argument() { FieldName = "DATE", DataType = SearchKeyword.Argument.SearchDataType.Date });
                            _list.Add(datekey);
                        }
                    }

                    SearchKeyword key = new SearchKeyword() { Name = "HEADER" };
                    key.Arguments = new List<SearchKeyword.Argument>();
                    key.Arguments.Add(new SearchKeyword.Argument() { FieldName = "FIELD-NAME", DataType = SearchKeyword.Argument.SearchDataType.Text });
                    key.Arguments.Add(new SearchKeyword.Argument() { FieldName = "STRING", DataType = SearchKeyword.Argument.SearchDataType.Text, Optional = true });
                    _list.Add(key);

                    key = new SearchKeyword() { Name = "KEYWORD" };
                    key.Arguments = new List<SearchKeyword.Argument>();
                    key.Arguments.Add(new SearchKeyword.Argument() { FieldName = "FLAG", DataType = SearchKeyword.Argument.SearchDataType.Text });
                    _list.Add(key);

                    key = new SearchKeyword() { Name = "LARGER", UseAsCollection = true };
                    key.Arguments = new List<SearchKeyword.Argument>();
                    key.Arguments.Add(new SearchKeyword.Argument() { FieldName = "N", DataType = SearchKeyword.Argument.SearchDataType.Number });
                    _list.Add(key);

                    key = new SearchKeyword() { Name = "SMALLER", UseAsCollection = true };
                    key.Arguments = new List<SearchKeyword.Argument>();
                    key.Arguments.Add(new SearchKeyword.Argument() { FieldName = "N", DataType = SearchKeyword.Argument.SearchDataType.Number });
                    _list.Add(key);

                    key = new SearchKeyword() { Name = "UID", UseAsCollection = true };
                    key.Arguments = new List<SearchKeyword.Argument>();
                    key.Arguments.Add(new SearchKeyword.Argument() { FieldName = "SEQUENCE-SET", DataType = SearchKeyword.Argument.SearchDataType.Text });
                    _list.Add(key);

                    key = new SearchKeyword() { Name = "OR", Type = SearchType.OR };
                    key.Arguments = new List<SearchKeyword.Argument>();
                    key.Arguments.Add(new SearchKeyword.Argument() { FieldName = "Searchkeys", DataType = SearchKeyword.Argument.SearchDataType.Text });
                    _list.Add(key);

                    key = new SearchKeyword() { Name = "NOT", Type = SearchType.NOT };
                    key.Arguments = new List<SearchKeyword.Argument>();
                    key.Arguments.Add(new SearchKeyword.Argument() { FieldName = "Searchkey", DataType = SearchKeyword.Argument.SearchDataType.Text });
                    _list.Add(key);
                }

                return _list;
            }
        }

        public static SearchKeyword Find(string name)
        {
            name = name.ToUpper();
            return List.Find(o => o.Name == name);
        }
    }

    public class SearchKeyword
    {
        public string Name { get; set; }

        public List<Argument> Arguments { get; set; }

        public bool UseAsCollection { get; set; }

        public SearchType Type { get; set; } = SearchType.Normal;


        public class Argument
        {
            public string FieldName { get; set; }

            public SearchDataType DataType { get; set; }

            public bool Optional { get; set; }

            public enum SearchDataType
            {
                Text = 0,
                Number = 1,
                Date = 3
            }
        }
    }
}

//ALL
//   All messages in the mailbox; the default initial key for
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
