//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.

using System.Linq;
using Kooboo.Api;
using Kooboo.Data.Permission;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Service;
using Kooboo.Sites.Storage;
using Kooboo.Sites.Storage.Kooboo;
using Kooboo.Web.ViewModel;

namespace Kooboo.Web.Api.Implementation
{
    public class MediaApi : StorageBase, IApi
    {
        public string ModelName => "media";

        public bool RequireSite => true;

        public bool RequireUser => true;

        [Kooboo.Attributes.RequireParameters("path", "name")]
        [Permission(Feature.MEDIA_LIBRARY, Action = Data.Permission.Action.EDIT)]
        public StorageFolderModel CreateFolder(ApiCall call)
        {
            string name = call.GetValue("name");
            string path = call.GetValue("path");
            if (!path.EndsWith("/"))
            {
                path = path + "/";
            }

            string fullpath = UrlHelper.Combine(path, name);
            if (!fullpath.StartsWith("/"))
            {
                fullpath = "/" + fullpath;
            }

            if (fullpath.EndsWith("/"))
            {
                fullpath = fullpath.TrimEnd('/');
            }

            var storage = GetStorageProvider(call);

            if (storage is KoobooStorageProvider)
            {
                fullpath = KoobooStorageProvider.RemoveCustomSettingPrefix(call.WebSite, fullpath);
                fullpath = KoobooStorageProvider.HandleCustomSettingPrefix(call.WebSite, fullpath);
            }

            return storage.CreateMediaFolder(fullpath);
        }

        [Permission(Feature.MEDIA_LIBRARY, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.PAGES, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.CONTENT, Action = Data.Permission.Action.EDIT)]
        public MediaLibraryViewModel List(ApiCall call)
        {
            string path = call.GetValue("path", "fullpath");
            if (string.IsNullOrEmpty(path) || path == "\\")
            {
                path = "/";
            }

            return new MediaLibraryViewModel
            {
                Folders = GetFolders(call.WebSite.SiteDb(), path),
                Files = GetFiles(call.WebSite.SiteDb(), path),
                CrumbPath = PathService.GetCrumbPath(path)
            };
        }

        [Permission(Feature.MEDIA_LIBRARY, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.PAGES, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.CONTENT, Action = Data.Permission.Action.EDIT)]
        public MediaLibraryViewModel ListBy(ApiCall call)
        {
            string by = call.GetValue("by");
            if (string.IsNullOrEmpty(by))
            {
                return null;
            }

            var lower = by.ToLower();
            if (lower == "page" || lower == "view" || lower == "layout" || lower == "textcontent" || lower == "style" ||
                lower == "htmlblock")
            {
                return new MediaLibraryViewModel
                {
                    Files = GetFilesBy(call.WebSite.SiteDb(), by),
                    CrumbPath = PathService.GetCrumbPath("/")
                };
            }

            return null;
        }

        [Permission(Feature.MEDIA_LIBRARY, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.PAGES, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.CONTENT, Action = Data.Permission.Action.EDIT)]
        public MediaPagedViewModel PagedListBy(ApiCall call)
        {
            string by = call.GetValue("by");
            if (string.IsNullOrEmpty(by))
            {
                return null;
            }

            var lower = by.ToLower();
            if (lower != "page" && lower != "view" && lower != "layout" && lower != "textcontent" && lower != "style" &&
                lower != "htmlblock") return null;
            int pageSize = ApiHelper.GetPageSize(call, 50);
            int pageNumber = ApiHelper.GetPageNr(call);

            MediaPagedViewModel model = new MediaPagedViewModel
            {
                Files = GetPagedFilesBy(call.WebSite.SiteDb(), by, pageSize, pageNumber),
                CrumbPath = PathService.GetCrumbPath("/")
            };

            return model;
        }

        [Permission(Feature.MEDIA_LIBRARY, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.PAGES, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.CONTENT, Action = Data.Permission.Action.EDIT)]
        public MediaPagedViewModel PagedList(ApiCall call)
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

