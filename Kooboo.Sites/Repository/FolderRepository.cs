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
 
        public List<Folder> GetSubFolders(string ParentFullPath, byte ConstObjectType)
        {
            List<Folder> Result = new List<Folder>();
            if (string.IsNullOrEmpty(ParentFullPath))
            {
                ParentFullPath = "/";
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

            Action<string, string> AddNewFolder = (FolderSeg, FullPath) =>
            {
                var lowerfolder = FolderSeg.ToLower();

                if (Result.Find(o => o.Segment.ToLower() == lowerfolder) == null)
                {
                    Folder subfolder = new Folder(ConstObjectType);
                    subfolder.FullPath = FullPath;
                    subfolder.Segment = FolderSeg;
                    Result.Add(subfolder);
                }
            };
             

            ParentFullPath = ParentFullPath.ToLower();
            var subfolders = GetDescendantFolders(ParentFullPath, ConstObjectType);

            foreach (var item in subfolders)
            {
                string directsub = GetDirectSub(ParentFullPath, item.FullPath);
                if (!string.IsNullOrEmpty(directsub))
                {
                    string fullpath = CombineFolder(ParentFullPath, directsub);
                    AddNewFolder(directsub, fullpath);
                }
            }

            PathTree tree = Cache.RouteTreeCache.RouteTree(this.SiteDb, ConstObjectType);

            var path = tree.FindPath(ParentFullPath, false);
            if (path == null)
            {
                return Result;
            }
            foreach (var item in path.Children)
            {
                if (item.Value.RouteId == default(Guid) && PathHasSubObject(item.Value))
                {
                    string FullPath = CombineFolder(ParentFullPath, item.Value.segment);
                    string Segment = item.Value.segment;
                    AddNewFolder(Segment, FullPath);
                }
            }
            return Result;
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
                Base = Base + "/";
            }
            return Base + sub;
        }
        /// <summary>
        /// Get all defined Descendant folders, this does not include the fake folders that generated from routes. 
        /// </summary>
        /// <param name="ParentFullPath"></param>
        /// <param name="ConstObjectType"></param>
        /// <returns></returns>
        public List<Folder> GetDescendantFolders(string ParentFullPath, byte ConstObjectType)
        {
            Func<string, string, bool> IsSubFolder = (parent, sub) =>
            {
                if (string.IsNullOrEmpty(sub))
                {
                    return false;
                }
                return (sub.ToLower().StartsWith(parent) && sub.ToLower() != parent);
            };

            if (string.IsNullOrEmpty(ParentFullPath))
            {
                ParentFullPath = "/";
            }

            if (!ParentFullPath.StartsWith("/"))
            {
                ParentFullPath = "/" + ParentFullPath;
            }
            ParentFullPath = ParentFullPath.ToLower(); 

            if (!ParentFullPath.EndsWith("/"))
            {
                ParentFullPath = ParentFullPath + "/"; 
            }

            return this.TableScan.Where(o => o.ObjectConstType == ConstObjectType && IsSubFolder(ParentFullPath, o.FullPath)).SelectAll();
        }

        public Folder GetFolder(string FullPath, byte ConstyObjectType)
        {
            Guid id = Kooboo.Data.IDGenerator.GetFolderId(FullPath, ConstyObjectType);

            return this.Get(id);
        }

        public void AddOrUpdate(byte ConstType, string Path)
        {
            if (!Path.StartsWith("/"))
            {
                Path = "/" + Path;
            }
            if (Path.EndsWith("/"))
            {
                Path = Path.TrimEnd('/');
            }
            Folder folder = new Folder(ConstType);
            folder.FullPath = Path;
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

        public List<SiteObject> GetFolderObjects(string FolderPath, byte ConstObjectType, bool UseColumnDataOnly = false, bool Recursive = false)
        {
            List<SiteObject> Result = new List<SiteObject>();

            var routes = GetFolderObjectRouteIds(FolderPath, ConstObjectType, Recursive);

            foreach (var item in routes)
            {
                var route = SiteDb.Routes.Get(item);
                if (route != null && route.objectId != default(Guid) && route.DestinationConstType == ConstObjectType)
                {
                    var siteobject = Service.ObjectService.GetSiteObject(this.SiteDb, route.objectId, ConstObjectType, UseColumnDataOnly);
                    if (siteobject != null  &&　siteobject is SiteObject)
                    {
                        Result.Add(siteobject as SiteObject);
                    }
                }
            }

            return Result;
        }

        public List<Guid> GetFolderObjectRouteIds(string FolderPath, byte ConstType, bool Recursive = false)
        {
            List<Guid> list = new List<Guid>();

            var roottree = Cache.RouteTreeCache.RouteTree(this.SiteDb, ConstType);

            Path folderpath = roottree.FindPath(FolderPath, false); 

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
                if (Recursive)
                {
                    if (!item.Value.segment.StartsWith("?"))
                    {
                        string newpath = FolderPath + "/" + item.Value.segment;
                        var subs = GetFolderObjectRouteIds(newpath, ConstType, true);

                        if (subs != null && subs.Count > 0)
                        {
                            list.AddRange(subs);
                        }
                    }
                }
            }
            return list;
        }

        public void Delete(string FolderFullPath, byte ConstType, Guid UserId=default(Guid))
        {
            var modeltype = Service.ConstTypeService.GetModelType(ConstType);
            var repo = this.SiteDb.GetRepository(modeltype);

            var allroutes = GetFolderObjectRouteIds(FolderFullPath, ConstType, true);

            foreach (var item in allroutes)
            {
                var route = this.SiteDb.Routes.Get(item);
                if (route != null)
                {
                    if (route.DestinationConstType == ConstType && route.objectId != default(Guid))
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

            var allfolders = GetDescendantFolders(FolderFullPath, ConstType);

            foreach (var item in allfolders)
            {
                SiteDb.Folders.Delete(item.FullPath, ConstType, UserId);
            }

            var selfid = Kooboo.Data.IDGenerator.GetFolderId(FolderFullPath, ConstType);
            SiteDb.Folders.Delete(selfid, UserId);
        }
         
    }
}
