//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kooboo.Mail.Imap.Commands
{
    public class STARTTLS : ICommand
    {
        public string AdditionalResponse
        {
            get; set;
        }

        public string CommandName
        {
            get
            {
                return "STARTTLS";
            }
        }

        public bool RequireAuth
        {
            get
            {
                return false;
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
            if (session.IsSecureConnection)
            {
                throw new CommandException("NO", "Already in secure connection");
            }

            //if (session.Server.SslMode != SslMode.StartTLS)
            //{
            throw new CommandException("NO", "StartTLS not supported");
            //} 

            //throw new StartTlsException();
        }
    }
}