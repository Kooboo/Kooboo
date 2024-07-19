//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Dom;

namespace Kooboo.Mail.Imap.Commands.SearchCommand
{
    public class Search
    {
        public static Search Instance { get; set; } = new Search();

        public List<ImapResponse> ExecuteBySeqNo(MailDb maildb, SelectFolder Folder, string args)
        {
            return Execute(maildb, Folder, args, SearchReturnType.SeqNO);
        }

        public List<ImapResponse> ExecuteByUid(MailDb maildb, SelectFolder Folder, string args)
        {
            return Execute(maildb, Folder, args, SearchReturnType.UID);
        }

        public List<ImapResponse> Execute(MailDb maildb, SelectFolder Folder, string args, SearchReturnType returnType)
        {
            var cmdreader = new SearchCommand.CommandReader(args);

            var allitems = cmdreader.ReadAllDataItems();

            var searchResult = Execute(maildb, Folder, allitems, returnType);

            List<ImapResponse> result = new List<ImapResponse>();

            var line = ResultLine.SEARCH(searchResult);
            result.Add(new ImapResponse(line));
            return result;
        }

        public List<int> ExecuteBySeqNo(MailDb maildb, SelectFolder Folder, List<SearchItem> SearchItems)
        {
            return Execute(maildb, Folder, SearchItems, SearchReturnType.SeqNO);
        }

        public List<int> Execute(MailDb maildb, SelectFolder Folder, List<SearchItem> SearchItems, SearchReturnType returnType)
        {
            // return list of seqno.  
            //var StartCol = FindStartCollectionItems(ref SearchItems);
            //var Msgs = GetCollection(maildb, Folder, StartCol);

            var Msgs = QueryRangeTerms(maildb, Folder, ref SearchItems);

            List<int> result = new List<int>();

            if (SearchItems.Any())
            {
                foreach (var msg in Msgs)
                {
                    bool match = true;

                    foreach (var searchitem in SearchItems)
                    {
                        if (!Check(maildb, Folder, searchitem, msg))
                        {
                            match = false;
                            break;
                        }
                    }

                    if (match)
                    {
                        result.Add(ToResult(msg));
                    }
                }
            }
            else
            {
                foreach (var msg in Msgs)
                {
                    result.Add(ToResult(msg));
                }
            }


            return result;

            int ToResult(Message message)
            {
                if (returnType == SearchReturnType.UID)
                {
                    return (int)message.MsgId;
                }
                else
                {
                    if (message.Id > 0 && message.Id != message.MsgId)
                    {
                        return message.Id;
                    }
                    else
                    {
                        return maildb.Message2.GetSeqNo(Folder, message.MsgId);
                    }
                }
            }
        }

        public List<Message> QueryByUid(MailDb maildb, SelectFolder folder, SearchItem searchitem)
        {
            if (searchitem.Name == "UID")
            {
                var set = searchitem.Parameters["SEQUENCE-SET"].ToString();
                var range = Kooboo.Mail.Imap.ImapHelper.GetSequenceRange(set);
                ImapHelper.CorrectRange(range, folder, true);

                var result = new List<Message>();
                foreach (var item in range)
                {
                    var messagesInRange = maildb.Message2.ByUidRange(folder, item.LowBound, item.UpBound);
                    int seqno = -1;

                    foreach (var message in messagesInRange)
                    {
                        if (seqno == -1)
                        {
                            seqno = maildb.Message2.GetSeqNo(folder, message.MsgId);
                        }

                        message.Id = seqno;

                        seqno += 1;
                        result.Add(message);
                    }
                }

                return result;
            }
            return null;
        }

        public List<Message> QueryByRange(MailDb maildb, SelectFolder folder, SearchItem searchItem)
        {
            if (searchItem != null && searchItem.Name == "UID")
            {
                return QueryByUid(maildb, folder, searchItem);
            }

            var query = maildb.Message2.Query.Where(o => o.FolderId == folder.FolderId);
            if (folder.AddressId != 0)
            {
                query.Where(o => o.AddressId == folder.AddressId);
            }

            AddCondition(maildb, folder, query, searchItem);

            List<Message> result = new List<Message>();

            query.OrderByAscending(o => o.MsgId);

            int seqno = -1;
            var messagesInRange = query.SelectAll();

            foreach (var message in messagesInRange)
            {
                if (seqno == -1)
                {
                    seqno = maildb.Message2.GetSeqNo(folder, message.MsgId);
                }
                message.Id = seqno;
                seqno += 1;
                result.Add(message);
            }
            return result;
        }

