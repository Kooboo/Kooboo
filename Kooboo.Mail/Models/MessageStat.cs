//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Mail.Models
{ 
    public class MessageStat
    {
        public int Exists { get; set; }

        public int Recent { get; set; }

        public int UnSeen { get; set; }

        public int FirstUnSeen { get; set; } = -1; 

        public int NextUid { get; set;  }

        public int FolderUid { get; set; }

        public int UIDVALIDITY { get; set; } = 1234; // in our case, we will never change this value.. this is only change when Message.Id are changed.   

        public int LastestMsgId { get; set; }
    }

}
