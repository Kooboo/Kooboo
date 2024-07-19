//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kooboo.Mail.Imap.Commands
{
    public class UID : ICommand
    {
        public string AdditionalResponse
        {
            get; set;
        }

        public string CommandName
        {
            get
            {
                return "UID";
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
            var spl = args.Split(new char[] { ' ' }, 2);
            var subCommand = spl[0].ToUpperInvariant();
            List<ImapResponse> result;
            switch (subCommand)
            {
                case "FETCH":
                    return FetchCommand.FetchResponse.GenerateByUid(session.MailDb, session.SelectFolder, spl[1], session);
                case "STORE":
                    result = STORE.ExecuteByUid(session.MailDb, session.SelectFolder, spl[1]);
                    break;
                case "COPY":
                    result = COPY.ExecuteByUid(session.MailDb, session.SelectFolder, spl[1]);
                    break;
                case "MOVE":
                    result = MOVE.ExecuteByUid(session.MailDb, session.SelectFolder, spl[1]);
                    break;
                case "SEARCH":
                    result = SearchCommand.Search.Instance.ExecuteByUid(session.MailDb, session.SelectFolder, spl[1]);
                    break;
                default:
                    throw new Exception("Not support for UID " + subCommand);
            }

            return Task.FromResult(result);
        }
    }
}
