//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LumiSoft.Net.IMAP;
using LumiSoft.Net;


namespace Kooboo.Mail.Imap.Commands
{
    public class UNSUBSCRIBE : ICommand
    {
        public string AdditionalResponse
        {
            get;set;
        }

        public string CommandName
        {
            get
            {
                return "UNSUBSCRIBE";
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
            var folderName = IMAP_Utils.DecodeMailbox(TextUtils.UnQuoteString(args));

            if (Folder.ReservedFolder.Any(o => folderName.StartsWith(o.Value, StringComparison.OrdinalIgnoreCase)))
                throw new CommandException("NO", "Reserved folders are always subscribed");

            var folder = session.MailDb.Folders.Get(folderName);
            if (folder == null)
                throw new CommandException("NO", "No such a folder");

            session.MailDb.Folders.Unsubscribe(folder);

            return this.NullResult();
        }
    }
}