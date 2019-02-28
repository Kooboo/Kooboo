//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LumiSoft.Net;
using LumiSoft.Net.IMAP;

using Kooboo.Mail.Imap.Commands.FetchCommand;

namespace Kooboo.Mail.Imap.Commands
{
    public class COPY : ICommand
    {
        public string AdditionalResponse
        {
            get; set;
        }

        public string CommandName
        {
            get
            {
                return "COPY";
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
                return true;
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

        public static List<ImapResponse> ExecuteBySeqNo(MailDb mailDb, SelectFolder folder, string args)
        {
            var copyArgs = ParseArgs(args);

            var messages = ImapHelper.GetMessagesBySeqNo(mailDb, folder, copyArgs.Ranges);

            return Execute(mailDb, messages, copyArgs.FolderName);
        }

        public static List<ImapResponse> ExecuteByUid(MailDb mailDb, SelectFolder folder, string args)
        {
            var copyArgs = ParseArgs(args);

            var messages = ImapHelper.GetMessagesByUid(mailDb, folder, copyArgs.Ranges);

            return Execute(mailDb, messages, copyArgs.FolderName);
        }

        public static List<ImapResponse> Execute(MailDb mailDb, IEnumerable<FetchMessage> messages, string folderName)
        { 
            var prasedFolder = Utility.FolderUtility.ParseFolder(folderName); 
              
            var folder = mailDb.Folders.Get(prasedFolder.FolderId);
            if (folder == null)
                throw new CommandException("NO", "Can't copy those messages or to that name");

            foreach (var each in messages)
            {
                var message = each.Message;
                var flags = mailDb.Messages.GetFlags(message.Id);

                message.Id = 0;
                message.FolderId = folder.Id;
               
                if (message.AddressId == 0)
                {
                    message.AddressId = prasedFolder.AddressId; 
                }
               
                mailDb.Messages.Add(message);
                flags = flags.Except(new string[] { "Deleted" }).ToArray();
                mailDb.Messages.UpdateRecent(message.Id);
                mailDb.Messages.ReplaceFlags(message.Id, flags);
            }

            return new List<ImapResponse>();
        }

        public static CopyArgs ParseArgs(string args)
        {
            var parts = args.Split(new char[] { ' ' }, 2);
            if (parts.Length != 2)
                throw new Exception("Error in arguments.");

            return new CopyArgs
            {
                Ranges = ImapHelper.GetSequenceRange(parts[0]),
                FolderName = IMAP_Utils.DecodeMailbox(TextUtils.UnQuoteString(parts[1]))
            };
        }

        public class CopyArgs
        {
            public List<ImapHelper.Range> Ranges { get; set; }

            public string FolderName { get; set; }
        }
    }
}




       /* RFC 3501 6.1.2. NOOP Command.
                Arguments:  none

                Responses:  no specific responses for this command (but see below)

                Result:     OK - noop completed
                            BAD - command unknown or arguments invalid

                The NOOP command always succeeds.  It does nothing.

                Since any command can return a status update as untagged data, the
                NOOP command can be used as a periodic poll for new messages or
                message status updates during a period of inactivity (this is the
                preferred method to do this).  The NOOP command can also be used
                to reset any inactivity autologout timer on the server.

                Example:    C: a002 NOOP
                            S: a002 OK NOOP completed
                            . . .
                            C: a047 NOOP
                            S: * 22 EXPUNGE
                            S: * 23 EXISTS
                            S: * 3 RECENT
                            S: * 14 FETCH (FLAGS (\Seen \Deleted))
                            S: a047 OK NOOP completed
            */

            //if (m_pSelectedFolder != null)
            //{
            //    UpdateSelectedFolderAndSendChanges();
            //}

            //// m_pResponseSender.SendResponseAsync(new IMAP_r_(cmdTag, "OK", "NOOP Completed"));

            //m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "OK", "NOOP Completed"));