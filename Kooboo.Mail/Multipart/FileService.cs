//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using Kooboo.Data.Models;

namespace Kooboo.Mail.MultiPart
{
    public static class FileService
    {
        private static string UserTempAttachmentFolder(User user)
        {
            var temp = System.IO.Path.Combine(Data.AppSettings.TempDataPath, "MailAttachment");
            return System.IO.Path.Combine(temp, user.Id.ToString());
        }

        public static void Upload(User user, string filename, byte[] binary)
        {
            var folder = UserTempAttachmentFolder(user);
            Lib.Helper.IOHelper.EnsureDirectoryExists(folder);
            string fullfilename = System.IO.Path.Combine(folder, filename);
            System.IO.File.WriteAllBytes(fullfilename, binary);
        }

        public static byte[] Get(User user, string filename)
        {
            var folder = UserTempAttachmentFolder(user);
            Lib.Helper.IOHelper.EnsureDirectoryExists(folder);
            string fullfilename = System.IO.Path.Combine(folder, filename);
            if (System.IO.File.Exists(fullfilename))
            {
                return System.IO.File.ReadAllBytes(fullfilename);
            }
            return null;
        }

        public static int GetSize(User user, string filename)
        {
            var folder = UserTempAttachmentFolder(user);
            string fullfilename = System.IO.Path.Combine(folder, filename);
            if (System.IO.File.Exists(fullfilename))
            {
                var info = new System.IO.FileInfo(fullfilename);
                if (info != null)
                {
                    return (int)info.Length;
                }
            }
            return 0;
        }

        public static void DeleteFolder(User user, bool forcedelete = false)
        {
            var allfiles = System.IO.Directory.GetFiles(UserTempAttachmentFolder(user));

            foreach (var item in allfiles)
            {
                var info = new System.IO.FileInfo(item);
                if (info.LastWriteTime < DateTime.Now.AddHours(10) || forcedelete)
                {
                    System.IO.File.Delete(item);
                }
            }
        }

        public static void DeleteFile(User user, string filename)
        {
            var folder = UserTempAttachmentFolder(user);
            string fullfilename = System.IO.Path.Combine(folder, filename);
            if (System.IO.File.Exists(fullfilename))
            {
                System.IO.File.Delete(fullfilename);
            }
        }
    }
}
