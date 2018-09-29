//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Kooboo.Mail.Imap.Commands.FetchCommand.CommandReader;
using LumiSoft.Net.MIME;

namespace Kooboo.Mail.Imap.Commands.FetchCommand.ResponseItem
{
    public class RFC822_TEXT : ICommandResponse
    {
        public virtual string Name
        {
            get
            {
                return "RFC822.TEXT"; 
            } 
        }

        public List<ImapResponse> Render(MailDb maildb, FetchMessage message, DataItem dataItem)
        {
            var parsed = LumiSoft.Net.Mail.Mail_Message.ParseFromByte(message.Bytes);
            var bytes = parsed.Body.ToByte();

            var builder = new StringBuilder()
                .Append(dataItem.FullItemName).Append(" {").Append(bytes.Length).Append("}");

            return new List<ImapResponse>
            {
                new ImapResponse(builder.ToString()),
                new ImapResponse(bytes)
            };
        }
    }
}
