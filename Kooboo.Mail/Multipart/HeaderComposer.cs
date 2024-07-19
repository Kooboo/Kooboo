//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Mail.Imap;

namespace Kooboo.Mail.Multipart
{
    public static class HeaderComposer
    {
        public static string Compose(Dictionary<string, string> Values)
        {
            string header = null;
            HashSet<string> valuesDone = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            string FromAddress = null;

            foreach (var item in Values)
            {
                string lowerCase = item.Key.ToLower();

                var value = item.Value;

                if (lowerCase == "date")
                {
                    // parse date. 
                }
                else if (lowerCase == "from")
                {
                    //value = Utility.HeaderUtility.EncodeField(value, true);
                    value = Utility.HeaderUtility.MailKitEncodeAddressField(value);
                    FromAddress = value;
                }

                else if (lowerCase == "to" || lowerCase == "cc" || lowerCase == "bcc")
                {

                    value = Utility.HeaderUtility.MailKitEncodeAddressField(value);
                }
                else if (lowerCase == "subject")
                {
                    value = Utility.HeaderUtility.EncodeField(value);
                }

                header += item.Key + ": " + value + "\r\n";
                valuesDone.Add(item.Key);
            }

            if (!valuesDone.Contains("Date"))
            {
                header += "Date: " + ImapHelper.DateTimeToRfc2822(DateTime.Now) + "\r\n";
            }

            if (!valuesDone.Contains("Message-ID"))
            {
                var msgID = Kooboo.Mail.Utility.SmtpUtility.GenerateMessageId(FromAddress);
                header += "Message-ID: " + msgID + "\r\n";
            }
            return header;
        }
    }
}
