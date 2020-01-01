//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data;
using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace KScript
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
        [Description(@"Write the text to the file name. When the target exists, it will be overwritten.
 k.file.write(""folder\filename.txt"", ""content to write to text file"");
          var info = k.file.write(""rootfile.txt"", ""content to write to text file"");")]
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

        [Description(@"Write an array of bytes to the site disk folder
    if (k.request.method=""POST""){ 
			  if (k.request.files.length > 0){ 
                  var file = k.request.files[0]; 
                  var info = k.file.writeBinary(file.fileName, file.bytes); 
              }
}")]
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


        [Description(@"Write the text to the file name. When the target does NOT exist, it will be created
k.file.append(""filename.txt"", ""content to append to text file"");")]
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

            string filename = Kooboo.Lib.Helper.IOHelper.CombinePath(this.RootFolder, valid);

            if (!filename.StartsWith(this.RootFolder))
            {
                return null; // this is not allowed, as it may try to write to other folders. 
            }

            Kooboo.Lib.Helper.IOHelper.EnsureFileDirectoryExists(filename);

            return filename;
        }

        [KIgnore]
        private string ToValidPath(string input)
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
            

        [Description("return the relative url path to access the file")]
        public string url(string filename)
        {
            if (filename.StartsWith(this.RootFolder))
            {
                filename = filename.Substring(this.RootFolder.Length + 1);
            }
            string url = "/__kb/kfile/";
            return Kooboo.Lib.Helper.UrlHelper.Combine(url, filename);
        }


        [Description(@"Read all the text of the file
var value = k.file.read(""filename.txt"");")]
        public string read(string FileName)
        {
            var name = _getfullname(FileName);
            if (!string.IsNullOrWhiteSpace(name) && System.IO.File.Exists(name))
            {
                return System.IO.File.ReadAllText(name);
            }
            return null;
        }

        [Description(@"read the file into a byte array
			var bytes = k.file.readBinary(""file.pdf""); 
            var info = k.file.writeBinary(""newname.pdf"", bytes);")]
        public byte[] readBinary(string FileName)
        {
            var name = _getfullname(FileName);
            if (!string.IsNullOrWhiteSpace(name) && System.IO.File.Exists(name))
            {
                return System.IO.File.ReadAllBytes(name);
            }
            return null;
        }

        [Description(@"Check whether the file exists or not, filename can be:/folder/filename.txt.
 if (k.file.exists(""filename.txt"")){}")]
        public bool Exists(string FileName)
        {
            var name = _getfullname(FileName);

            return System.IO.File.Exists(name);
        }

        [Description(@"Delete the file
k.file.delete(""filename.txt"");")]
        public void delete(string FileName)
        {
            var name = _getfullname(FileName);
            if (System.IO.File.Exists(name))
            {
                System.IO.File.Delete(name);
            }
        }

        [Description(@"Return all files in all folders, return an Array of FileInfo
var allfiles= k.file.getAllFiles();")]
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


        [Description(@"Return all files in the provided folder, return an Array of FileInfo
var folderFiles = k.file.folderFiles(k.request.folder);")]
        public FileInfo[] FolderFiles(string folder)
        {
            if (string.IsNullOrEmpty(folder))
            {
                folder = this.RootFolder;
            }
            else
            {
                folder = folder.Replace("/", "\\");
                folder = Kooboo.Lib.Helper.IOHelper.CombinePath(this.RootFolder, folder);
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

        [Description(@"Return all files under the root folder, return an Array of FileInfo
var folderFiles = k.file.folderFiles();")]
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
                        info.StringSize = Kooboo.Lib.Utilities.CalculateUtility.GetSizeString(iofile.Length);
                    }

                    string url = fullname.Replace("\\", "/");
                    url = Kooboo.Lib.Helper.UrlHelper.Combine("/__kb/kfile/", url);
                    string absurl = this.context.WebSite.BaseUrl(url);
                    info.AbsoluteUrl = absurl;
                    info.RelativeUrl = url;
                    return info;
                }
            }
            return null;
        }

        [Description(@"List sub folders under current folder, return an Array of FolderInfo
var subfolders = k.file.subFolders(k.request.folder);")]
        public FolderInfo[] SubFolders(string folder = null)
        {
            if (string.IsNullOrEmpty(folder))
            {
                folder = this.RootFolder;
            }
            else
            {
                folder = folder.Replace("/", "\\");
                folder = Kooboo.Lib.Helper.IOHelper.CombinePath(this.RootFolder, folder);
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

        [Description(@"create a sub folder under current folder")]
        public void CreateFolder(string Folder, string ParentFolder)
        {
            if (string.IsNullOrEmpty(Folder))
            {
                return;
            }

            string fulldir = null;
            if (string.IsNullOrEmpty(ParentFolder))
            {
                fulldir = Kooboo.Lib.Helper.IOHelper.CombinePath(this.RootFolder, Folder);
            }
            else
            {
                string parentdir = Kooboo.Lib.Helper.IOHelper.CombinePath(this.RootFolder, ParentFolder);
                fulldir = Kooboo.Lib.Helper.IOHelper.CombinePath(parentdir, Folder);
            }
            Kooboo.Lib.Helper.IOHelper.EnsureDirectoryExists(fulldir);
        }

        [Description(@"create a sub folder under root folder")]
        public void CreateFolder(string Folder)
        {
            CreateFolder(Folder, ""); 
        }

        [Description(@"Delete a folder and all sub directories and files in it.
k.file.deleteFolder(k.request.deleteFolder);")]
        public void DeleteFolder(string Folder)
        {
            if (string.IsNullOrEmpty(Folder))
            {
                return;
            }

            string fulldir = Kooboo.Lib.Helper.IOHelper.CombinePath(this.RootFolder, Folder);
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
