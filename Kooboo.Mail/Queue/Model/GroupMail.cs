//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.Mail.Queue.Model
{
    public class GroupMail
    {
        public string MessageContent { get; set; }

        public List<string> Members { get; set; }

        public string MailFrom { get; set; }
    }
}
