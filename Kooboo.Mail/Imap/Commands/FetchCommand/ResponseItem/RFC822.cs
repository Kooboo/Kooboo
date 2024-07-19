//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using System.Text;
using static Kooboo.Mail.Imap.Commands.FetchCommand.CommandReader;

namespace Kooboo.Mail.Imap.Commands.FetchCommand.ResponseItem
{

    //RFC822 Equivalent to BODY[].
    public class RFC822 : ICommandResponse
    {
        public virtual string Name
        {
            get
            {
                return "RFC822";
            }
        }

        public List<ImapResponse> Render(MailDb mailDB, FetchMessage message, DataItem dataItem)
        {
            //var text = message.MailMessage.ToMessageText();

            //var bytes = System.Text.Encoding.UTF8.GetBytes(text); 

            var bytes = message.GetBytes();

            var builder = new StringBuilder()
                .Append(dataItem.FullItemName).Append(" {").Append(bytes).Append("}");

            return new List<ImapResponse>
            {
                new ImapResponse(builder.ToString()),
                new ImapResponse(bytes)
            };
        }
    }
}