        public List<Message> QueryRangeTerms(MailDb maildb, SelectFolder folder, ref List<SearchItem> items)
        {
            var find = items.Find(o => o.Name == "UID");
            if (find != null)
            {
                items.Remove(find);
                return QueryByUid(maildb, folder, find);
            }

            var partialKeys = GetRangeKeys();

            find = items.Find(o => o.Name.isOneOf(partialKeys));

            if (find != null)
            {
                items.Remove(find);
                return QueryByRange(maildb, folder, find);
            }

            var query = maildb.Message2.Query.Where(o => o.FolderId == folder.FolderId);
            if (folder.AddressId != 0)
            {
                query.Where(o => o.AddressId == folder.AddressId);
            }

            if (items == null)
            {
                return query.SelectAll();
            }

            List<SearchItem> KeepItems = new List<SearchItem>();

            foreach (var item in items)
            {
                if (item.Name.isOneOf(GetAllWhereKeyWords()))
                {
                    AddCondition(maildb, folder, query, item);
                }
                else if (item.Name == "NOT" && item.NOT != null && item.NOT.Name.isOneOf(GetAllWhereKeyWords()))
                {
                    query.AddOperator(SqlWhere<Message>.OperatorType.NOT);
                    AddCondition(maildb, folder, query, item.NOT);
                }
                else
                {
                    KeepItems.Add(item);
                }
            }

            return query.SelectAll();

        }

        private void addFlagCondition(SqlWhere<Message> query, string ItemName)
        {
            if (ItemName == null)
            {
                return;
            }

            switch (ItemName)
            {
                case "ALL":
                    {
                        // donothing.
                        return;
                    }
                case "ANSWERED":
                    {
                        query.Where(o => o.Answered == true);
                        return;
                    }
                case "DELETED":
                    {
                        query.Where(o => o.Deleted == true);
                        return;
                    }
                case "DRAFT":
                    {
                        query.Where(o => o.Draft == true);
                        return;
                    }
                case "FLAGGED":
                    {
                        query.Where(o => o.Flagged == true);
                        return;
                    }
                case "NEW":
                    {
                        query.Where(o => o.Recent == true);
                        return;
                    }
                case "OLD":
                    {
                        query.Where(o => o.Recent == false);
                        return;
                    }
                case "RECENT":
                    {
                        query.Where(o => o.Recent == true);
                        return;
                    }
                case "SEEN":
                    {
                        query.Where(o => o.Read == true);
                        return;
                    }
                ///UNANSWERED UNDELETED UNDRAFT UNFLAGGED UNSEEN 
                case "UNANSWERED":
                    {
                        query.Where(o => o.Answered == false);
                        return;
                    }
                case "UNDELETED":
                    {
                        query.Where(o => o.Deleted == false);
                        return;
                    }
                case "UNDRAFT":
                    {
                        query.Where(o => o.Draft == false);
                        return;
                    }
                case "UNFLAGGED":
                    {
                        query.Where(o => o.Flagged == false);
                        return;
                    }
                case "UNSEEN":
                    {
                        query.Where(o => o.Read == false);
                        return;
                    }
            }
        }

