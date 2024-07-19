//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Mail.Imap.Commands.FetchCommand;
using Kooboo.Mail.Utility;

namespace Kooboo.Mail.Imap.Commands
{
    public class STATUS : ICommand
    {
        public string AdditionalResponse
        {
            get; set;
        }

        public string CommandName
        {
            get
            {
                return "STATUS";
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
            return Task.FromResult(Execute(session.MailDb, args));
        }

        public static List<ImapResponse> Execute(MailDb mailDb, string args)
        {
            var parts = TextUtils.SplitQuotedString(args, ' ', false, 2);

            string folderName = IMAP_Utils.DecodeMailbox(TextUtils.UnQuoteString(parts[0]));

            if (!(parts[1].StartsWith("(") && parts[1].EndsWith(")")))
                throw new Exception("Error in arguments.");

            var index = args.IndexOf(" ");
            var folderArg = args.Substring(0, index);

            var folder = new SelectFolder(folderName, mailDb);
            var stat = folder.Stat;

            var commandReader = new CommandReader(parts[1]);
            var AllDataItems = commandReader.ReadAllDataItems();

            var builder = new StringBuilder()
                .Append("* STATUS ").Append(folderArg).Append(" (");

            var first = true;
            foreach (var each in AllDataItems)
            {
                if (!first)
                {
                    builder.Append(" ");
                }
                switch (each.Name)
                {
                    case "MESSAGES":
                        builder.Append("MESSAGES ").Append(stat.Exists);
                        break;
                    case "RECENT":
                        builder.Append("RECENT ").Append(stat.Recent);
                        break;
                    case "UIDNEXT":
                        builder.Append("UIDNEXT ").Append(stat.NextUid);
                        break;
                    case "UIDVALIDITY":
                        builder.Append("UIDVALIDITY ").Append(stat.FolderUid);
                        break;
                    case "UNSEEN":
                        builder.Append("UNSEEN ").Append(stat.UnSeen);
                        break;
                    default:
                        break;
                }
                if (first)
                {
                    first = false;
                }
            }

            builder.Append(")");

            return new List<ImapResponse>
            {
                new ImapResponse(builder.ToString())
            };
        }
    }
}
