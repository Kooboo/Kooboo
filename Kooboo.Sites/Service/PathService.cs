//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
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
            string fullPath = "/" + path.segment;

            var parent = path.ParentPath;
            while (parent != null && !string.IsNullOrEmpty(parent.segment))
            {
                fullPath = "/" + parent.segment + fullPath;
                parent = parent.ParentPath;
            }

            return fullPath;
        }

        public static List<CrumbPath> GetCrumbPath(string fullFolderPath)
        {
            if (string.IsNullOrEmpty(fullFolderPath))
            {
                return null;
            }
            fullFolderPath = fullFolderPath.Trim();

            if (!fullFolderPath.StartsWith("/"))
            {
                fullFolderPath = "/" + fullFolderPath;
            }

            List<CrumbPath> result = new List<CrumbPath>();

            string[] segments = fullFolderPath.Split('/');

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

        public static DateTime GetLastModified(SiteDb siteDb, string fullPath, byte constType)
        {
            DateTime lastTime = Kooboo.IndexedDB.GlobalSettings.UTCStartdate;
            var allfolder = siteDb.Folders.GetDescendantFolders(fullPath, constType);

            bool hastime = false;

            if (allfolder != null && allfolder.Count > 0)
            {
                foreach (var item in allfolder)
                {
                    if (item.LastModified > lastTime)
                    {
                        lastTime = item.LastModified;
                    }
                }
                hastime = true;
            }

            var allobjects = siteDb.Folders.GetFolderObjects(fullPath, constType, true, true);

            if (allobjects != null && allobjects.Count > 0)
            {
                foreach (var item in allobjects)
                {
                    if (item.LastModified > lastTime)
                    {
                        lastTime = item.LastModified;
                    }
                }
                hastime = true;
            }

            if (!hastime)
            {
                var folder = siteDb.Folders.GetFolder(fullPath, constType);
                if (folder != null)
                {
                    return folder.LastModified;
                }
            }
            return lastTime;
        }

        public static List<string> FindCommonPath(List<string> paths)
        {
            Func<List<string>, bool> hasSameValue = (list) =>
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

            Func<string, List<string>> toSegments = Kooboo.Lib.Compatible.CompatibleManager.Instance.System.GetSegments; ; ;

            List<List<string>> allSegments = new List<List<string>>();

            foreach (var item in paths)
            {
                allSegments.Add(toSegments(item));
            }

            List<string> common = new List<string>();

            int i = 0;

            while (i < 999)
            {
                List<string> indexitem = new List<string>();
                foreach (var item in allSegments)
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
                if (indexitem.Any() && hasSameValue(indexitem))
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