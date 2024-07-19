//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using static Kooboo.Mail.Imap.Commands.FetchCommand.CommandReader;

namespace Kooboo.Mail.Imap.Commands.FetchCommand.ResponseItem
{
    public class UID : ICommandResponse
    {
        public string Name
        {
            get
            {
                return "UID";
            }
        }

        public List<ImapResponse> Render(MailDb maildb, FetchMessage message, DataItem dataItem)
        {
            return new List<ImapResponse>
            {
                new ImapResponse(dataItem.FullItemName + " " + message.Message.MsgId)
            };
        }
    }
}
