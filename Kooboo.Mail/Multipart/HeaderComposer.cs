//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Mail.Multipart
{
  public static class HeaderComposer
    {  
        public static string Compose(Dictionary<string, string> Values)
        {
            string header = null;

            HashSet<string> donevalues = new HashSet<string>(StringComparer.OrdinalIgnoreCase); 

            foreach (var item in Values)
            {
                string lower = item.Key.ToLower();

                var value = item.Value; 

                if(lower == "date")
                {
                    // parse date. 
                }

                else if (lower == "to" || lower == "from" )
                {
                    value = Utility.HeaderUtility.EncodeField(value, true); 
                }
                else if (lower == "subject")
                {
                    value = Utility.HeaderUtility.EncodeField(value); 
                }
                
                header += item.Key + ": " + value + "\r\n";
                donevalues.Add(item.Key); 
            }

           if (!donevalues.Contains("Date"))
            {
                header += "Date: " + LumiSoft.Net.MIME.MIME_Utils.DateTimeToRfc2822(DateTime.Now)+ "\r\n"; 
            } 

           if (!donevalues.Contains("Message-ID"))
            {
                header += "Message-ID: <" + Guid.NewGuid().ToString().Replace("-", "") + "@mail.kooboo.com>\r\n"; 
            }
            return header; 
        }
        
    }
}
