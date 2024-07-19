//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
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
            //var text = message.MailMessage.ToMessageText();

            //var bytes = System.Text.Encoding.UTF8.GetBytes(text); 

            var len = message.GetByteLength();

            return new List<ImapResponse>
            {
               // new ImapResponse(dataItem.FullItemName + " " + bytes.Length)
                new ImapResponse(dataItem.FullItemName + " " + len)
            };
        }
    }
}
