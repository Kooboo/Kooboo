//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Routing;
using System;
using System.Collections.Generic;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Data.Models;
using Kooboo.Data.Extensions;
using Kooboo.Sites.Repository;
using System.Linq;

namespace Kooboo.Sites.Service
{
    public static class PathService
    {
        /// <summary>
        /// return the full navigation from root to this path. 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFullPath(Path path)
        {
            string FullPath = "/" + path.segment;

            var parent = path.ParentPath;
            while (parent != null && !string.IsNullOrEmpty(parent.segment))
            {
                FullPath = "/" + parent.segment + FullPath;
                parent = parent.ParentPath;
            }

            return FullPath;
        }

        public static List<CrumbPath> GetCrumbPath(string FullFolderPath)
        {
            if (string.IsNullOrEmpty(FullFolderPath))
            {
                return null;
            }
            FullFolderPath = FullFolderPath.Trim();

            if (!FullFolderPath.StartsWith("/"))
            {
                FullFolderPath = "/" + FullFolderPath;
            }

            List<CrumbPath> result = new List<CrumbPath>();

            string[] segments = FullFolderPath.Split('/');

            bool rootused = false;
            string currentpath = string.Empty;

            foreach (var item in segments)
            {
                CrumbPath path = new CrumbPath();
                if (!currentpath.EndsWith("/"))
                {
                    currentpath = currentpath + "/";
                }
                currentpath = currentpath + item;
                path.FullPath = currentpath;
                string name = item;

                if (string.IsNullOrEmpty(item))
                {
                    if (!rootused)
                    {
                        name = Kooboo.DataConstants.RootPathName;
                        rootused = true;
                    }
                }

                if (!string.IsNullOrEmpty(name))
                {
                    path.Name = name;
                    result.Add(path);
                }

            }

            return result;

        }

        public static DateTime GetLastModified(SiteDb SiteDb, string FullPath, byte ConstType)
        {
            DateTime LastTime = Kooboo.IndexedDB.GlobalSettings.UTCStartdate;
            var allfolder = SiteDb.Folders.GetDescendantFolders(FullPath, ConstType);

            bool hastime = false;

            if (allfolder != null && allfolder.Count > 0)
            {
                foreach (var item in allfolder)
                {
                    if (item.LastModified > LastTime)
                    {
                        LastTime = item.LastModified;
                    }
                }
                hastime = true;
            }

            var allobjects = SiteDb.Folders.GetFolderObjects(FullPath, ConstType, true, true);

            if (allobjects != null && allobjects.Count > 0)
            {
                foreach (var item in allobjects)
                {
                    if (item.LastModified > LastTime)
                    {
                        LastTime = item.LastModified;
                    }
                }
                hastime = true;
            }

            if (!hastime)
            {
                var folder = SiteDb.Folders.GetFolder(FullPath, ConstType);
                if (folder != null)
                {
                    return folder.LastModified;
                }
            }
            return LastTime;
        }

        public static List<string> FindCommonPath(List<string> paths)
        {
            Func<List<string>, bool> HasSameValue = (list) =>
            {
                string current = null;

                foreach (var item in list)
                {
                    if (current == null)
                    {
                        current = item;
                        if (current.ToLower().Contains("_kooboo"))
                        {
                            return false; 
                        }
                    }
                    else
                    {
                        if (!Kooboo.Lib.Helper.StringHelper.IsSameValue(current, item))
                        {
                            return false;
                        } 
                    }
                }
                return true;
            };

            Func<string, List<string>> ToSegments = Kooboo.Lib.Compatible.CompatibleManager.Instance.System.GetSegments; ; ;

            List<List<string>> AllSegments = new List<List<string>>();

            foreach (var item in paths)
            {
                AllSegments.Add(ToSegments(item));
            }

            List<string> common = new List<string>();

            int i = 0;

            while (i < 999)
            {
                List<string> indexitem = new List<string>();
                foreach (var item in AllSegments)
                {
                    if (i > item.Count() - 1)
                    {
                        break;
                    }
                    else
                    {
                        indexitem.Add(item[i]);
                    }
                }
                if (indexitem.Count() > 0 && HasSameValue(indexitem))
                {
                    common.Add(indexitem[0]);
                    i += 1;
                }
                else
                {
                    break;
                }
            }

            return common;  
             
        }
         
    }
     
}
