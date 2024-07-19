//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.IO;

namespace Kooboo.IndexedDB
{
    public class StoreVersionUpgrade
    {
        public static void Upgrade(IObjectStore store)
        {
            //UpgradeBlocKFolder(store); 
        }

        private static void UpgradeBlocKFolder(IObjectStore store)
        {
            string oldname = OldBlockName(store.ObjectFolder, store.Name);
            string newname = NewBlockName(store.ObjectFolder);
            if (File.Exists(oldname) && oldname != newname)
            {
                File.Move(oldname, newname);
            }
        }

        private static string OldBlockName(string objectFolder, string storeName)
        {
            return System.IO.Path.Combine(objectFolder, storeName.ToValidFileName() + ".block");
        }

        private static string NewBlockName(string objectFolder)
        {
            return System.IO.Path.Combine(objectFolder, "Data.block");
        }
    }
}
