using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Mail.MassMailing
{

    //K own command
    public class XCommand
    {
        public bool Priority { get; set; }

        public string ClientId { get; set; }

        public string SendTaskId { get; set; }

        public string SentRecipientId { get; set; }

        public string RecipientId { get; set; }

        public string WebSiteId { get; set; }

        public string LogUrl { get; set; }   // relative or absolute URL, if relative, post to the client IP. Log can send multiple lines using GZIP stream, not zip needed.

        public string DedicatedIP { get; set; }  // required to send by one special IP. 

        public Dictionary<string, string> Values { get; set; }   // additional values.


        public string ToHeaderline()
        {
            string result = "x-command:";
            if (Priority)
            {
                result += "p=1,";
            }
            if (ClientId != null)
            {
                result += "c=" + ClientId + ",";
            }

            if (SendTaskId != null)
            {
                result += "s=" + this.SendTaskId + ",";
            }
            if (SentRecipientId != null)
            {
                result += "r=" + this.SentRecipientId + ",";
            }

            if (WebSiteId != null)
            {
                result += "w=" + this.WebSiteId + ",";
            }

            if (LogUrl != null)
            {
                result += "l=" + LogUrl + ",";
            }

            if (DedicatedIP != null)
            {
                result += "d=" + DedicatedIP + ",";
            }

            if (Values != null && Values.Any())
            {
                string json = System.Text.Json.JsonSerializer.Serialize(Values);

                result += "j=" + json;
            }
            return result;
        }

        private const int commandLen = 10;  // x-command:

        public static XCommand ParseHeaderLine(string UnFoldedLine)
        {
            if (UnFoldedLine.StartsWith("x-command:"))
            {
                UnFoldedLine = UnFoldedLine.Substring(commandLen);
            }

            var parts = UnFoldedLine.Split(',', StringSplitOptions.RemoveEmptyEntries);

            if (parts == null || !parts.Any())
            {
                return null;
            }

            XCommand command = new XCommand();

            foreach (var item in parts)
            {
                if (item[1] == '=')
                {
                    var identifier = item[0];
                    if (identifier == 'p')
                    {
                        command.Priority = true;
                    }
                    else if (identifier == 'd')
                    {
                        command.DedicatedIP = item.Substring(2);
                    }
                    else if (identifier == 'w')
                    {
                        command.WebSiteId = item.Substring(2);
                    }
                    else if (identifier == 'r')
                    {
                        command.SentRecipientId = item.Substring(2);
                    }
                    else if (identifier == 's')
                    {
                        command.SendTaskId = item.Substring(2);
                    }
                    else if (identifier == 'c')
                    {
                        command.ClientId = item.Substring(2);
                    }
                    else if (identifier == 'l')
                    {
                        command.LogUrl = item.Substring(2);
                    }
                    else if (identifier == 'j')
                    {
                        var json = item.Substring(2);

                        command.Values = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                    }
                }
            }
            return command;
        }

    }
}


/*
 * Google feedback loop, can be similar as X-command tracking. 
 * 
 * Feedback-ID: CampaignIDX:CustomerID2:MailTypeID3:SenderId

CampaignIDX: Campaign Identifier specific to Customer2 and is unique across the board (that is, no 2 customers share the same campaign ID).
CustomerID2: Unique customer Identifier.
MailTypeID3: Identifier for the type of mail (for example, a newsletter versus a product update) and can be either unique or common across customers, based on how the sender wants to view the data.
SenderId: Sender's unique Identifier and can be used for overall statistics.
 * 
 * 
 * 
 * 
 * 
 */