//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kooboo.Mail.Imap.Commands.FetchCommand.CommandReader;

namespace Kooboo.Mail.Imap.Commands.FetchCommand.ResponseItem
{
    public class RFC822_SIZE : ICommandResponse
    {
        public string Name
        {
            get
            {
                return "RFC822.SIZE"; 
            } 
        }

        public List<ImapResponse> Render(MailDb maildb, FetchMessage message, DataItem dataItem)
        {
            return new List<ImapResponse>
            {
                new ImapResponse(dataItem.FullItemName + " " + message.Bytes.Length)
            };
        }
    }
}
