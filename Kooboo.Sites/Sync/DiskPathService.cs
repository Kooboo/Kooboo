//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Interface;
using Kooboo.Data.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Sync
{
    public static class DiskPathService
    { 
        public static string PathPrefix = "___kooboo";

        public static string DefaultExtension = ".kooboo";

        public static string FolderFileName = "folder.kooboo"; 

        private static Dictionary<string, string> _EscapeList;
        /// <summary>
        /// replace some chars that are not allowed as disk file name and convert them back..
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, string> EscapeList()
        {
            if (_EscapeList == null)
            {
                _EscapeList = new Dictionary<string, string>();
                _EscapeList.Add(":", "=KComma");
                _EscapeList.Add("?", "=KQues");
                _EscapeList.Add("*", "=KStart");
                _EscapeList.Add(">", "=KRight");
                _EscapeList.Add("<", "=KLeft");
            }
            return _EscapeList;
        }
        
        public static string GetFullDiskPath(Data.Models.WebSite WebSite, string relativeUrl)
        {
            relativeUrl = ReplaceQuestionMark(relativeUrl, false);

            var relativepath = EscapeChar(relativeUrl, false);

            string FullPath = Lib.Compatible.CompatibleManager.Instance.System.CombinePath(WebSite.DiskSyncFolder, relativepath);

            if (FullPath.EndsWith("/") || FullPath.EndsWith("\\"))
            {
                FullPath = FullPath + FolderFileName;
            }
            else
            {
                System.IO.FileInfo file = new System.IO.FileInfo(FullPath);
                if (String.IsNullOrEmpty(file.Extension))
                {
                    FullPath = FullPath + DefaultExtension;
                }
            }

            return FullPath;
        }

        public static string GetRelativeUrl(Data.Models.WebSite WebSite, string FullPath)
        {
            if (FullPath.EndsWith(FolderFileName, StringComparison.OrdinalIgnoreCase))
            { 
                FullPath = FullPath.Substring(0, FullPath.Length - FolderFileName.Length);
            }
            else if (FullPath.EndsWith(DefaultExtension))
            { 
                FullPath = FullPath.Substring(0, FullPath.Length - DefaultExtension.Length);
            }

            FullPath = EscapeChar(FullPath, true);

            string relative = Lib.Helper.StringHelper.ReplaceIgnoreCase(FullPath, WebSite.DiskSyncFolder, "");

            if (!string.IsNullOrEmpty(relative))
            {
                relative = relative.Replace("\\", "/");
            }

            relative = ReplaceQuestionMark(relative, true);

            return relative;
        }

        public static string CleanRelativeUrl(string relativePath)
        {
            if (relativePath.EndsWith(FolderFileName, StringComparison.OrdinalIgnoreCase))
            {
                relativePath = relativePath.Substring(0, relativePath.Length - FolderFileName.Length);
            }
            else if (relativePath.EndsWith(DefaultExtension))
            {
                relativePath = relativePath.Substring(0, relativePath.Length - DefaultExtension.Length);
            }

            relativePath = EscapeChar(relativePath, true);
             
            if (!string.IsNullOrEmpty(relativePath))
            {
                relativePath = relativePath.Replace("\\", "/");
            }

            relativePath = ReplaceQuestionMark(relativePath, true);
 
            if (!relativePath.StartsWith("/"))
            {
                relativePath = "/" + relativePath;
            }

            return relativePath;
        }


        public static string EscapeChar(string input, bool Back)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            var charlist = EscapeList();
            if (Back)
            {
                foreach (var item in charlist)
                {
                    input = input.Replace(item.Value, item.Key);
                } 
            }
            else
            { 
                foreach (var item in charlist)
                {
                    input = input.Replace(item.Key, item.Value);
                } 
            }
            return input;

        }

        private static string ReplaceQuestionMark(string input, bool IsBack)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            string start = "((?";
            string end = "))";
            string result = input;

            if (IsBack)
            {
                int startindex = input.IndexOf(start);
                if (startindex > -1)
                {
                    int endindex = input.IndexOf(end, startindex);
                    if (endindex > -1)
                    {
                        string first = input.Substring(0, startindex);
                        string middle = input.Substring(startindex + 3, endindex - startindex - 3);

                        string last = string.Empty;
                        if (endindex + 1 < input.Length)
                        {
                            last = input.Substring(endindex + 2);
                        }
                        result = first + last + "?" + middle;
                    }
                }
            }
            else
            {
                int questionMarkStart = input.IndexOf("?");
                if (questionMarkStart == -1)
                {
                    return input;
                }
                string markstring = string.Empty;

                markstring = input.Substring(questionMarkStart);
                if (!string.IsNullOrEmpty(markstring))
                {
                    markstring = markstring.Substring(1);
                    markstring = start + markstring + end;
                }

                input = input.Substring(0, questionMarkStart);

                int slashposition = input.LastIndexOf("/");
                if (slashposition == -1)
                {
                    slashposition = 0;
                }

                string xbegin = input.Substring(0, slashposition + 1);
                string xend = input.Substring(slashposition);
                if (!string.IsNullOrEmpty(xend))
                {
                    xend = xend.Substring(1);
                }

                result = xbegin + markstring + xend;
            }

            return result;

        }
 
        public static string GetObjectRelativeUrl(ISiteObject siteobject, SiteDb sitedb, string StoreName = null)
        {
            if (siteobject == null)
            {
                return null; 
            }

            if (Attributes.AttributeHelper.IsRoutable(siteobject))
            {
                return sitedb.Routes.GetObjectPrimaryRelativeUrl(siteobject.Id);
            }
            else
            {
                string typepath = string.IsNullOrEmpty(StoreName) ? siteobject.GetType().Name : StoreName; 
                string name = Attributes.AttributeHelper.IsNameAsId(siteobject) ? siteobject.Name : siteobject.Id.ToString(); 

                string extension = DefaultExtension; 
                if (siteobject is IExtensionable)
                {
                    var extensionboject = siteobject as IExtensionable;
                    if (!string.IsNullOrEmpty(extensionboject.Extension))
                    {
                        extension = extensionboject.Extension;
                    }
                }
                if (!extension.StartsWith("."))
                {
                    extension = "." + extension; 
                } 

                if (!name.ToLower().EndsWith(extension))
                {
                    name = name + extension; 
                }

                return "/" + PathPrefix + "/" + typepath + "/" + name;  
            }
        }
         

        public static string GetNonRoutableFolder(string FullOrRelativePath)
        {
            FullOrRelativePath = FullOrRelativePath.Replace("/", "\\");

            string path = "\\" + PathPrefix + "\\";

            int index = FullOrRelativePath.IndexOf(path, StringComparison.OrdinalIgnoreCase);

            if (index > -1)
            {
                string substring = FullOrRelativePath.Substring(index + path.Length);

                if (substring.StartsWith("\\"))
                {
                    substring = substring.Substring(1);
                }

                if (substring.EndsWith("\\"))
                {
                    substring = substring.TrimEnd('\\'); 
                }

                return substring; 
            }
            return null;
        }

        public static NonRoutableObject GetNonRoutableObject(string FullOrRelativePath)
        {
            FullOrRelativePath = FullOrRelativePath.Replace("/", "\\"); 
            
            string path = "\\" + PathPrefix + "\\";

            int index = FullOrRelativePath.IndexOf(path, StringComparison.OrdinalIgnoreCase);

            if (index == -1)
            {
                if (FullOrRelativePath.StartsWith(PathPrefix+ "\\", StringComparison.OrdinalIgnoreCase))
                {
                    FullOrRelativePath = "\\" + FullOrRelativePath; 
                    index = 0; 
                }
            }

            if (index > -1)
            {
                string substring = FullOrRelativePath.Substring(index + path.Length);

                if (substring.StartsWith("\\"))
                {
                    substring = substring.Substring(1);
                }

                string[] splited = substring.Split('\\');

                if (splited.Length > 1)
                {
                    var storename = splited[0]; 

                    if (!string.IsNullOrEmpty(storename))
                    {
                        NonRoutableObject item = new NonRoutableObject();
                        item.StoreName = storename;  

                        string fullname = splited[1];
                        if (fullname.EndsWith(DefaultExtension))
                        {
                            fullname = fullname.Substring(0, fullname.Length - DefaultExtension.Length);
                        }
                        else
                        {
                            int lastdot = fullname.LastIndexOf(".");

                            if (lastdot > 0)
                            { 
                                string extension = fullname.Substring(lastdot + 1);
                                 fullname = fullname.Substring(0, lastdot);

                                item.Extension = extension; 
                            }
                        }

                        Guid id;
                        if (System.Guid.TryParse(fullname, out id))
                        {
                            item.Id = id;
                        }
                        else
                        {
                            item.Name = fullname;
                        }

                        return item;
                    }
                }
            } 
            return null; 
        }

        public static bool IsDirectory(Data.Models.WebSite website, string fullpath)
        {
            if (fullpath.EndsWith("\\"))
            { return true; }

            string path = "\\" + PathPrefix + "\\";

            if (fullpath.IndexOf(path, StringComparison.OrdinalIgnoreCase) > -1)
            {
                int index = fullpath.IndexOf(path, StringComparison.OrdinalIgnoreCase);
                string substring = fullpath.Substring(index + path.Length);
                if (substring.StartsWith("\\"))
                {
                    substring = substring.Substring(1); 
                }
                substring = substring.TrimEnd('\\'); 
                if (substring.IndexOf("\\") > -1)
                {
                    return false;
                }
                return true; 
            }

            else
            {
                string relative = GetRelativeUrl(website, fullpath); 
                if (string.IsNullOrEmpty(relative))
                {
                    return true; 
                } 
                var route = website.SiteDb().Routes.GetByUrl(relative); 
                if (route !=null && route.objectId != default(Guid))
                {
                    return false; 
                }

                if (relative.EndsWith("\\") || relative.EndsWith("/"))
                {
                    return true; 
                }

                int lastindex = relative.LastIndexOf("\\");
                int lastbackslash = relative.LastIndexOf("/"); 
                if (lastbackslash > lastindex)
                {
                    lastindex = lastbackslash; 
                }
                if (lastindex > -1)
                {
                    string lastpart = relative.Substring(lastindex);
                    return !lastpart.Contains(".");  
                }
                else
                {
                    return !relative.Contains(".");
                }
            }

         
        }
    }
     
    public class NonRoutableObject
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string StoreName { get; set; } 
        public string Extension { get; set; }

    }

}
