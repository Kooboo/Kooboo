//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using System.Text;
using static Kooboo.Mail.Imap.Commands.FetchCommand.CommandReader;

namespace Kooboo.Mail.Imap.Commands.FetchCommand.ResponseItem
{
    public class FLAGS : ICommandResponse
    {
        public string Name
        {
            get
            {
                return "FLAGS";
            }
        }

        public List<ImapResponse> Render(MailDb maildb, FetchMessage message, DataItem dataItem)
        {
            var flags = maildb.Message2.GetFlags(message.Message);

            var result = new StringBuilder()
                .Append(dataItem.FullItemName)
                .Append(" (");

            for (int i = 0; i < flags.Length; i++)
            {
                if (i > 0)
                {
                    result.Append(" ");
                }
                result.Append("\\").Append(flags[i]);
            }

            result.Append(")");

            return new List<ImapResponse>
            {
                new ImapResponse(result.ToString())
            };
        }
    }
}
