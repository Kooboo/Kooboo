//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Mail.Smtp
{
    public class SmtpPoolItem
    {
        public SmtpPoolItem(SmtpClient client, int allowedMails)
        {
            if (client == null)
                throw new ArgumentNullException();

            Client = client;
            SentMails = 0;
            AllowedMails = allowedMails;
        }

        public string IP { get; set; }

        public string Host { get; set; }

        public SmtpClient Client { get; set; }

        public int SentMails { get; set; }

        public int AllowedMails { get; set; }
    }
}
