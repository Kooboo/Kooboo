//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using Kooboo.Mail.Models;

namespace Kooboo.Mail.Utility
{
    public static class FolderUtility
    {
        public static FolderAddressModel ParseFolder(string folder)
        {
            FolderAddressModel model = default(FolderAddressModel);

            if (folder.Contains("@"))
            {
                int index = folder.IndexOf("/");
                if (index < 0)
                {
                    model.FolderId = Folder.ToId(folder);
                    model.AddressId = EmailAddress.ToId(folder);
                    return model;
                }
                string namepart = folder.Substring(0, index);
                string addresspart = folder.Substring(index + 1);
                model.FolderId = Folder.ToId(namepart);
                model.AddressId = EmailAddress.ToId(addresspart);
            }
            else
            {
                model.FolderId = Kooboo.Mail.Folder.ToId(folder);
            }

            return model;
        }

        public static List<UnreadCounter> AddressUnread(MailDb db, bool IsInbox = true)
        {
            int folderid = IsInbox ? Folder.ToId(Folder.Inbox) : Folder.ToId(Folder.Sent);

            var MsgUnread = db.Message2.AddressUnread(folderid);
            return MsgUnread;
        }



        public struct FolderAddressModel
        {
            public int FolderId { get; set; }
            public int AddressId { get; set; }
        }

    }
}