            if (storage is KoobooStorageProvider)
            {
                path = KoobooStorageProvider.RemoveCustomSettingPrefix(call.WebSite, path);
            }

            var isSearch = !string.IsNullOrWhiteSpace(request.Keyword);
            var response = new MediaPagedViewModel
            {
                Providers = StorageProviderFactory.GetEnabledProviders(call.Context),
                CrumbPath = isSearch ? CrumbPath.SearchResultCrumbs : PathService.GetCrumbPath(path)
            };
            try
            {
                if (storage is KoobooStorageProvider)
                {
                    if (request.Path == "/")
                    {
                        request.Path = KoobooStorageProvider.HandleCustomSettingPrefix(call.WebSite, request.Path);
                    }
                }
                var data = storage.GetMediaObjects(request);
                response.Folders = data.Folders;

                response.Files = new PagedListViewModel<MediaStorageFileModel>
                {
                    Infinite = data.Infinite,
                    List = data.Files.DataList.ToList(),
                    TotalPages = data.Files.TotalPages,
                    PageNr = data.Files.PageNr,
                    TotalCount = data.Files.TotalCount,
                    PageSize = data.Files.PageSize,
                };
            }
            catch (Exception ex)
            {
                response.ErrorMessage = "<br/>" + ex.Message;
            }

            return response;
        }

        private List<ImageFolderViewModel> GetFolders(SiteDb siteDb, string path)
        {
            var SubFolders = siteDb.Folders.GetSubFolders(path, ConstObjectType.Image);

            return SubFolders.Select(x => new ImageFolderViewModel()
            {
                Name = x.Segment,
                FullPath = x.FullPath,
                Count = siteDb.Folders.GetFolderObjects<Image>(x.FullPath, true, false).Count +
                        siteDb.Folders.GetSubFolders(x.FullPath, ConstObjectType.Image).Count,
                LastModified = PathService.GetLastModified(siteDb, x.FullPath, ConstObjectType.Image)
            }).OrderByDescending(x => x.LastModified).ToList();
        }

        private List<MediaStorageFileModel> GetFiles(SiteDb siteDb, string path)
        {
            string baseurl = siteDb.WebSite.BaseUrl();

            List<Image> images = siteDb.Folders.GetFolderObjects<Image>(path, true);

            return MediaFileViewModels(siteDb, images, baseurl).ToList();
        }

        private List<MediaStorageFileModel> GetFilesBy(SiteDb siteDb, string by)
        {
            string baseurl = siteDb.WebSite.BaseUrl();
            // by = View, Page, Layout, TextContent, Style. 
            byte consttype = ConstTypeContainer.GetConstType(by);

            var images = siteDb.Images.ListUsedBy(consttype);

            return MediaFileViewModels(siteDb, images, baseurl).ToList();
        }


        private PagedListViewModel<MediaStorageFileModel> GetPagedFilesBy(SiteDb siteDb, string by, int PageSize,
            int PageNumber)
        {
            string baseurl = siteDb.WebSite.BaseUrl();
            // by = View, Page, Layout, TextContent, Style. 
            byte consttype = ConstTypeContainer.GetConstType(by);

            var images = siteDb.Images.ListUsedBy(consttype);

            int totalskip = 0;
            if (PageNumber > 1)
            {
                totalskip = (PageNumber - 1) * PageSize;
            }

            PagedListViewModel<MediaStorageFileModel> Result = new PagedListViewModel<MediaStorageFileModel>();

            Result.TotalCount = images.Count();
            Result.TotalPages = ApiHelper.GetPageCount(Result.TotalCount, PageSize);
            Result.PageSize = PageSize;
            Result.PageNr = PageNumber;

            Result.List = MediaFileViewModels(siteDb, images, baseurl).Skip(totalskip).Take(PageSize).ToList();
            return Result;
        }

        [Permission(Feature.MEDIA_LIBRARY, Action = Data.Permission.Action.DELETE)]
        public void DeleteFolders(ApiCall call)
        {
            string jsonvalue = call.Context.Request.Body;

            var folders = JsonHelper.Deserialize<string[]>(jsonvalue);

            var storage = GetStorageProvider(call);
            storage.DeleteMediaFolders(folders);
        }

