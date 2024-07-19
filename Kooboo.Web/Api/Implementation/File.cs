//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.

using System.Linq;
using Kooboo.Api;
using Kooboo.Data.Permission;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Models;
using Kooboo.Sites.Service;
using Kooboo.Sites.Storage;
using Kooboo.Web.ViewModel;

namespace Kooboo.Web.Api.Implementation
{
    public class FileApi : IApi
    {
        public string ModelName
        {
            get { return "file"; }
        }

        public bool RequireSite
        {
            get { return true; }
        }

        public bool RequireUser
        {
            get { return true; }
        }

        [Kooboo.Attributes.RequireParameters("path", "name")]
        [Permission(Feature.FILE, Action = Data.Permission.Action.EDIT)]
        public StorageFolderModel CreateFolder(ApiCall call)
        {
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

            var storage = GetStorageProvider(call);
            return storage.CreateCmsFileFolder(fullpath);
        }

        [Permission(Feature.FILE, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.CONTENT, Action = Data.Permission.Action.EDIT)]
        public FileOverViewModel List(ApiCall call)
        {
            string path = call.GetValue("path", "fullpath");
            if (string.IsNullOrEmpty(path) || path == "\\")
            {
                path = "/";
            }

            var storage = GetStorageProvider(call);

            var request = new GetObjectsRequest
            {
                Path = path,
                PageSize = ApiHelper.GetPageSize(call, 50),
                Keyword = call.GetValue("keyword"),
                Page = ApiHelper.GetPageNr(call),
                StartAfter = call.GetValue("startAfter"),
            };

            var isSearch = !string.IsNullOrWhiteSpace(request.Keyword);

            var response = new FileOverViewModel
            {
                Providers = StorageProviderFactory.GetEnabledProviders(call.Context),
                CrumbPath = isSearch ? CrumbPath.SearchResultCrumbs : PathService.GetCrumbPath(path)
            };
            try
            {
                var result = storage.GetCmsFileObjects(request);
                response.Folders = result.Folders;
                response.Files = new PagedListViewModel<StorageFileModel>
                {
                    Infinite = result.Infinite,
                    List = result.Files.DataList.ToList(),
                    TotalPages = result.Files.TotalPages,
                    PageNr = result.Files.PageNr,
                    TotalCount = result.Files.TotalCount,
                    PageSize = result.Files.PageSize,
                };
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
            }

            return response;
        }

        [Kooboo.Attributes.RequireModel(typeof(List<string>))]
        [Permission(Feature.FILE, Action = Data.Permission.Action.DELETE)]
        public void DeleteFolders(ApiCall call)
        {
            List<string> folders = call.Context.Request.Model as List<string>;
            var storage = GetStorageProvider(call);
            storage.DeleteCmsFileFolders(folders.ToArray());
        }

        [Kooboo.Attributes.RequireModel(typeof(List<string>))]
        [Permission(Feature.FILE, Action = Data.Permission.Action.DELETE)]
        public void DeleteFiles(ApiCall call)
        {
            string jsonvalue = call.Context.Request.Body;

            var keys = JsonHelper.Deserialize<string[]>(jsonvalue);
            var storage = GetStorageProvider(call);
            storage.DeleteCmsFileObjects(keys);
        }

        private IStorageProvider GetStorageProvider(ApiCall call)
        {
            var provider = call.GetValue("provider");
            return StorageProviderFactory.GetProvider(provider, call.Context);
        }
    }
}