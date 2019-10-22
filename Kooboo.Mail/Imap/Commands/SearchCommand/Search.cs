//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Mail.Imap.Commands.SearchCommand
{
    public static class Search
    {
        public static List<ImapResponse> ExecuteBySeqNo(MailDb maildb, SelectFolder folder, string args)
        {
            return Execute(maildb, folder, args, (d, f, m) =>
                d.Messages.GetSeqNo(f.Folder, f.Stat.LastestMsgId, f.Stat.Exists, m.Id)
            );
        }

        public static List<ImapResponse> ExecuteByUid(MailDb maildb, SelectFolder folder, string args)
        {
            return Execute(maildb, folder, args, (d, f, m) => m.Id);
        }

        public static List<ImapResponse> Execute(MailDb maildb, SelectFolder folder, string args, Func<MailDb, SelectFolder, Message, int> toResult)
        {
            var cmdreader = new SearchCommand.CommandReader(args);

            var allitems = cmdreader.ReadAllDataItems();

            var searchResult = Execute(maildb, folder, allitems, toResult);

            List<ImapResponse> result = new List<ImapResponse>();

            var line = ResultLine.SEARCH(searchResult);
            result.Add(new ImapResponse(line));

            return result;
        }

        public static List<int> ExecuteBySeqNo(MailDb maildb, SelectFolder folder, List<SearchItem> searchItems)
        {
            return Execute(maildb, folder, searchItems, (d, f, m) =>
                d.Messages.GetSeqNo(f.Folder, f.Stat.LastestMsgId, f.Stat.Exists, m.Id)
            );
        }

        public static List<int> Execute(MailDb maildb, SelectFolder folder, List<SearchItem> searchItems, Func<MailDb, SelectFolder, Message, int> toResult)
        {
            // return list of seqno.
            var startCol = FindStartCollectionItems(ref searchItems);

            var msgs = GetCollection(maildb, folder, startCol);

            List<int> result = new List<int>();

            foreach (var msg in msgs)
            {
                bool match = true;

                foreach (var searchitem in searchItems)
                {
                    if (!Check(maildb, folder, searchitem, msg))
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    result.Add(toResult(maildb, folder, msg));
                }
            }

            return result;
        }

        public static List<Message> GetCollection(MailDb maildb, SelectFolder folder, SearchItem item)
        {
            if (item?.Name == null)
            {
                return maildb.Messages.FolderQuery(folder.Folder).SelectAll();
            }

            if (item.Name == "LARGER" || item.Name == "SMALLER")
            {
                int value = (int)item.Parameters["N"];

                var msg = maildb.Messages.GetBySeqNo(folder.Folder, folder.Stat.LastestMsgId, folder.Stat.Exists, value);

                if (msg == null)
                {
                    throw new CommandException("NO", "not a valid sequence no. ");
                }

                if (item.Name == "LARGER")
                {
                    return maildb.Messages.FolderQuery(folder.Folder).Where(o => o.Id > msg.Id).SelectAll();
                }
                else if (item.Name == "SMALLER")
                {
                    return maildb.Messages.FolderQuery(folder.Folder).Where(o => o.Id < msg.Id).SelectAll();
                }
            }
            else if (item.Name == "UID")
            {
                var set = item.Parameters["SEQUENCE-SET"].ToString();
                var range = Kooboo.Mail.Imap.ImapHelper.GetSequenceRange(set);
                ImapHelper.CorrectRange(range, folder, true);

                List<Message> result = new List<Message>();
                foreach (var uidrange in range)
                {
                    for (int i = uidrange.LowBound; i <= uidrange.UpBound; i++)
                    {
                        var msg = maildb.Messages.Get(i);
                        if (msg != null)
                        {
                            result.Add(msg);
                        }
                    }
                }
                return result;
            }
            else
            {
                if (item.Name == "BEFORE" || item.Name == "SENTBEFORE")
                {
                    var date = (DateTime)item.Parameters["DATE"];
                    var tick = date.Ticks;
                    return maildb.Messages.FolderQuery(folder.Folder).Where(o => o.CreationTimeTick < tick).SelectAll();
                }
                else if (item.Name == "SENTSINCE" || item.Name == "SINCE")
                {
                    var date = (DateTime)item.Parameters["DATE"];
                    var tick = date.Ticks;
                    return maildb.Messages.FolderQuery(folder.Folder).Where(o => o.CreationTimeTick > tick).SelectAll();
                }
                else if (item.Name == "ON" || item.Name == "SENTON")
                {
                    var date = (DateTime)item.Parameters["DATE"];
                    // get the day before and after, then filter... I think this has better performance because there is an index on tick.
                    var before = date.AddDays(-1);
                    var after = date.AddDays(1);

                    var allmessages = maildb.Messages.FolderQuery(folder.Folder).Where(o => o.CreationTimeTick > before.Ticks && o.CreationTimeTick < after.Ticks).SelectAll();

                    if (allmessages != null && allmessages.Any())
                    {
                        return allmessages.Where(o => o.CreationTime.DayOfYear == date.DayOfYear && o.CreationTime.Year == date.Year).ToList();
                    }
                }
                // string dateKey = "BEFORE SENTBEFORE SENTON SENTSINCE SINCE ON";
                // only those left...
            }

            // not collection found, loop all.
            return maildb.Messages.FolderQuery(folder.Folder).SelectAll();
        }

        public static SearchItem FindStartCollectionItems(ref List<SearchItem> items)
        {
            var find = items.Find(o => o.Name == "LARGER" || o.Name == "SMALLER");
            if (find != null)
            {
                items.Remove(find);
                return find;
            }

            find = items.Find(o => o.Name == "UID");
            if (find != null)
            {
                items.Remove(find);
                return find;
            }
            // BEFORE SENTBEFORE SENTON SENTSINCE SINCE ON
            find = items.Find(o => o.Name == "BEFORE" || o.Name == "SENTBEFORE" || o.Name == "SENTON" || o.Name == "SENTSINCE" || o.Name == "SINCE" || o.Name == "ON");
            if (find != null)
            {
                items.Remove(find);
                return find;
            }
            return null;
        }

        public static bool Check(MailDb maildb, SelectFolder folder, SearchItem item, Message message)
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
                //UNANSWERED UNDELETED UNDRAFT UNFLAGGED UNSEEN
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
                        return CheckText(maildb, message, item.Name, item.Parameters.First().Value.ToString());
                    }
                //"BCC BODY CC FROM SUBJECT TEXT TO";
                case "CC":
                    {
                        return CheckText(maildb, message, item.Name, item.Parameters.First().Value.ToString());
                    }
                case "TO":
                    {
                        return CheckText(maildb, message, item.Name, item.Parameters.First().Value.ToString());
                    }
                case "FROM":
                    {
                        return CheckText(maildb, message, item.Name, item.Parameters.First().Value.ToString());
                    }
                case "SUBJECT":
                    {
                        return CheckText(maildb, message, item.Name, item.Parameters.First().Value.ToString());
                    }
                case "TEXT":
                    {
                        return CheckText(maildb, message, item.Name, item.Parameters.First().Value.ToString());
                    }
                case "BODY":
                    {
                        return CheckText(maildb, message, item.Name, item.Parameters.First().Value.ToString());
                    }
                case "HEADER":
                    {
                        var field = item.Parameters["FIELD-NAME"].ToString();
                        string value = null;
                        if (item.Parameters.ContainsKey("STRING"))
                        {
                            value = item.Parameters["STRING"].ToString();
                        }
                        return CheckText(maildb, message, field, value);
                    }
                //string dateKey = "BEFORE SENTBEFORE SENTON SENTSINCE SINCE ON";
                case "BEFORE":
                    {
                        return CheckDate(maildb, message, item.Name, (DateTime)item.Parameters["DATE"]);
                    }
                case "SENTBEFORE":
                    {
                        return CheckDate(maildb, message, item.Name, (DateTime)item.Parameters["DATE"]);
                    }
                case "SENTON":
                    {
                        return CheckDate(maildb, message, item.Name, (DateTime)item.Parameters["DATE"]);
                    }
                case "SENTSINCE":
                    {
                        return CheckDate(maildb, message, item.Name, (DateTime)item.Parameters["DATE"]);
                    }
                case "SINCE":
                    {
                        return CheckDate(maildb, message, item.Name, (DateTime)item.Parameters["DATE"]);
                    }
                case "ON":
                    {
                        return CheckDate(maildb, message, item.Name, (DateTime)item.Parameters["DATE"]);
                    }
                case "SMALLER":
                    {
                        if (item.SeqCompareUid == -1)
                        {
                            int value = (int)item.Parameters["N"];
                            var msg = maildb.Messages.GetBySeqNo(folder.Folder, folder.Stat.LastestMsgId, folder.Stat.Exists, value);
                            item.SeqCompareUid = msg?.Id ?? 0;
                        }
                        return message.Id < item.SeqCompareUid;
                    }

                case "LARGER":
                    {
                        if (item.SeqCompareUid == -1)
                        {
                            int value = (int)item.Parameters["N"];
                            var msg = maildb.Messages.GetBySeqNo(folder.Folder, folder.Stat.LastestMsgId, folder.Stat.Exists, value);

                            item.SeqCompareUid = msg?.Id ?? 0;
                        }
                        return message.Id > item.SeqCompareUid;
                    }

                case "NOT":
                    {
                        if (item.NOT != null)
                        {
                            return !Check(maildb, folder, item.NOT, message);
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
                            if (Check(maildb, folder, item.OROne, message))
                            {
                                match = true;
                            }
                        }

                        if (item.ORTwo != null)
                        {
                            if (Check(maildb, folder, item.ORTwo, message))
                            {
                                match = true;
                            }
                        }
                        return match;
                    }

                case "KEYWORD":
                    {
                        var value = item.Parameters["FLAG"].ToString();

                        foreach (var flag in maildb.Messages.GetFlags(message.Id))
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

        public static DateTime ConvertToDate(string RFCDate)
        {
            DateTime result;

            if (DateTime.TryParse(RFCDate, out result))
            {
                return result;
            }
            return default(DateTime);
        }

        public static bool CheckText(MailDb maildb, Message message, string headerField, string compareValue)
        {
            // BCC BODY CC FROM SUBJECT TEXT TO
            var upper = headerField.ToUpper();
            if (upper == "TO")
            {
                if (!string.IsNullOrEmpty(compareValue))
                {
                    return message.To.Contains(compareValue);
                }
                return true;
            }
            else if ((upper == "CC"))
            {
                if (!string.IsNullOrEmpty(compareValue))
                {
                    return message.Cc.IndexOf(compareValue, StringComparison.OrdinalIgnoreCase) >= 0;
                }
                return true;
            }
            else if (upper == "FROM")
            {
                if (!string.IsNullOrEmpty(compareValue))
                {
                    return message.From.IndexOf(compareValue, StringComparison.OrdinalIgnoreCase) >= 0;
                }
                return true;
            }
            else if ((upper == "BCC"))
            {
                if (!string.IsNullOrEmpty(compareValue))
                {
                    return message.Bcc.IndexOf(compareValue, StringComparison.OrdinalIgnoreCase) >= 0;
                }
                return true;
            }
            else if ((upper == "SUBJECT"))
            {
                if (!string.IsNullOrEmpty(compareValue))
                {
                    return message.Subject.IndexOf(compareValue, StringComparison.OrdinalIgnoreCase) >= 0;
                }
                return true;
            }
            else
            {
                var content = maildb.Messages.GetContent(message.Id);

                if (upper == "TEXT")
                {
                    if (!string.IsNullOrEmpty(compareValue))
                    {
                        return content.IndexOf(compareValue, StringComparison.OrdinalIgnoreCase) >= 0;
                    }
                }
                else
                {
                    int index = content.IndexOf("\r\n\r\n");

                    if (upper == "BODY")
                    {
                        if (!string.IsNullOrEmpty(compareValue))
                        {
                            if (index >= 0)
                            {
                                string body = content.Substring(index);
                                return body.IndexOf(compareValue, StringComparison.OrdinalIgnoreCase) >= 0;
                            }
                        }
                        return true;
                    }
                    else
                    {
                        string headerpart = null;
                        headerpart = index > -1 ? content.Substring(0, index) : content;

                        var headerline = GetHeaderFieldLine(headerpart, headerField);

                        if (string.IsNullOrEmpty(headerline))
                        {
                            return false;
                        }

                        if (!string.IsNullOrEmpty(compareValue))
                        {
                            return headerline.IndexOf(compareValue, StringComparison.OrdinalIgnoreCase) >= 0;
                        }

                        return true;
                    }
                }
            }
            return false;
        }

        internal static string GetHeaderFieldLine(string header, string field)
        {
            if (header.StartsWith(field, StringComparison.OrdinalIgnoreCase))
            {
                int nextend = header.IndexOf("\r\n", field.Length);
                return nextend > -1 ? header.Substring(0, nextend) : header;
            }

            if (!field.StartsWith("\r\n"))
            {
                field = "\r\n" + field;
            }

            int start = header.IndexOf(field, StringComparison.OrdinalIgnoreCase);

            if (start > -1)
            {
                string line = null;
                int end = header.IndexOf("\r\n", start + field.Length);
                line = end > -1 ? header.Substring(start, end - start) : header.Substring(start);
                return line;
            }
            return null;
        }

        public static bool CheckDate(MailDb maildb, Message message, string keyword, DateTime value)
        {
            // BEFORE SENTBEFORE SENTON SENTSINCE SINCE ON
            if (keyword == "BEFORE" || keyword == "SENTBEFORE")
            {
                return message.CreationTime < value;
            }
            else if (keyword == "SINCE" || keyword == "SENTSINCE")
            {
                return message.CreationTime > value;
            }
            else if (keyword == "ON" || keyword == "SENTON")
            {
                return message.CreationTime.DayOfYear == value.DayOfYear && message.CreationTime.Year == value.Year;
            }
            return false;
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