        [Permission(Feature.MEDIA_LIBRARY, Action = Data.Permission.Action.DELETE)]
        public void DeleteImages(ApiCall call)
        {
            string jsonvalue = call.Context.Request.Body;

            var keys = JsonHelper.Deserialize<string[]>(jsonvalue);
            var storage = GetStorageProvider(call);
            storage.DeleteMediaObjects(keys);
        }

        [Kooboo.Attributes.RequireParameters("id")]
        [Permission(Feature.MEDIA_LIBRARY, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.PAGES, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.CONTENT, Action = Data.Permission.Action.VIEW)]
        public ImageEditModel Get(ApiCall call)
        {
            var storage = GetStorageProvider(call);
            var id = call.GetValue("id");
            return storage.GetMediaFile(id);
        }

        [Kooboo.Attributes.RequireParameters("id")]
        [Permission(Feature.MEDIA_LIBRARY, Action = Data.Permission.Action.EDIT)]
        public bool ImageUpdate(ApiCall call)
        {
            string alt = call.GetValue("alt");
            string base64 = call.GetValue("base64");
            var id = call.GetValue("id");
            var storage = GetStorageProvider(call);
            var metas = new Dictionary<string, string>
            {
                ["alt"] = alt,
                ["name"] = call.GetValue("name"),
            };
            return storage.UpdateImage(id, base64, metas);
        }

        [Permission(Feature.MEDIA_LIBRARY, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.PAGES, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.CONTENT, Action = Data.Permission.Action.VIEW)]
        public string ContentImage(ApiCall call)
        {
            var files = call.Context.Request.Files;

            if (files == null || files.Count() == 0)
            {
                return null;
            }

            foreach (var f in files)
            {
                string filename = f.FileName;

                string url = GetUploadUrl(call, filename);

                var siteobject = call.WebSite.SiteDb().Images.UploadImage(f.Bytes, url, call.Context.User.Id);
                return url;
            }

            return null;
        }

        private string GetUploadUrl(ApiCall call, string filename)
        {
            var sitedb = call.WebSite.SiteDb();
            if (string.IsNullOrEmpty(filename))
            {
                return null;
            }

            string fullname = filename.Replace("\\", "/");
            if (fullname.StartsWith("/"))
            {
                fullname = fullname.Substring(1);
            }

            string checkurl = "/" + fullname;

            var image = sitedb.Images.GetByUrl(checkurl);
            if (image == null)
            {
                return checkurl;
            }

            for (int i = 0; i < 999; i++)
            {
                checkurl = "/" + i.ToString() + filename;
                image = sitedb.Images.GetByUrl(checkurl);
                if (image == null)
                {
                    return checkurl;
                }
            }

            return null;
        }

        private static IEnumerable<MediaStorageFileModel> MediaFileViewModels(SiteDb siteDb, IEnumerable<Image> images,
            string baseurl)
        {
            return images.Select(x =>
            {
                var usedby = siteDb.Images.GetUsedByForCount(x.Id);
                var imageUrl = ObjectService.GetObjectRelativeUrl(siteDb, x) + $"?version={x.Version}";
                return new MediaStorageFileModel()
                {
                    Id = x.Id,
                    Key = x.Id.ToString(),
                    Height = x.Height,
                    Alt = x.Alt,
                    Width = x.Width,
                    Size = x.Size,
                    Name = x.Name,
                    LastModified = x.LastModified,
                    Thumbnail = ThumbnailService.GenerateThumbnailUrl(x.Id, 90, 0, siteDb.Id, x.Version),
                    Url = ObjectService.GetObjectRelativeUrl(siteDb, x),
                    IsImage = true,
                    PreviewUrl = UrlHelper.Combine(baseurl, imageUrl),
                    References = usedby?.GroupBy(it => it.ModelType).ToDictionary(
                        key => key.Key.Name, value => value.Count())
                };
            }).OrderByDescending(x => x.LastModified.ToLocalTime());
        }


    }
}