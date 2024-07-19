//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Mail.Spam
{
    public static class SpamFilter
    {
        public static Folder DetermineFolder(string messagebody, bool IsContact)
        {
            if (IsContact)
            {
                return new Folder(Folder.Inbox);
            }

            if (Kooboo.Mail.SecurityControl.SpamTest.instance.IsSpam(messagebody))
            {
                Kooboo.Data.Log.Instance.EmailDebug.Write("Move to Spam Folder: \r\n\r\n" + messagebody);

                return new Folder(Folder.Spam);
            }
            else
            {
                return new Folder(Folder.Inbox);
            }
        }

    }
}
