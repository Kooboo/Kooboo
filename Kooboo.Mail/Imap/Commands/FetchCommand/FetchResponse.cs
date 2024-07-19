//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kooboo.Mail.Imap.Commands.FetchCommand.CommandReader;
using static Kooboo.Mail.Imap.ImapHelper;

namespace Kooboo.Mail.Imap.Commands.FetchCommand
{
    public static class FetchResponse
    {
        /// <summary>
        /// UID FETCH
        /// </summary>
        public static Task<List<ImapResponse>> GenerateByUid(MailDb mailDb, SelectFolder Folder, string Cmd, ImapSession session)
        {
            var cmdReader = new CommandReader(Cmd);
            var set = cmdReader.SequenceSet;
            var range = GetSequenceRange(set);

            var AllDataItems = cmdReader.ReadAllDataItems();

            CorrectDataItems(AllDataItems);

            var messages = ImapHelper.GetMessagesByUid(mailDb, Folder, range);

            // Auto include UID data item
            if (!AllDataItems.Any(o => o.Name == "UID"))
            {
                AllDataItems.Insert(0, new DataItem
                {
                    Name = "UID",
                    FullItemName = "UID"
                });
            }

            return Generate(mailDb, Folder, AllDataItems, messages, session);
        }


        public static void CorrectDataItems(List<DataItem> items)
        {
            if (items == null || !items.Any())
            {
                return;
            }

            // Rule, if item contains body or body.peek and other items. 

            if (items.Count() == 2)
            {
                var find = items.Find(o => o.Name.StartsWith("BODY")); /// || o.Name == "BODY.PEEK");    ;     ;

                if (find != null)
                {
                    find.SinglePartBodyHeader = true;
                }
            }
        }


        /// <summary>
        /// FETCH
        /// </summary>
        public static Task<List<ImapResponse>> GenerateBySeqId(MailDb mailDb, SelectFolder Folder, string Cmd, ImapSession session)
        {
            var cmdReader = new CommandReader(Cmd);
            var set = cmdReader.SequenceSet;
            var range = GetSequenceRange(set);

            var AllDataItems = cmdReader.ReadAllDataItems();

            var messages = ImapHelper.GetMessagesBySeqNo(mailDb, Folder, range);

            return Generate(mailDb, Folder, AllDataItems, messages, session);
        }

        public static async Task<List<ImapResponse>> Generate(MailDb maildb, SelectFolder Folder, List<DataItem> DataItems, List<FetchMessage> messages, ImapSession session)
        {
            var response = new List<ImapResponse>();

            foreach (var message in messages)
            {
                var builder = new StringBuilder()
                    .Append("* " + message.SeqNo + " FETCH (");

                bool firstDataItem = true;
                foreach (var dataItem in DataItems)
                {
                    var itemResponse = ResponseManager.GetResponse(dataItem, maildb, message);
                    foreach (var each in itemResponse)
                    {
                        if (each.Line != null)
                        {
                            // Continue to append string if it's string
                            if (firstDataItem)
                            {
                                firstDataItem = false;
                            }
                            else
                            {
                                builder.Append(" ");
                            }

                            builder.Append(each.Line);
                        }
                        else
                        {
                            // Otherwise end the string and add bytes
                            // response.Add(new ImapResponse(builder.ToString()));
                            // response.Add(each);
                            var responseString = builder.ToString();
                            await session.Stream.WriteLineAsync(responseString);
                            await session.Stream.WriteAsync(each.Binary);
                            builder.Clear();
                            ///firstDataItem = true; 
                        }
                    }
                }

                builder.Append(")");
                var responseNotAddBytes = builder.ToString();
                // response.Add(new ImapResponse(builder.ToString()));
                await session.Stream.WriteLineAsync(responseNotAddBytes);
            }

            return response;
        }

