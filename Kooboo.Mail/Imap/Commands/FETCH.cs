//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Mail.Imap.Commands
{
    public class FETCH : ICommand
    {
        public string AdditionalResponse
        {
            get; set;
        }

        public string CommandName
        {
            get
            {
                return "FETCH"; 
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
            var result = FetchCommand.FetchResponse.GenerateBySeqId(session.MailDb, session.SelectFolder, args);
            return Task.FromResult(result);
        }
    }
}