        private void AddCondition(MailDb maildb, SelectFolder folder, SqlWhere<Message> query, SearchItem term)
        {
            if (term == null)
            {
                return;
            }
            if (term.Name == "LARGER" || term.Name == "SMALLER")
            {
                int value = (int)term.Parameters["N"];

                var msg = maildb.Message2.GetBySeqNo(folder, value);
                if (msg == null)
                {
                    throw new CommandException("NO", "not a valid sequence no. ");
                }

                if (term.Name == "LARGER")
                {
                    query.Where(o => o.MsgId > msg.MsgId);
                }
                else if (term.Name == "SMALLER")
                {
                    query.Where(o => o.MsgId < msg.MsgId);
                }
            }

            else if (term.Name == "BEFORE" || term.Name == "SENTBEFORE")
            {
                var date = (DateTime)term.Parameters["DATE"];
                var tick = date.Ticks;
                query.Where(o => o.CreationTimeTick < tick);
            }
            else if (term.Name == "SENTSINCE" || term.Name == "SINCE")
            {
                var date = (DateTime)term.Parameters["DATE"];
                var tick = date.Ticks;
                query.Where(o => o.CreationTimeTick > tick);
            }
            else if (term.Name == "ON" || term.Name == "SENTON")
            {
                var date = (DateTime)term.Parameters["DATE"];
                var dateStart = date.Date.Ticks;
                var dateEnd = date.AddDays(1).Date.AddTicks(-1).Ticks;
                query.Where(o => o.CreationTimeTick > dateStart && o.CreationTimeTick < dateEnd);
            }
            else if (term.Name.isOneOf(GetFlags()))
            {
                addFlagCondition(query, term.Name);
            }
        }

        public string[] GetFlags()
        {
            string strFlags = "ALL,ANSWERED,DELETED,DRAFT,FLAGGED,NEW,OLD,RECENT,SEEN,UNANSWERED,UNDELETED,UNDRAFT,UNFLAGGED,UNSEEN";
            return strFlags.Split(',', StringSplitOptions.RemoveEmptyEntries);
        }

        public string[] GetRangeKeys()
        {
            string supportKeywords = "LARGER,SMALLER,BEFORE,SENTBEFORE,SENTSINCE,SINCE,ON,SENTON";

            return supportKeywords.Split(',', StringSplitOptions.RemoveEmptyEntries);
        }

        public string[] GetAllWhereKeyWords()
        {
            var flags = GetFlags();

            var extraWords = GetRangeKeys();

            extraWords.ToList().AddRange(flags);

            return extraWords;
        }

