//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.

namespace Kooboo.Mail.Models
{
    public class MessageStat
    {
        public int Exists { get; set; }

        public int Recent { get; set; }

        public int Seen { get; set; }

        public int UnSeen
        {
            get
            {
                return Exists - Seen;
            }
        }

        public int FirstUnSeen { get; set; } = -1;

        public int NextUid { get; set; }

        public int FolderUid { get; set; }

        public int UIDVALIDITY { get; set; } = 2345; // in our case, we will never change this value.. this is only change when Message.Id are changed.   

        public int LastestMsgId { get; set; }
    }

}
