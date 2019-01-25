//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using LumiSoft.Net.IMAP;
using LumiSoft.Net;

namespace Kooboo.Mail.Imap.Commands
{
    public class SELECT : ICommand
    {
        public string AdditionalResponse
        {
            get
            {
                return "[READ-WRITE]";
            } 
            set { } 
        }

        public string CommandName
        {
            get
            {
                return "SELECT";
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

        // RFC 3501 6.3.1. SELECT Command.

        public Task<List<ImapResponse>> Execute(ImapSession session, string args)
        {
            string[] parts = TextUtils.SplitQuotedString(args, ' ');
            if (parts.Length >= 2)
            {
                // At moment we don't support UTF-8 mailboxes.
                if (Lib.Helper.StringHelper.IsSameValue(parts[1], "(UTF8)"))
                {
                    throw new CommandException("NO", "UTF8 name not supported");
                }
                else
                {
                    throw new CommandException("BAD", "Argument error");
                }
            }

            List<ImapResponse> Result = new List<ImapResponse>();

            string foldername = TextUtils.UnQuoteString(IMAP_Utils.DecodeMailbox(args));

            var parsedfolder = Kooboo.Mail.Utility.FolderUtility.ParseFolder(foldername);

            session.SelectFolder = new SelectFolder(foldername, session.MailDb);

            var stat = session.SelectFolder.Stat;

            session.MailDb.Messages.UpdateRecentByMaxId(stat.LastestMsgId); 

            Result.Add(new ImapResponse(ResultLine.EXISTS(stat.Exists)));
            Result.Add(new ImapResponse(ResultLine.RECENT(stat.Recent)));  

            if (stat.FirstUnSeen > -1)
            {
                Result.Add(new ImapResponse(ResultLine.UNSEEN(stat.FirstUnSeen))); 
            }

            Result.Add(new ImapResponse(ResultLine.UIDNEXT(stat.NextUid)));

            Result.Add(new ImapResponse(ResultLine.UIDVALIDAITY(stat.FolderUid)));
             
            Result.Add(new ImapResponse(ResultLine.FLAGS(Setting.SupportFlags.ToList())));

            return Task.FromResult(Result);
        }

    }
}
