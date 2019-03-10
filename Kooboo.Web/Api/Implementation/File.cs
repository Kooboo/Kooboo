//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Data.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Relation;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Service;
using Kooboo.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Kooboo.Web.Api.Implementation
{
   public class FileApi : IApi
    {

        public string ModelName
        {
            get
            {
                return "file";
            }
        }

        public bool RequireSite { get { return true; } }

        public bool RequireUser { get { return true; } }

        [Kooboo.Attributes.RequireParameters("path", "name")]
        public FileFolderViewModel CreateFolder(ApiCall call)
        {
            var sitedb = call.Context.WebSite.SiteDb();

            string name = call.GetValue("name");
            string path = call.GetValue("path");
            if (!path.EndsWith("/"))
            {
                path = path + "/";
            }

            string fullpath = Lib.Helper.UrlHelper.Combine(path, name);
            if (!fullpath.StartsWith("/"))
            {
                fullpath = "/" + fullpath;
            }
            if (fullpath.EndsWith("/"))
            {
                fullpath = fullpath.TrimEnd('/');
            }

            Folder folder = new Folder(ConstObjectType.CmsFile);
            folder.FullPath = fullpath;

            sitedb.Folders.AddOrUpdate(folder, call.Context.User.Id);

            return new FileFolderViewModel
            {
                Id = folder.FullPath,
                Name = folder.Segment,
                FullPath = folder.FullPath,
                LastModified = PathService.GetLastModified(sitedb, folder.FullPath, ConstObjectType.CmsFile),
                Count = sitedb.Folders.GetFolderObjects<CmsFile>(folder.FullPath, true, false).Count +
                sitedb.Folders.GetSubFolders(folder.FullPath, ConstObjectType.CmsFile).Count
            };
        }

        public FileOverViewModel List(ApiCall call)
        {
            string path = call.GetValue("path", "fullpath");
            if (string.IsNullOrEmpty(path) || path == "\\")
            {
                path = "/";
            }

            FileOverViewModel model = new FileOverViewModel(); 

            model.Folders = GetFolders(call.WebSite.SiteDb(), path);

            model.Files = GetFiles(call.WebSite.SiteDb(), path);

            model.CrumbPath = PathService.GetCrumbPath(path);

            return model;
        } 

        private List<FileFolderViewModel> GetFolders(SiteDb siteDb, string path)
        {
            var SubFolders = siteDb.Folders.GetSubFolders(path, ConstObjectType.CmsFile);

            List<FileFolderViewModel> Result = new List<FileFolderViewModel>();

            foreach (var item in SubFolders)
            {
                FileFolderViewModel model = new FileFolderViewModel();
                // model.Id = path; 
                model.Name = item.Segment;
                model.FullPath = item.FullPath;
                model.Count = siteDb.Folders.GetFolderObjects<CmsFile>(item.FullPath, true, false).Count +
                                siteDb.Folders.GetSubFolders(item.FullPath, ConstObjectType.CmsFile).Count;
                model.LastModified = PathService.GetLastModified(siteDb, item.FullPath, ConstObjectType.CmsFile);

                Result.Add(model);
            }

            return Result;
        }

        private List<FileItemViewModel> GetFiles(SiteDb siteDb, string path)
        { 
            string baseurl = siteDb.WebSite.BaseUrl(); 

            List<CmsFile> files = siteDb.Folders.GetFolderObjects<CmsFile>(path, true);

            List<FileItemViewModel> Result = new List<FileItemViewModel>();

            foreach (var item in files)
            {
                FileItemViewModel model = new FileItemViewModel();
                model.Id = item.Id; 
                model.Size = item.Size;
                model.Name = item.Name;
                model.LastModified = item.LastModified; 
                model.Url = ObjectService.GetObjectRelativeUrl(siteDb, item);
                model.PreviewUrl = Lib.Helper.UrlHelper.Combine(baseurl, model.Url);

                model.Relations = Sites.Helper.RelationHelper.Sum(siteDb.Files.GetUsedBy(item.Id)); 
                  
                Result.Add(model);
            }
            return Result;
        }
         
        [Kooboo.Attributes.RequireModel(typeof(List<string>))]
        public void DeleteFolders(ApiCall call)
        { 
            List<string> FolderFullPaths = call.Context.Request.Model as List<string>;   
            foreach (var item in FolderFullPaths)
            {
                call.WebSite.SiteDb().Folders.Delete(item, ConstObjectType.CmsFile);
            }
        }

        [Kooboo.Attributes.RequireModel(typeof(List<Guid>))]
        public void DeleteFiles(ApiCall call)
        {  
            List<Guid> FileIds = call.Context.Request.Model as List<Guid>;
            foreach (var item in FileIds)
            {
                call.WebSite.SiteDb().Files.Delete(item);
            }
        }
    }
}
