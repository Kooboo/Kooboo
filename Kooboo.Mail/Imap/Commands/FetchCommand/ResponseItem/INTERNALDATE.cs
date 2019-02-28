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
    public class INTERNALDATE : ICommandResponse
    {
        public string Name
        {
            get
            {
                return "INTERNALDATE"; 
            } 
        }

        public List<ImapResponse> Render(MailDb maildb, FetchMessage message, DataItem dataItem)
        {
            var timeStr = ToDateTimeString(message.Message.CreationTime);

            return new List<ImapResponse>
            {
                new ImapResponse(dataItem.FullItemName + " " + timeStr)
            };
        }

        public static string ToDateTimeString(DateTime utcTime)
        {
            return "\"" + utcTime.ToString("dd-MMM-yyyy HH:mm:ss", new System.Globalization.CultureInfo("en-US"))
                + (" +0000") + "\"";
        }
    }
}
