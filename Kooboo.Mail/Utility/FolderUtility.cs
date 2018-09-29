//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Mail.Utility
{
   public static class FolderUtility
    {
        public static  FolderAddressModel ParseFolder(string folder)
        {
            FolderAddressModel model = default(FolderAddressModel);

            if (folder.Contains("@"))
            {
                int index = folder.IndexOf("/");
                string namepart = folder.Substring(0, index);
                string addresspart = folder.Substring(index + 1);
                model.FolderId =  Folder.ToId(namepart);
                model.AddressId = EmailAddress.ToId(addresspart); 
            }
            else
            {
                model.FolderId = Kooboo.Mail.Folder.ToId(folder);
            }

            return model;
        }

        public struct FolderAddressModel
        { 
            public int FolderId { get; set; }

            public int AddressId { get; set; }
        }
          
    }  
}