        public bool Check(MailDb mailDb, SelectFolder folder, SearchItem item, Message message)
        {
            //ALL ANSWERED DELETED DRAFT FLAGGED NEW OLD RECENT SEEN UNANSWERED UNDELETED UNDRAFT UNFLAGGED UNSEEN 
            switch (item.Name)
            {
                case "ALL":
                    {
                        return true;
                    }
                case "ANSWERED":
                    {
                        return message.Answered;
                    }
                case "DELETED":
                    {
                        return message.Deleted;
                    }
                case "DRAFT":
                    {
                        return message.Draft;
                    }
                case "FLAGGED":
                    {
                        return message.Flagged;
                    }
                case "NEW":
                    {
                        return message.Recent;
                    }
                case "OLD":
                    {
                        return !message.Recent;
                    }
                case "RECENT":
                    {
                        return message.Recent;
                    }
                case "SEEN":
                    {
                        return message.Read;
                    }
                ///UNANSWERED UNDELETED UNDRAFT UNFLAGGED UNSEEN 
                case "UNANSWERED":
                    {
                        return !message.Answered;
                    }
                case "UNDELETED":
                    {
                        return !message.Deleted;
                    }
                case "UNDRAFT":
                    {
                        return !message.Draft;
                    }
                case "UNFLAGGED":
                    {
                        return !message.Flagged;
                    }
                case "UNSEEN":
                    {
                        return !message.Read;
                    }
                case "BCC":
                    {
                        return CheckText(mailDb, message, item.Name, item.Parameters.First().Value.ToString());
                    }
                ///"BCC BODY CC FROM SUBJECT TEXT TO"; 
                case "CC":
                    {
                        return CheckText(mailDb, message, item.Name, item.Parameters.First().Value.ToString());
                    }
                case "TO":
                    {
                        return CheckText(mailDb, message, item.Name, item.Parameters.First().Value.ToString());
                    }
                case "FROM":
                    {
                        return CheckText(mailDb, message, item.Name, item.Parameters.First().Value.ToString());
                    }
                case "SUBJECT":
                    {
                        return CheckText(mailDb, message, item.Name, item.Parameters.First().Value.ToString());
                    }
                case "TEXT":
                    {
                        return CheckText(mailDb, message, item.Name, item.Parameters.First().Value.ToString());
                    }
                case "BODY":
                    {
                        return CheckText(mailDb, message, item.Name, item.Parameters.First().Value.ToString());
                    }
                case "HEADER":
                    {
                        var field = item.Parameters["FIELD-NAME"].ToString();
                        string value = null;
                        if (item.Parameters.ContainsKey("STRING"))
                        {
                            value = item.Parameters["STRING"].ToString();
                        }
                        return CheckText(mailDb, message, field, value);
                    }
                ///string dateKey = "BEFORE SENTBEFORE SENTON SENTSINCE SINCE ON";
                case "BEFORE":
                    {
                        return CheckDate(mailDb, message, item.Name, (DateTime)item.Parameters["DATE"]);
                    }
                case "SENTBEFORE":
                    {
                        return CheckDate(mailDb, message, item.Name, (DateTime)item.Parameters["DATE"]);
                    }
                case "SENTON":
                    {
                        return CheckDate(mailDb, message, item.Name, (DateTime)item.Parameters["DATE"]);
                    }
                case "SENTSINCE":
                    {
                        return CheckDate(mailDb, message, item.Name, (DateTime)item.Parameters["DATE"]);
                    }
                case "SINCE":
                    {
                        return CheckDate(mailDb, message, item.Name, (DateTime)item.Parameters["DATE"]);
                    }
                case "ON":
                    {
                        return CheckDate(mailDb, message, item.Name, (DateTime)item.Parameters["DATE"]);
                    }
                case "SMALLER":
                    {
                        if (item.SeqCompareUid == -1)
                        {
                            int value = (int)item.Parameters["N"];
                            var msg = mailDb.Message2.GetBySeqNo(folder, value);
                            if (msg != null)
                            {
                                item.SeqCompareUid = (int)msg.MsgId;
                            }
                            else
                            {
                                item.SeqCompareUid = 0;
                            }
                        }
                        return message.MsgId < item.SeqCompareUid;
                    }

                case "LARGER":
                    {
                        if (item.SeqCompareUid == -1)
                        {
                            int value = (int)item.Parameters["N"];
                            var msg = mailDb.Message2.GetBySeqNo(folder, value);

                            if (msg != null)
                            {
                                item.SeqCompareUid = (int)msg.MsgId;
                            }
                            else
                            {
                                item.SeqCompareUid = 0;
                            }
                        }
                        return message.MsgId > item.SeqCompareUid;
                    }

                case "NOT":
                    {
                        if (item.NOT != null)
                        {
                            return !Check(mailDb, folder, item.NOT, message);
                        }
                        else
                        {
                            return true;
                        }
                    }

                case "OR":
                    {
                        if (item.OROne == null && item.ORTwo == null)
                        {
                            return false;
                        }

                        bool match = false;
                        if (item.OROne != null)
                        {
                            if (Check(mailDb, folder, item.OROne, message))
                            {
                                match = true;
                            }
                        }

                        if (item.ORTwo != null)
                        {
                            if (Check(mailDb, folder, item.ORTwo, message))
                            {
                                match = true;
                            }
                        }
                        return match;
                    }

                case "KEYWORD":
                    {
                        var value = item.Parameters["FLAG"].ToString();

                        foreach (var flag in mailDb.Message2.GetFlags(message.MsgId))
                        {
                            if (flag.ToUpper() == value)
                            {
                                return true;
                            }
                        }
                        return false;
                    }

                default:
                    break;
            }

            return true;
        }

        public DateTime ConvertToDate(string RFCDate)
        {
            DateTime result;

            if (DateTime.TryParse(RFCDate, out result))
            {
                return result;
            }
            return default(DateTime);
        }

