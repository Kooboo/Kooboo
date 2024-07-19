//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using System.Text;
using Kooboo.Mail.Multipart;
using static Kooboo.Mail.Imap.Commands.FetchCommand.CommandReader;

namespace Kooboo.Mail.Imap.Commands.FetchCommand.ResponseItem
{
    public class RFC822_HEADER : ICommandResponse
    {
        public virtual string Name
        {
            get
            {
                return "RFC822.HEADER";
            }
        }

        public List<ImapResponse> Render(MailDb mailDb, FetchMessage message, DataItem dataItem)
        {
            //var text = message.MailMessage.ToMessageText(); 

            //var header = FetchHelper.GetHeader(text);
            //var bytes = System.Text.Encoding.UTF8.GetBytes(header); 

            var bytes = MultiPartHelper.ReadHeaderPart(message.GetMsgFileName());

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