        public static void CorrectDataItem(List<DataItem> dataItems)
        {
            // make all name, section. upper case. 
            foreach (var item in dataItems)
            {
                item.Name = item.Name.ToUpper();
                if (item.Section != null && item.Section.PartSpecifier != null)
                {
                    item.Section.PartSpecifier = item.Section.PartSpecifier.ToUpper();
                }
            }

            // There are three macros which specify commonly - used sets of data
            // items, and can be used instead of data items.A macro must be
            //used by itself, and not in conjunction with other macros or data
            //items.
            // ALL
            //Macro equivalent to: (FLAGS INTERNALDATE RFC822.SIZE ENVELOPE) 
            // FAST
            //    Macro equivalent to: (FLAGS INTERNALDATE RFC822.SIZE) 
            // FULL
            //    Macro equivalent to: (FLAGS INTERNALDATE RFC822.SIZE ENVELOPE
            //    BODY)

            if (dataItems.Find(o => o.Name == "ALL") != null)
            {
                dataItems.Clear();
                dataItems.Add(new DataItem() { Name = "FLAGS", FullItemName = "FLAGS" });
                dataItems.Add(new DataItem() { Name = "INTERNALDATE", FullItemName = "INTERNALDATE" });
                dataItems.Add(new DataItem() { Name = "RFC822.SIZE", FullItemName = "RFC822.SIZE" });
                dataItems.Add(new DataItem() { Name = "ENVELOPE", FullItemName = "ENVELOPE" });
            }
            else if (dataItems.Find(o => o.Name == "FAST") != null)
            {
                dataItems.Clear();
                dataItems.Add(new DataItem() { Name = "FLAGS", FullItemName = "FLAGS" });
                dataItems.Add(new DataItem() { Name = "INTERNALDATE", FullItemName = "INTERNALDATE" });
                dataItems.Add(new DataItem() { Name = "RFC822.SIZE", FullItemName = "RFC822.SIZE" });
            }
            else if (dataItems.Find(o => o.Name == "FULL") != null)
            {
                dataItems.Clear();
                dataItems.Add(new DataItem() { Name = "FLAGS", FullItemName = "FLAGS" });
                dataItems.Add(new DataItem() { Name = "INTERNALDATE", FullItemName = "INTERNALDATE" });
                dataItems.Add(new DataItem() { Name = "RFC822.SIZE", FullItemName = "RFC822.SIZE" });
                dataItems.Add(new DataItem() { Name = "ENVELOPE", FullItemName = "ENVELOPE" });
                dataItems.Add(new DataItem() { Name = "BODY", FullItemName = "BODY" });
            }
            //TODO: consider move body to the end...???
        }

    }
}



/* RFC 3501
              seq-number     = nz-number / "*"
                              ; message sequence number (COPY, FETCH, STORE
                              ; commands) or unique identifier (UID COPY,
                              ; UID FETCH, UID STORE commands).
                              ; * represents the largest number in use.  In
                              ; the case of message sequence numbers, it is
                              ; the number of messages in a non-empty mailbox.
                              ; In the case of unique identifiers, it is the
                              ; unique identifier of the last message in the
                              ; mailbox or, if the mailbox is empty, the
                              ; mailbox's current UIDNEXT value.
                              ; The server should respond with a tagged BAD
                              ; response to a command that uses a message
                              ; sequence number greater than the number of
                              ; messages in the selected mailbox.  This
                              ; includes "*" if the selected mailbox is empty.

              seq-range      = seq-number ":" seq-number
                              ; two seq-number values and all values between
                              ; these two regardless of order.
                              ; Example: 2:4 and 4:2 are equivalent and indicate
                              ; values 2, 3, and 4.
                              ; Example: a unique identifier sequence range of
                              ; 3291:* includes the UID of the last message in
                              ; the mailbox, even if that value is less than 3291.

              sequence-set    = (seq-number / seq-range) *("," sequence-set)
                              ; set of seq-number values, regardless of order.
                              ; Servers MAY coalesce overlaps and/or execute the
                              ; sequence in any order.
                              ; Example: a message sequence number set of
                              ; 2,4:7,9,12:* for a mailbox with 15 messages is
                              ; equivalent to 2,4,5,6,7,9,12,13,14,15
                              ; Example: a message sequence number set of *:4,5:7
                              ; for a mailbox with 10 messages is equivalent to
                              ; 10,9,8,7,6,5,4,5,6,7 and MAY be reordered and
                              ; overlap coalesced to be 4,5,6,7,8,9,10.
          */