        public bool CheckText(MailDb maildb, Message message, string HeaderField, string CompareValue)
        {
            // BCC BODY CC FROM SUBJECT TEXT TO
            var upper = HeaderField.ToUpper();
            if (upper == "TO")
            {
                if (!string.IsNullOrEmpty(CompareValue))
                {
                    return message.To.Contains(CompareValue);
                }
                return true;
            }
            else if ((upper == "CC"))
            {
                if (!string.IsNullOrEmpty(CompareValue))
                {
                    return message.Cc.IndexOf(CompareValue, StringComparison.OrdinalIgnoreCase) >= 0;
                }
                return true;
            }
            else if (upper == "FROM")
            {
                if (!string.IsNullOrEmpty(CompareValue))
                {
                    return message.From.IndexOf(CompareValue, StringComparison.OrdinalIgnoreCase) >= 0;
                }
                return true;
            }

            else if ((upper == "BCC"))
            {
                if (!string.IsNullOrEmpty(CompareValue))
                {
                    return message.Bcc.IndexOf(CompareValue, StringComparison.OrdinalIgnoreCase) >= 0;
                }
                return true;
            }

            else if ((upper == "SUBJECT"))
            {
                if (!string.IsNullOrEmpty(CompareValue))
                {
                    return message.Subject.IndexOf(CompareValue, StringComparison.OrdinalIgnoreCase) >= 0;
                }
                return true;
            }
            else
            {
                var content = message.Body;
                if (string.IsNullOrEmpty(content))
                {
                    content = maildb.Message2.GetContent(message.MsgId);
                }

                if (upper == "TEXT")
                {
                    if (!string.IsNullOrEmpty(CompareValue))
                    {
                        return content.IndexOf(CompareValue, StringComparison.OrdinalIgnoreCase) >= 0;
                    }
                }
                else
                {
                    int index = content.IndexOf("\r\n\r\n");

                    if (upper == "BODY")
                    {
                        if (!string.IsNullOrEmpty(CompareValue))
                        {
                            if (index >= 0)
                            {
                                string body = content.Substring(index);
                                return body.IndexOf(CompareValue, StringComparison.OrdinalIgnoreCase) >= 0;
                            }
                        }
                        return true;
                    }
                    else
                    {
                        string headerpart = null;
                        if (index > -1)
                        {
                            headerpart = content.Substring(0, index);
                        }
                        else
                        { headerpart = content; }

                        var headerline = GetHeaderFieldLine(headerpart, HeaderField);

                        if (string.IsNullOrEmpty(headerline))
                        {
                            return false;
                        }

                        if (!string.IsNullOrEmpty(CompareValue))
                        {
                            return headerline.IndexOf(CompareValue, StringComparison.OrdinalIgnoreCase) >= 0;
                        }

                        return true;
                    }

                }
            }
            return false;
        }

        internal string GetHeaderFieldLine(string header, string Field)
        {
            if (header.StartsWith(Field, StringComparison.OrdinalIgnoreCase))
            {
                int nextend = header.IndexOf("\r\n", Field.Length);
                if (nextend > -1)
                {
                    return header.Substring(0, nextend);
                }
                else
                {
                    return header;
                }
            }

            if (!Field.StartsWith("\r\n"))
            {
                Field = "\r\n" + Field;
            }

            int start = header.IndexOf(Field, StringComparison.OrdinalIgnoreCase);

            if (start > -1)
            {
                string line = null;
                int end = header.IndexOf("\r\n", start + Field.Length);
                if (end > -1)
                {
                    line = header.Substring(start, end - start);
                }
                else
                {
                    line = header.Substring(start);
                }
                return line;
            }
            return null;
        }

        public bool CheckDate(MailDb maildb, Message message, string Keyword, DateTime Value)
        {
            /// BEFORE SENTBEFORE SENTON SENTSINCE SINCE ON 
            if (Keyword == "BEFORE" || Keyword == "SENTBEFORE")
            {
                return message.CreationTime < Value;
            }
            else if (Keyword == "SINCE" || Keyword == "SENTSINCE")
            {
                return message.CreationTime > Value;
            }
            else if (Keyword == "ON" || Keyword == "SENTON")
            {
                return message.CreationTime.DayOfYear == Value.DayOfYear && message.CreationTime.Year == Value.Year;
            }
            return false;
        }
    }

    public enum SearchReturnType
    {
        SeqNO = 1,
        UID = 2
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