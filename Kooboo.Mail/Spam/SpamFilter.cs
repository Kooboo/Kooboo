//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.

namespace Kooboo.Mail.Spam
{
    public static class SpamFilter
    {
        public static Folder DetermineFolder()
        {
            return new Folder("Inbox");
        }
    }
}