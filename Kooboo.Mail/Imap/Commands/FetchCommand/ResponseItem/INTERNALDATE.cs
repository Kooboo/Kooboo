//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved. 
using System;
using System.Collections.Generic;
using Kooboo.Mail.Utility;
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
            //INFO Imap(2)[6] Response: * 1 FETCH (UID 8 RFC822.SIZE 576 FLAGS (\Seen) INTERNALDATE ¡°21-Jun-2018 17:51:47 +0000¡± ENVELOPE (¡°Sat, 3 Dec 2016 21:39:27 -0700¡± ¡°Test Message¡± ((NIL NIL ¡°john.doe¡±  

            var timeStr = ToDate(message.Message.CreationTime.ToLocalTime());

            return new List<ImapResponse>
            {
                new ImapResponse(dataItem.FullItemName + " " +TextUtils.QuoteString(timeStr))
            };
        }

        public string ToDate(DateTime dateTime)
        {
            return dateTime.ToString("dd-MMM-yyyy HH':'mm':'ss ", System.Globalization.DateTimeFormatInfo.InvariantInfo) + dateTime.ToString("zzz").Replace(":", "");
        }


        public string DateTimeToRfc2822(DateTime dateTime)
        {
            return dateTime.ToString("ddd, dd MMM yyyy HH':'mm':'ss ", System.Globalization.DateTimeFormatInfo.InvariantInfo) + dateTime.ToString("zzz").Replace(":", "");
        }
    }
}
