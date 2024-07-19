//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using System.Text;
using static Kooboo.Mail.Imap.Commands.FetchCommand.CommandReader;

namespace Kooboo.Mail.Imap.Commands.FetchCommand.ResponseItem
{
    public class RFC822_TEXT : ICommandResponse
    {
        /*
        RFC822.SIZE The number of octets in the message, as expressed
                     in [RFC-822]
        format.

      RFC822.TEXT The text body of the message, omitting the
                     [RFC - 822] header.The \Seen flag is implicitly
                     set; if this causes the flags to change they should
                     be included as part of the fetch responses.
        */
        public virtual string Name
        {
            get
            {
                return "RFC822.TEXT";
            }
        }

        public List<ImapResponse> Render(MailDb mailDb, FetchMessage message, DataItem dataItem)
        {
            //The \Seen flag is implicitly set;
            if (!message.Message.Read)
            {
                message.Message.Read = true;
                mailDb.Message2.UpdateMeta(message.Message);
            }

            // var text = message.MailMessage.ToMessageText();  
            var text = message.GetTextSource();

            var body = FetchHelper.GetBodyPart(text);

            if (body != null)
            {
                byte[] binary = System.Text.Encoding.UTF8.GetBytes(body);

                var builder = new StringBuilder()
                    .Append(dataItem.FullItemName).Append(" {").Append(binary.Length).Append("}");

                return new List<ImapResponse>
                {
                    new ImapResponse(builder.ToString()),
                    new ImapResponse(binary)
                };
            }

            return null;

        }
    }
}
