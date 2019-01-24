//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data;
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
        private string RootFolder
        {
            get
            {
                if (_rootfolder == null)
                {
                    _rootfolder = Kooboo.Data.AppSettings.GetFileIORoot(this.context.WebSite);
                }
                return _rootfolder;
            }
        }

        private RenderContext context { get; set; }

        public FileIO(RenderContext context)
        {
            this.context = context;
        }

        public FileInfo write(string fileName, string content)
        {
            var name = _getfullname(fileName);

            if (!string.IsNullOrEmpty(name))
            {
                System.IO.File.WriteAllText(name, content);
                return GetFileInfo(name); 
            }              
            return null; 
        }

        public FileInfo writeBinary(string fileName, byte[] binary)
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

        public void append(string FileName, string content)
        {
            var name = _getfullname(FileName);
            if (!string.IsNullOrWhiteSpace(name))
            {
                System.IO.File.AppendAllText(name, content);
            }
        }

        private string _getfullname(string FileName)
        {
            var valid = ToValidPath(FileName);

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
            

        public string url(string filename)
        {
            if (filename.StartsWith(this.RootFolder))
            {
                filename = filename.Substring(this.RootFolder.Length + 1);
            }
            string url = "/__kb/kfile/";
            return Lib.Helper.UrlHelper.Combine(url, filename);
        }

        public string read(string FileName)
        {
            var name = _getfullname(FileName);
            if (!string.IsNullOrWhiteSpace(name) && System.IO.File.Exists(name))
            {
                return System.IO.File.ReadAllText(name);
            }
            return null;
        }

        public byte[] readBinary(string FileName)
        {
            var name = _getfullname(FileName);
            if (!string.IsNullOrWhiteSpace(name) && System.IO.File.Exists(name))
            {
                return System.IO.File.ReadAllBytes(name);
            }
            return null;
        }

        public bool Exists(string FileName)
        {
            var name = _getfullname(FileName);

            return System.IO.File.Exists(name);
        }

        public void delete(string FileName)
        {
            var name = _getfullname(FileName);
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
                foreach (var file in files)
                {
                    var info = this.GetFileInfo(file);
                    if (info != null)
                    {
                        fileList.Add(info);
                    }
                }
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
                foreach (var file in files)
                {
                    var info = this.GetFileInfo(file);
                    if (info != null)
                    {
                        fileList.Add(info);
                    }
                }
            }
            return fileList.ToArray();
        }


        public FileInfo[] FolderFiles()
        {
            var list= FolderFiles(null);
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

                    FileInfo info = new FileInfo();
                    info.Name = name;
                    info.FullName = fullname;
                    var iofile = new System.IO.FileInfo(fullfilename);
                    if (iofile != null)
                    {
                        info.Size = iofile.Length;
                        info.LastModified = iofile.LastWriteTime;
                        info.StringSize = Lib.Utilities.CalculateUtility.GetSizeString(iofile.Length);
                    }

                    string url = fullname.Replace("\\", "/");
                    url = Lib.Helper.UrlHelper.Combine("/__kb/kfile/", url);
                    string absurl = this.context.WebSite.BaseUrl(url);
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
                    FolderInfo info = new FolderInfo();
                    info.Name = name;
                    info.FullName = fullname;
                    subs.Add(info);
                }
            }
            return subs.ToArray();
        }

        public void CreateFolder(string Folder, string ParentFolder)
        {
            if (string.IsNullOrEmpty(Folder))
            {
                return;
            }

            string fulldir = null;
            if (string.IsNullOrEmpty(ParentFolder))
            {
                fulldir = Lib.Helper.IOHelper.CombinePath(this.RootFolder, Folder);
            }
            else
            {
                string parentdir = Lib.Helper.IOHelper.CombinePath(this.RootFolder, ParentFolder);
                fulldir = Lib.Helper.IOHelper.CombinePath(parentdir, Folder);
            }
            Lib.Helper.IOHelper.EnsureDirectoryExists(fulldir);
        }

        public void CreateFolder(string Folder)
        {
            CreateFolder(Folder, ""); 
        }

        public void DeleteFolder(string Folder)
        {
            if (string.IsNullOrEmpty(Folder))
            {
                return;
            }

            string fulldir = Lib.Helper.IOHelper.CombinePath(this.RootFolder, Folder);
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

        public string url { get { return this.RelativeUrl;  } }

        public System.DateTime LastModified { get; set; }
    }
}
