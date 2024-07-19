//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Kooboo.Mail.Imap.Commands.FetchCommand;

namespace Kooboo.Mail.Imap.Commands
{
    public class STORE : ICommand
    {
        public string AdditionalResponse
        {
            get; set;
        }

        public string CommandName
        {
            get
            {
                return "STORE";
            }
        }

        public bool RequireAuth
        {
            get
            {
                return true;
            }
        }

        public bool RequireFolder
        {
            get
            {
                return false;
            }
        }

        public bool RequireTwoPartCommand
        {
            get
            {
                return false;
            }
        }

        public Task<List<ImapResponse>> Execute(ImapSession session, string args)
        {
            return Task.FromResult(ExecuteBySeqNo(session.MailDb, session.SelectFolder, args));
        }

        public static StoreArgs ParseStoreArgs(string args)
        {
            string[] parts = args.Split(new char[] { ' ' }, 3);
            if (parts.Length != 3)
                throw new Exception("Error in arguments.");

            var result = new StoreArgs
            {
                Ranges = ImapHelper.GetSequenceRange(parts[0])
            };

            switch (parts[1].ToUpperInvariant())
            {
                case "FLAGS":
                    result.DataItem = "FLAGS";
                    result.Silent = false;
                    break;
                case "FLAGS.SILENT":
                    result.DataItem = "FLAGS";
                    result.Silent = true;
                    break;
                case "+FLAGS":
                    result.DataItem = "+FLAGS";
                    result.Silent = false;
                    break;
                case "+FLAGS.SILENT":
                    result.DataItem = "+FLAGS";
                    result.Silent = true;
                    break;
                case "-FLAGS":
                    result.DataItem = "-FLAGS";
                    result.Silent = false;
                    break;
                case "-FLAGS.SILENT":
                    result.DataItem = "-FLAGS";
                    result.Silent = true;
                    break;
                default:
                    throw new Exception("Error in arguments.");
            }

            if (!(parts[2].StartsWith("(") && parts[2].EndsWith(")")))
                throw new Exception("Error in arguments.");

            var spl = parts[2].Substring(1, parts[2].Length - 2).Split(' ');
            result.Flags = new HashSet<string>(spl.Select(o => o.TrimStart('\\'))).ToArray();

            return result;
        }

        public static List<ImapResponse> ExecuteBySeqNo(MailDb mailDb, SelectFolder folder, string args)
        {
            var storeArgs = ParseStoreArgs(args);

            var messages = ImapHelper.GetMessagesBySeqNo(mailDb, folder, storeArgs.Ranges);

            return ExecuteNew(mailDb, messages, storeArgs.DataItem, storeArgs.Flags, storeArgs.Silent);
        }

        public static List<ImapResponse> ExecuteByUid(MailDb mailDb, SelectFolder folder, string args)
        {
            var storeArgs = ParseStoreArgs(args);

            var messages = ImapHelper.GetMessagesByUid(mailDb, folder, storeArgs.Ranges);

            return ExecuteNew(mailDb, messages, storeArgs.DataItem, storeArgs.Flags, storeArgs.Silent);
        }

        public static List<ImapResponse> Execute(List<FetchMessage> messages, string dataItem, string[] flags, bool silent)
        {
            Action<Message> action = null;
            switch (dataItem)
            {
                case "FLAGS":
                    action = o => o.ReplaceFlags(flags);
                    break;
                case "+FLAGS":
                    action = o => o.AddFlags(flags);
                    break;
                case "-FLAGS":
                    action = o => o.RemoveFlags(flags);
                    break;
                default:
                    throw new Exception("Error in argument");
            }

            var response = new List<ImapResponse>();
            foreach (var each in messages)
            {
                action(each.Message);

                if (!silent)
                {
                    var flagStr = String.Join(" ", each.Message.GetFlags().Select(o => "\\" + o));
                    response.Add(new ImapResponse($"* {each.SeqNo} FETCH ({flagStr})"));
                }
            }
            return response;
        }

        public static List<ImapResponse> ExecuteNew(MailDb Maildb, List<FetchMessage> messages, string dataItem, string[] flags, bool silent)
        {
            var response = new List<ImapResponse>();
            foreach (var each in messages)
            {
                switch (dataItem)
                {
                    case "FLAGS":
                        Maildb.Message2.ReplaceFlags(each.Message.MsgId, flags);
                        break;
                    case "+FLAGS":
                        Maildb.Message2.AddFlags(each.Message.MsgId, flags);
                        break;
                    case "-FLAGS":
                        Maildb.Message2.RemoveFlags(each.Message.MsgId, flags);
                        break;
                    default:
                        throw new Exception("Error in argument");
                }

                if (!silent)
                {
                    //var eachflags = Maildb.Msgstore.GetFlags(each.Message.MsgId); 
                    var eachflags = Maildb.Message2.GetFlags(each.Message.MsgId);
                    var flagStr = String.Join(" ", eachflags.Select(o => "\\" + o));
                    response.Add(new ImapResponse($"* {each.SeqNo} FETCH (FLAGS ({flagStr}))"));
                }
            }
            return response;
        }

        public class StoreArgs
        {
            public List<ImapHelper.Range> Ranges { get; set; }

            public string DataItem { get; set; }

            public bool Silent { get; set; }

            public string[] Flags { get; set; }
        }
    }
}


//6.4.6.  STORE Command

//   Arguments:  sequence set
//               message data item name
//               value for message data item

//   Responses:  untagged responses: FETCH

//   Result:     OK - store completed
//               NO - store error: can't store that data
//               BAD - command unknown or arguments invalid

//      The STORE command alters data associated with a message in the
//      mailbox.Normally, STORE will return the updated value of the
//      data with an untagged FETCH response.A suffix of ".SILENT" in
//      the data item name prevents the untagged FETCH, and the server
//      SHOULD assume that the client has determined the updated value
//      itself or does not care about the updated value.

//           Note: Regardless of whether or not the ".SILENT" suffix
//           was used, the server SHOULD send an untagged FETCH
//           response if a change to a message's flags from an
//           external source is observed.The intent is that the
//           status of the flags is determinate without a race
//           condition.











//Crispin Standards Track[Page 58]

//RFC 3501                         IMAPv4 March 2003


//      The currently defined data items that can be stored are:

//      FLAGS<flag list>
//         Replace the flags for the message (other than \Recent) with the
//         argument.The new value of the flags is returned as if a FETCH
//         of those flags was done.

//      FLAGS.SILENT<flag list>
//         Equivalent to FLAGS, but without returning a new value.

//      +FLAGS<flag list>
//         Add the argument to the flags for the message.The new value
//       of the flags is returned as if a FETCH of those flags was done.

//      +FLAGS.SILENT<flag list>
//         Equivalent to +FLAGS, but without returning a new value.

//      -FLAGS<flag list>
//         Remove the argument from the flags for the message.The new
//       value of the flags is returned as if a FETCH of those flags was
//         done.

//      -FLAGS.SILENT<flag list>
//         Equivalent to -FLAGS, but without returning a new value.


//   Example:    C: A003 STORE 2:4 +FLAGS(\Deleted)
//               S: * 2 FETCH(FLAGS (\Deleted \Seen))
//               S: * 3 FETCH(FLAGS (\Deleted))
//               S: * 4 FETCH(FLAGS (\Deleted \Flagged \Seen))
//               S: A003 OK STORE completed