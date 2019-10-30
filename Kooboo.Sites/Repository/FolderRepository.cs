//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB;
using Kooboo.Sites.Models;
using Kooboo.Sites.Routing;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Repository
{
    public class FolderRepository : SiteRepositoryBase<Kooboo.Sites.Models.Folder>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters paras = new ObjectStoreParameters();
                paras.AddColumn<Folder>(o => o.ObjectConstType);
                paras.SetPrimaryKeyField<Folder>(o => o.Id);
                return paras;
            }
        }

        public List<Folder> GetSubFolders(string parentFullPath, byte constObjectType)
        {
            List<Folder> result = new List<Folder>();
            if (string.IsNullOrEmpty(parentFullPath))
            {
                parentFullPath = "/";
            }
            Func<string, string, string> GetDirectSub = (parent, sub) =>
              {
                  if (string.IsNullOrEmpty(sub) || string.IsNullOrEmpty(parent))
                  {
                      return null;
                  }
                  if (sub.Length <= parent.Length)
                  {
                      return null;
                  }
                  string left = sub.Substring(parent.Length);
                  string[] segments = left.Split('/');
                  foreach (var item in segments)
                  {
                      if (!string.IsNullOrEmpty(item))
                      {
                          return item;
                      }
                  }
                  return null;
              };

            Action<string, string> AddNewFolder = (folderSeg, fullPath) =>
            {
                var lowerfolder = folderSeg.ToLower();

                if (result.Find(o => o.Segment.ToLower() == lowerfolder) == null)
                {
                    Folder subfolder = new Folder(constObjectType) {FullPath = fullPath, Segment = folderSeg};
                    result.Add(subfolder);
                }
            };

            parentFullPath = parentFullPath.ToLower();
            var subfolders = GetDescendantFolders(parentFullPath, constObjectType);

            foreach (var item in subfolders)
            {
                string directsub = GetDirectSub(parentFullPath, item.FullPath);
                if (!string.IsNullOrEmpty(directsub))
                {
                    string fullpath = CombineFolder(parentFullPath, directsub);
                    AddNewFolder(directsub, fullpath);
                }
            }

            PathTree tree = Cache.RouteTreeCache.RouteTree(this.SiteDb, constObjectType);

            var path = tree.FindPath(parentFullPath, false);
            if (path == null)
            {
                return result;
            }
            foreach (var item in path.Children)
            {
                if (item.Value.RouteId == default(Guid) && PathHasSubObject(item.Value))
                {
                    string fullPath = CombineFolder(parentFullPath, item.Value.segment);
                    string segment = item.Value.segment;
                    AddNewFolder(segment, fullPath);
                }
            }
            return result;
        }

        private bool PathHasSubObject(Path path)
        {
            if (path.RouteId != default(Guid))
            {
                var route = this.SiteDb.Routes.Get(path.RouteId);
                if (route != null && route.objectId != default(Guid))
                {
                    return true;
                }
            }

            foreach (var item in path.Children)
            {
                if (PathHasSubObject(item.Value))
                {
                    return true;
                }
            }

            return false;
        }

        public string CombineFolder(string Base, string sub)
        {
            if (!Base.EndsWith("/"))
            {
                Base += "/";
            }
            return Base + sub;
        }

        /// <summary>
        /// Get all defined Descendant folders, this does not include the fake folders that generated from routes.
        /// </summary>
        /// <param name="parentFullPath"></param>
        /// <param name="constObjectType"></param>
        /// <returns></returns>
        public List<Folder> GetDescendantFolders(string parentFullPath, byte constObjectType)
        {
            Func<string, string, bool> IsSubFolder = (parent, sub) =>
            {
                if (string.IsNullOrEmpty(sub))
                {
                    return false;
                }
                return (sub.ToLower().StartsWith(parent) && sub.ToLower() != parent);
            };

            if (string.IsNullOrEmpty(parentFullPath))
            {
                parentFullPath = "/";
            }

            if (!parentFullPath.StartsWith("/"))
            {
                parentFullPath = "/" + parentFullPath;
            }
            parentFullPath = parentFullPath.ToLower();

            if (!parentFullPath.EndsWith("/"))
            {
                parentFullPath = parentFullPath + "/";
            }

            return this.TableScan.Where(o => o.ObjectConstType == constObjectType && IsSubFolder(parentFullPath, o.FullPath)).SelectAll();
        }

        public Folder GetFolder(string fullPath, byte constyObjectType)
        {
            Guid id = Kooboo.Data.IDGenerator.GetFolderId(fullPath, constyObjectType);

            return this.Get(id);
        }

        public void AddOrUpdate(byte constType, string path)
        {
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }
            if (path.EndsWith("/"))
            {
                path = path.TrimEnd('/');
            }

            Folder folder = new Folder(constType) {FullPath = path};
            base.AddOrUpdate(folder);
        }

        public List<T> GetFolderObjects<T>(string FolderPath, bool UseColumnDataOnly = false, bool Recursive = false) where T : SiteObject
        {
            List<T> list = new List<T>();

            byte consttype = ConstTypeContainer.GetConstType(typeof(T));

            var siteobjectlist = GetFolderObjects(FolderPath, consttype, UseColumnDataOnly, Recursive);

            foreach (var item in siteobjectlist)
            {
                var tvalue = (T)item;
                if (tvalue != null)
                {
                    list.Add(tvalue);
                }
            }

            return list;
        }

        public List<SiteObject> GetFolderObjects(string folderPath, byte constObjectType, bool useColumnDataOnly = false, bool recursive = false)
        {
            List<SiteObject> result = new List<SiteObject>();

            var routes = GetFolderObjectRouteIds(folderPath, constObjectType, recursive);

            foreach (var item in routes)
            {
                var route = SiteDb.Routes.Get(item);
                if (route != null && route.objectId != default(Guid) && route.DestinationConstType == constObjectType)
                {
                    var siteobject = Service.ObjectService.GetSiteObject(this.SiteDb, route.objectId, constObjectType, useColumnDataOnly);
                    if (siteobject != null && siteobject is SiteObject siteObject)
                    {
                        result.Add(siteObject);
                    }
                }
            }

            return result;
        }

        public List<Guid> GetFolderObjectRouteIds(string folderPath, byte constType, bool recursive = false)
        {
            List<Guid> list = new List<Guid>();

            var roottree = Cache.RouteTreeCache.RouteTree(this.SiteDb, constType);

            Path folderpath = roottree.FindPath(folderPath, false);

            if (folderpath == null)
            {
                return list;
            }

            foreach (var item in folderpath.Children)
            {
                if (item.Value.RouteId != default(Guid))
                {
                    list.Add(item.Value.RouteId);
                }
                if (recursive)
                {
                    if (!item.Value.segment.StartsWith("?"))
                    {
                        string newpath = folderPath + "/" + item.Value.segment;
                        var subs = GetFolderObjectRouteIds(newpath, constType, true);

                        if (subs != null && subs.Count > 0)
                        {
                            list.AddRange(subs);
                        }
                    }
                }
            }
            return list;
        }

        public void Delete(string folderFullPath, byte constType, Guid userId = default(Guid))
        {
            var modeltype = Service.ConstTypeService.GetModelType(constType);
            var repo = this.SiteDb.GetRepository(modeltype);

            var allroutes = GetFolderObjectRouteIds(folderFullPath, constType, true);

            foreach (var item in allroutes)
            {
                var route = this.SiteDb.Routes.Get(item);
                if (route != null)
                {
                    if (route.DestinationConstType == constType && route.objectId != default(Guid))
                    {
                        repo.Delete(route.objectId);
                    }
                    //if (route.DestinationConstType == ConstType)
                    //{
                    //    if (route.objectId == default(Guid))
                    //    {
                    //        this.SiteDb.Routes.Delete(route.Id);
                    //    }
                    //    else
                    //    {
                    //        repo.Delete(route.objectId);
                    //    }
                    //}
                }
            }

            var allfolders = GetDescendantFolders(folderFullPath, constType);

            foreach (var item in allfolders)
            {
                SiteDb.Folders.Delete(item.FullPath, constType, userId);
            }

            var selfid = Kooboo.Data.IDGenerator.GetFolderId(folderFullPath, constType);
            SiteDb.Folders.Delete(selfid, userId);
        }
    }
}