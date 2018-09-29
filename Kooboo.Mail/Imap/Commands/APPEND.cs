//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

using LumiSoft.Net;
using LumiSoft.Net.Mail;
using LumiSoft.Net.IMAP;

using Kooboo.Mail.Utility;
using Kooboo.Mail.Imap.Commands.FetchCommand;

namespace Kooboo.Mail.Imap.Commands
{
    public class APPEND : ICommand
    {
        public string AdditionalResponse
        {
            get; set;
        }

        public string CommandName
        {
            get
            {
                return "APPEND";
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

        public async Task<List<ImapResponse>> Execute(ImapSession session, string args)
        {
            var appendArgs = ParseArgs(args);
            if (appendArgs == null)
                throw new CommandException("BAD", "Error in arguments");

            await session.Stream.WriteLineAsync("+ Continue");

            var bytes = await session.Stream.ReadAsync(appendArgs.Size);

            var content = SmtpUtility.GetString(bytes);

            await session.Stream.ReadLineAsync();

            var message = MessageUtility.ParseMeta(content);
            message.FolderId = session.SelectFolder.FolderId;
            message.UserId = session.MailDb.UserId;

            var folderName = session.SelectFolder.Folder.ToLower();
            if (folderName == "sent" || folderName == "drafts")
            {
                var user = Data.GlobalDb.Users.Get(session.MailDb.UserId);
                var orgDb = Kooboo.Mail.Factory.DBFactory.OrgDb(user.CurrentOrgId);
                var address = orgDb.EmailAddress.Get(Mail_t_Mailbox.Parse(message.From).Address);
                if (address != null)
                {
                    message.AddressId = address.Id;
                }
            }
            session.MailDb.Messages.Add(message, content);

            return null;
        }

        public static AppendArgs ParseArgs(string args)
        {
            var parts = args.Split(new char[] { ' ' }, 2);
            if (parts.Length != 2)
                throw new CommandException("BAD", "Error in arguments.");

            var result = new AppendArgs
            {
                FodlerName = IMAP_Utils.DecodeMailbox(TextUtils.UnQuoteString(parts[0]))
            };

            var restStr = parts[1];

            // Flags
            if (restStr.StartsWith("("))
            {
                var index = restStr.IndexOf(")");
                if (index <= 0)
                    return null;

                result.Flags = restStr.Substring(1, index - 1).Split(' ').Select(o => o.TrimStart('\\')).ToArray();
                restStr = parts[1].Substring(index + 1).TrimStart();
            }

            // Internal date
            if (restStr.StartsWith("\""))
            {
                var index = restStr.LastIndexOf("\"");
                if (index <= 0)
                    return null;

                var dateTimeStr = restStr.Substring(1, index - 1);
                DateTimeOffset time;
                if (!DateTimeOffset.TryParse(dateTimeStr, out time))
                    return null;

                result.InternalDate = time.UtcDateTime;

                restStr = restStr.Substring(index + 1).TrimStart();
            }

            if (!restStr.StartsWith("{") || !restStr.EndsWith("}"))
                return null;

            result.Size = Convert.ToInt32(restStr.Substring(1, restStr.Length - 2));

            return result;
        }

        public class AppendArgs
        {
            public string FodlerName { get; set; }

            public string[] Flags { get; set; }

            public DateTime? InternalDate { get; set; }

            public int Size { get; set; }
        }
    }
}



//6.3.11. APPEND Command

//   Arguments:  mailbox name
//               OPTIONAL flag parenthesized list
//               OPTIONAL date/time string
//               message literal

//   Responses:  no specific responses for this command

//   Result:     OK - append completed
//               NO - append error: can't append to that mailbox, error
//                    in flags or date/time or message text
//               BAD - command unknown or arguments invalid

//      The APPEND command appends the literal argument as a new message
//      to the end of the specified destination mailbox.This argument
//      SHOULD be in the format of an[RFC - 2822] message.  8-bit
//       characters are permitted in the message.A server implementation
 
//       that is unable to preserve 8-bit data properly MUST be able to
//       reversibly convert 8-bit APPEND data to 7-bit using a [MIME-IMB]
//content transfer encoding.

//     Note: There MAY be exceptions, e.g., draft messages, in
//           which required[RFC - 2822] header lines are omitted in
//           the message literal argument to APPEND.The full
//           implications of doing so MUST be understood and
//           carefully weighed.

//      If a flag parenthesized list is specified, the flags SHOULD be set
//      in the resulting message; otherwise, the flag list of the
//      resulting message is set to empty by default.  In either case, the
//      Recent flag is also set.

//      If a date-time is specified, the internal date SHOULD be set in
//      the resulting message; otherwise, the internal date of the
//      resulting message is set to the current date and time by default.

//      If the append is unsuccessful for any reason, the mailbox MUST be
//      restored to its state before the APPEND attempt; no partial
//      appending is permitted.

//      If the destination mailbox does not exist, a server MUST return an
//      error, and MUST NOT automatically create the mailbox.Unless it
//      is certain that the destination mailbox can not be created, the
//      server MUST send the response code "[TRYCREATE]" as the prefix of
//      the text of the tagged NO response.  This gives a hint to the
//      client that it can attempt a CREATE command and retry the APPEND
//      if the CREATE is successful.



//Crispin Standards Track[Page 46]

//RFC 3501                         IMAPv4 March 2003


//      If the mailbox is currently selected, the normal new message
//      actions SHOULD occur.Specifically, the server SHOULD notify the
//      client immediately via an untagged EXISTS response.  If the server
//      does not do so, the client MAY issue a NOOP command (or failing
//      that, a CHECK command) after one or more APPEND commands.

//   Example:    C: A003 APPEND saved-messages (\Seen)
//{ 310}
//S: + Ready for literal data
//               C: Date: Mon, 7 Feb 1994 21:52:25 -0800 (PST)
//               C: From: Fred Foobar<foobar@Blurdybloop.COM>
//               C: Subject: afternoon meeting
//               C: To: mooch @owatagu.siam.edu
//               C: Message-Id: <B27397-0100000@Blurdybloop.COM>
//               C: MIME-Version: 1.0
//               C: Content-Type: TEXT/PLAIN; CHARSET=US-ASCII
//               C:
//               C: Hello Joe, do you think we can meet at 3:30 tomorrow?
//               C:
//               S: A003 OK APPEND completed

//        Note: The APPEND command is not used for message delivery,
//        because it does not provide a mechanism to transfer[SMTP]
//        envelope information.
