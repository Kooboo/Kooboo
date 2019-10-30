//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Sites.Scripting.Global
{
    public class FileIO
    {
        private string _rootfolder;

        private string RootFolder => _rootfolder ?? (_rootfolder = Kooboo.Data.AppSettings.GetFileIORoot(this.Context.WebSite));

        private RenderContext Context { get; set; }

        public FileIO(RenderContext context)
        {
            this.Context = context;
        }

        public FileInfo Write(string fileName, string content)
        {
            var name = _getfullname(fileName);

            if (!string.IsNullOrEmpty(name))
            {
                System.IO.File.WriteAllText(name, content);
                return GetFileInfo(name);
            }
            return null;
        }

        public FileInfo WriteBinary(string fileName, byte[] binary)
        {
            if (binary != null && binary.Length > 0)
            {
                var name = _getfullname(fileName);

                if (!string.IsNullOrEmpty(name))
                {
                    System.IO.File.WriteAllBytes(name, binary);
                    return GetFileInfo(name);
                }
            }
            return null;
        }

        public void Append(string fileName, string content)
        {
            var name = _getfullname(fileName);
            if (!string.IsNullOrWhiteSpace(name))
            {
                System.IO.File.AppendAllText(name, content);
            }
        }

        private string _getfullname(string fileName)
        {
            var valid = ToValidPath(fileName);

            string filename = Lib.Helper.IOHelper.CombinePath(this.RootFolder, valid);

            if (!filename.StartsWith(this.RootFolder))
            {
                return null; // this is not allowed, as it may try to write to other folders.
            }

            Lib.Helper.IOHelper.EnsureFileDirectoryExists(filename);

            return filename;
        }

        public string ToValidPath(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            StringBuilder sb = new StringBuilder();

            char[] invalid = System.IO.Path.GetInvalidPathChars();

            for (int i = 0; i < input.Length; i++)
            {
                if (!invalid.Contains(input[i]) || (input[i] == '\\' || input[i] == '/'))
                {
                    sb.Append(input[i]);
                }
            }
            string result = sb.ToString();

            if (!string.IsNullOrEmpty(result))
            {
                result = result.Replace("  ", " ");
                return result.Replace(" ", "_");
            }
            return result;
        }

        public string Url(string filename)
        {
            if (filename.StartsWith(this.RootFolder))
            {
                filename = filename.Substring(this.RootFolder.Length + 1);
            }
            string url = "/__kb/kfile/";
            return Lib.Helper.UrlHelper.Combine(url, filename);
        }

        public string Read(string fileName)
        {
            var name = _getfullname(fileName);
            if (!string.IsNullOrWhiteSpace(name) && System.IO.File.Exists(name))
            {
                return System.IO.File.ReadAllText(name);
            }
            return null;
        }

        public byte[] ReadBinary(string fileName)
        {
            var name = _getfullname(fileName);
            if (!string.IsNullOrWhiteSpace(name) && System.IO.File.Exists(name))
            {
                return System.IO.File.ReadAllBytes(name);
            }
            return null;
        }

        public bool Exists(string fileName)
        {
            var name = _getfullname(fileName);

            return System.IO.File.Exists(name);
        }

        public void Delete(string fileName)
        {
            var name = _getfullname(fileName);
            if (System.IO.File.Exists(name))
            {
                System.IO.File.Delete(name);
            }
        }

        public FileInfo[] GetAllFiles()
        {
            var fileList = new List<FileInfo>();
            var seps = "/\\".ToCharArray();

            if (System.IO.Directory.Exists(this.RootFolder))
            {
                var files = System.IO.Directory.GetFiles(this.RootFolder, "*.*", System.IO.SearchOption.AllDirectories);
                fileList.AddRange(files.Select(this.GetFileInfo).Where(info => info != null));
            }
            return fileList.ToArray();
        }

        public FileInfo[] FolderFiles(string folder)
        {
            if (string.IsNullOrEmpty(folder))
            {
                folder = this.RootFolder;
            }
            else
            {
                folder = folder.Replace("/", "\\");
                folder = Lib.Helper.IOHelper.CombinePath(this.RootFolder, folder);
            }

            var fileList = new List<FileInfo>();

            if (System.IO.Directory.Exists(folder))
            {
                var files = System.IO.Directory.GetFiles(folder, "*.*", System.IO.SearchOption.TopDirectoryOnly);
                fileList.AddRange(files.Select(this.GetFileInfo).Where(info => info != null));
            }
            return fileList.ToArray();
        }

        public FileInfo[] FolderFiles()
        {
            var list = FolderFiles(null);
            return list.ToArray();
        }

        private FileInfo GetFileInfo(string fullfilename)
        {
            if (System.IO.File.Exists(fullfilename))
            {
                var seps = "/\\".ToCharArray();

                var lastslash = fullfilename.LastIndexOfAny(seps);
                if (lastslash > -1)
                {
                    string name = fullfilename.Substring(lastslash + 1);
                    string fullname = fullfilename.Substring(this.RootFolder.Length + 1);

                    FileInfo info = new FileInfo { Name = name, FullName = fullname };
                    var iofile = new System.IO.FileInfo(fullfilename);
                    info.Size = iofile.Length;
                    info.LastModified = iofile.LastWriteTime;
                    info.StringSize = Lib.Utilities.CalculateUtility.GetSizeString(iofile.Length);

                    string url = fullname.Replace("\\", "/");
                    url = Lib.Helper.UrlHelper.Combine("/__kb/kfile/", url);
                    string absurl = this.Context.WebSite.BaseUrl(url);
                    info.AbsoluteUrl = absurl;
                    info.RelativeUrl = url;
                    return info;
                }
            }
            return null;
        }

        public FolderInfo[] SubFolders(string folder = null)
        {
            if (string.IsNullOrEmpty(folder))
            {
                folder = this.RootFolder;
            }
            else
            {
                folder = folder.Replace("/", "\\");
                folder = Lib.Helper.IOHelper.CombinePath(this.RootFolder, folder);
            }

            List<FolderInfo> subs = new List<FolderInfo>();

            if (System.IO.Directory.Exists(folder))
            {
                var subdirs = System.IO.Directory.GetDirectories(folder, "*.*", System.IO.SearchOption.TopDirectoryOnly);
                foreach (var item in subdirs)
                {
                    string fullname = item.Substring(this.RootFolder.Length + 1);
                    string name = item.Substring(folder.Length + 1);
                    FolderInfo info = new FolderInfo { Name = name, FullName = fullname };
                    subs.Add(info);
                }
            }
            return subs.ToArray();
        }

        public void CreateFolder(string folder, string parentFolder)
        {
            if (string.IsNullOrEmpty(folder))
            {
                return;
            }

            string fulldir;
            if (string.IsNullOrEmpty(parentFolder))
            {
                fulldir = Lib.Helper.IOHelper.CombinePath(this.RootFolder, folder);
            }
            else
            {
                string parentdir = Lib.Helper.IOHelper.CombinePath(this.RootFolder, parentFolder);
                fulldir = Lib.Helper.IOHelper.CombinePath(parentdir, folder);
            }
            Lib.Helper.IOHelper.EnsureDirectoryExists(fulldir);
        }

        public void CreateFolder(string folder)
        {
            CreateFolder(folder, "");
        }

        public void DeleteFolder(string folder)
        {
            if (string.IsNullOrEmpty(folder))
            {
                return;
            }

            string fulldir = Lib.Helper.IOHelper.CombinePath(this.RootFolder, folder);
            if (System.IO.Directory.Exists(fulldir))
            {
                bool ok = true;

                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        System.IO.Directory.Delete(fulldir, true);
                    }
                    catch (System.Exception)
                    {
                        ok = false;
                    }

                    if (ok)
                    {
                        break;
                    }

                    System.Threading.Thread.Sleep(50);
                }
            }
        }
    }

    public class FolderInfo
    {
        public string Name { get; set; }

        public string FullName { get; set; }
    }

    public class FileInfo
    {
        public string Name { get; set; }

        public string FullName { get; set; }

        public long Size { get; set; }

        public string StringSize { get; set; }

        public string RelativeUrl { get; set; }

        public string AbsoluteUrl { get; set; }

        public string Url { get { return this.RelativeUrl; } }

        public System.DateTime LastModified { get; set; }
    }
}