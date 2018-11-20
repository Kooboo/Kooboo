//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Template;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Sync;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Kooboo.Web.ViewModel;
using Kooboo.Api;
using Kooboo.Sites.Routing;
using Kooboo.Data;
using Kooboo.Lib.Helper;
using System.Threading;
using Kooboo.Sites.TaskQueue.Model;

namespace Kooboo.Web.Api.Implementation
{
    public    class TemplateApi : IApi
    {
        public string ModelName
        {
            get
            {
                return "Template";
            }
        }

        public bool RequireSite
        {
            get
            {
                return false;
            }
        }

        public bool RequireUser
        {
            get
            {
                return false;  // this needs to turn back...
            }
        }

    
        public PagedListViewModel<TemplateItemViewModel> List(ApiCall call)
        {
            int pagenr = ApiHelper.GetPageNr(call);
            int pagesize = ApiHelper.GetPageSize(call);

            Dictionary<string, string> paras = new Dictionary<string, string>();
            if (pagenr != 0)
            {
                paras.Add("PageNr", pagenr.ToString());
            }
            if (pagesize != 0)
            {
                paras.Add("PageSize", pagesize.ToString());
            }

            string Url = UrlHelper.Combine(AppSettings.ThemeUrl, "/_api/template/List2");
            Url = UrlHelper.AppendQueryString(Url, paras);

            var pagedlist = HttpHelper.Get<PagedListViewModel<TemplateItemViewModel>>(Url);
            SetThumbnailUrl(pagedlist.List);


            return pagedlist;

        }

     
        public TemplateDetailViewModel Get(ApiCall call)
        {
            string Url = UrlHelper.Combine(AppSettings.ThemeUrl, "/_api/template/Get2");
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("id", call.ObjectId.ToString());
            var detail = HttpHelper.Get<TemplateDetailViewModel>(Url, para);

            detail.LastModified = DateTime.SpecifyKind(detail.LastModified, DateTimeKind.Utc);

            SetImageDownloadUrl(detail);

            return detail;
        }

   
        public PagedListViewModel<TemplateItemViewModel> Private(ApiCall call)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("OrganizationId", call.Context.User.CurrentOrgId.ToString());

            int pagenr = ApiHelper.GetPageNr(call);
            int pagesize = ApiHelper.GetPageSize(call);

            if (pagenr != 0)
            {
                para.Add("PageNr", pagenr.ToString());
            }
            if (pagesize != 0)
            {
                para.Add("PageSize", pagesize.ToString());
            }

            string Url = UrlHelper.Combine(Kooboo.Data.AppSettings.ThemeUrl, "/_api/template/Private2");

            Url = UrlHelper.AppendQueryString(Url, para);

            var pagedlist = HttpHelper.Get<PagedListViewModel<TemplateItemViewModel>>(Url, para);
            SetThumbnailUrl(pagedlist.List);
            return pagedlist;
        }

   
        public PagedListViewModel<TemplateItemViewModel> Personal(ApiCall call)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("Id", call.Context.User.Id.ToString());

            int pagenr = ApiHelper.GetPageNr(call);
            int pagesize = ApiHelper.GetPageSize(call);

            if (pagenr != 0)
            {
                para.Add("PageNr", pagenr.ToString());
            }
            if (pagesize != 0)
            {
                para.Add("PageSize", pagesize.ToString());
            }

            string Url = UrlHelper.Combine(Kooboo.Data.AppSettings.ThemeUrl, "/_api/template/Personal2");

            Url = UrlHelper.AppendQueryString(Url, para);
            var pagedlist = HttpHelper.Get<PagedListViewModel<TemplateItemViewModel>>(Url, para);
            SetThumbnailUrl(pagedlist.List);
            return pagedlist;
        }

   
        public PagedListViewModel<TemplateItemViewModel> Search(ApiCall call)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("Keyword", call.GetValue("keyword"));

            int pagenr = ApiHelper.GetPageNr(call);
            int pagesize = ApiHelper.GetPageSize(call);

            if (pagenr != 0)
            {
                para.Add("PageNr", pagenr.ToString());
            }
            if (pagesize != 0)
            {
                para.Add("PageSize", pagesize.ToString());
            }

            string Url = UrlHelper.Combine(Kooboo.Data.AppSettings.ThemeUrl, "/_api/template/Search2");
            Url = UrlHelper.AppendQueryString(Url, para);
            var pagedlist = HttpHelper.Get<PagedListViewModel<TemplateItemViewModel>>(Url, para);
            SetThumbnailUrl(pagedlist.List);
            return pagedlist;
        }

             
        protected string GetStartRelativeUrl(Data.Models.WebSite site)
        {
            var startpages = site.StartPages();
            if (startpages != null && startpages.Count() > 0)
            {
                foreach (var item in startpages)
                {
                    Route route = site.SiteDb().Routes.Query.Where(o => o.objectId == item.Id).FirstOrDefault();

                    if (route != null && !route.Name.Contains("{") && !route.Name.Contains("%"))
                    {
                        return route.Name;
                    }
                }
            }

            var allpages = site.SiteDb().Pages.All();

            if (allpages != null && allpages.Count() > 0)
            {
                foreach (var item in allpages)
                {
                    Route route = site.SiteDb().Routes.Query.Where(o => o.objectId == item.Id).FirstOrDefault();

                    if (route != null && !route.Name.Contains("{") && !route.Name.Contains("%"))
                    {
                        return route.Name;
                    }
                }
            }

            if (allpages != null && allpages.Count() > 0)
            {
                foreach (var item in allpages)
                {
                    Route route = site.SiteDb().Routes.Query.Where(o => o.objectId == item.Id).FirstOrDefault();

                    if (route != null)
                    {
                        return route.Name;
                    }
                }
            }

            return "/";
        }


        protected void SetThumbnailUrl(List<TemplateItemViewModel> items)
        {
            if (items == null)
            {
                return;
            }

            string imgbase = UrlHelper.Combine(AppSettings.ThemeUrl, "/_api/download/themeimg/");
            foreach (var item in items)
            {
                if (!string.IsNullOrEmpty(item.ThumbNail) && !item.ThumbNail.ToLower().StartsWith("http://"))
                {
                    item.ThumbNail = UrlHelper.Combine(imgbase, item.ThumbNail);
                }
                item.ThumbNail += "?width=200";
            }
        }

        protected void SetImageDownloadUrl(TemplateDetailViewModel detail)
        {
            if (detail == null)
            {
                return;
            }
            string imgbase = UrlHelper.Combine(AppSettings.ThemeUrl, "/_api/download/themeimg/");

            int count = detail.Images.Count();
            for (int i = 0; i < count; i++)
            {
                var current = detail.Images[i];
                var newurl = UrlHelper.Combine(imgbase, current);
                detail.Images[i] = newurl;
            }
        }

        protected static bool ContainSeach(string input, string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return true;
            }

            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            return input.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) > -1;
        }

        protected SiteDb GetSiteDb(ApiCall call)
        {
            var siteDb = call.WebSite.SiteDb();
            if (siteDb == null)
            {
                Guid SiteId = call.GetGuidValue("SiteId");

                var website = Kooboo.Data.GlobalDb.WebSites.Get(SiteId);
                siteDb = website != null ? website.SiteDb() : null;
            }
            return siteDb;
        }

        protected TemplateDataModel InitData(Kooboo.Lib.NETMultiplePart.FormResult formResult, ApiCall call)
        {
            TemplateDataModel data = new TemplateDataModel();

            if (formResult.FormData.ContainsKey("sitename"))
            {
                data.Name = formResult.FormData["sitename"];
            }

            if (formResult.FormData.ContainsKey("link"))
            {
                data.Link = formResult.FormData["link"];
            }
            if (formResult.FormData.ContainsKey("description"))
            {
                data.Description = formResult.FormData["description"];
            }
            if (formResult.FormData.ContainsKey("tags"))
            {
                data.Tags = formResult.FormData["tags"];
            }

            if (formResult.FormData.ContainsKey("price"))
            {
                data.Price=decimal.Parse(formResult.FormData["price"]);
            }
            if (formResult.FormData.ContainsKey("currency"))
            {
                data.Price = decimal.Parse(formResult.FormData["currency"]);
            }

            data.UserId = call.Context.User.Id;

            if (formResult.FormData.ContainsKey("IsPrivate"))
            {
                string strisprivae = formResult.FormData["IsPrivate"];
                bool isprivate = false;
                bool.TryParse(strisprivae, out isprivate);

                if (isprivate)
                {
                    data.OrganizationId = call.Context.User.CurrentOrgId;
                }
            }

            foreach (var item in formResult.Files)
            {
                TemplateUserImages image = new TemplateUserImages();
                image.FileName = item.FileName;
                image.Base64 = Convert.ToBase64String(item.Bytes);
                data.Images.Add(image);
            }

            if (data.Images.Count() > 0)
            {
                if (formResult.FormData.ContainsKey("defaultimg"))
                {
                    string strindex = formResult.FormData["defaultimg"];
                    int index = 0;
                    int.TryParse(strindex, out index);

                    if (data.Images.Count() > index)
                    {
                        data.Images[index].IsDefault = true;
                    }
                }
            }

            return data;
        }

        public virtual void Share(ApiCall call)
        {
            SiteDb siteDb = call.WebSite != null ? call.WebSite.SiteDb() : null;
            if (siteDb == null)
            { return; }

            var tempFolder = Kooboo.Data.AppSettings.TempDataPath;

            var exportfile = ImportExport.ExportInter(siteDb);
            if (!File.Exists(exportfile))
            {
                return;
            }

            var formreader = Kooboo.Lib.NETMultiplePart.FormReader.ReadForm(call.Context.Request.PostData);

            var postdata = InitData(formreader, call);

            var zipbytes = IOHelper.ReadAllBytes(exportfile);
            postdata.Bytes = zipbytes;

            if (zipbytes.Length > AppSettings.MaxTemplateSize)
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("Exceed max template size", call.Context));
            }

            postdata.ByteHash = Kooboo.Lib.Security.Hash.ComputeGuid(zipbytes);

            Kooboo.IndexedDB.Serializer.Simple.SimpleConverter<TemplateDataModel> converter = new IndexedDB.Serializer.Simple.SimpleConverter<TemplateDataModel>();

            var postbytes = converter.ToBytes(postdata);

            string url = UrlHelper.Combine(AppSettings.ThemeUrl, "/_api/receiver/template");

            var response = HttpHelper.PostData(url, new Dictionary<string, string>(), postbytes);

            if (!response)
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("Share template failed", call.Context));
            }
        }

        public virtual void ShareBatch(ApiCall call)
        {
            SiteDb siteDb = call.WebSite != null ? call.WebSite.SiteDb() : null;

            var formreader = Kooboo.Lib.NETMultiplePart.FormReader.ReadForm(call.Context.Request.PostData);
            if (siteDb == null)
            {
                Guid siteId;
                if (!Guid.TryParse(formreader.FormData["SiteId"], out siteId))
                {
                    return;
                }
                var website = Kooboo.Data.GlobalDb.WebSites.Get(siteId);
                siteDb = website.SiteDb();
            }

            if (siteDb == null)
            {
                return;
            }

            var tempFolder = Kooboo.Data.AppSettings.TempDataPath;

            var exportfile = ImportExport.ExportInter(siteDb);
            if (!File.Exists(exportfile))
            {
                return;
            }


            var postdata = InitData(formreader, call);

            var zipbytes = IOHelper.ReadAllBytes(exportfile);
            postdata.Bytes = zipbytes;

            if (zipbytes.Length > AppSettings.MaxTemplateSize)
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("Exceed max template size", call.Context));
            }

            postdata.ByteHash = Kooboo.Lib.Security.Hash.ComputeGuid(zipbytes);

            Kooboo.IndexedDB.Serializer.Simple.SimpleConverter<TemplateDataModel> converter = new IndexedDB.Serializer.Simple.SimpleConverter<TemplateDataModel>();

            var postbytes = converter.ToBytes(postdata);

            string url = UrlHelper.Combine(AppSettings.ThemeUrl, "/_api/receiver/template");

            var response = HttpHelper.PostData(url, new Dictionary<string, string>(), postbytes);

            if (!response)
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("Share template failed", call.Context));
            }
        }
         
        public void Update(ApiCall call)
        {
            var formResult = Kooboo.Lib.NETMultiplePart.FormReader.ReadForm(call.Context.Request.PostData);

            TemplateUpdateModel update = new TemplateUpdateModel();

            update.UserId = call.Context.User.Id;

            if (formResult.FormData.ContainsKey("id"))
            {
                string strid = formResult.FormData["id"];
                Guid id;
                if (System.Guid.TryParse(strid, out id))
                {
                    update.Id = id;
                }
                else
                {
                    throw new Exception(Data.Language.Hardcoded.GetValue("Invalid package id", call.Context));
                }
            }
            else
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("Missing package id", call.Context));
            }

            if (formResult.FormData.ContainsKey("category"))
            {
                update.Category = formResult.FormData["category"];
            }
            if (formResult.FormData.ContainsKey("link"))
            {
                update.Link = formResult.FormData["link"];
            }
            if (formResult.FormData.ContainsKey("description"))
            {
                update.Description = formResult.FormData["description"];
            }
            if (formResult.FormData.ContainsKey("tags"))
            {
                update.Tags = formResult.FormData["tags"];
            }
            if (formResult.FormData.ContainsKey("Images"))
            {
                update.Images = formResult.FormData["Images"];
            }

            if (formResult.FormData.ContainsKey("IsPrivate"))
            {
                string strisprivae = formResult.FormData["IsPrivate"];
                bool isprivate = false;
                bool.TryParse(strisprivae, out isprivate);

                if (isprivate)
                {
                    update.OrganizationId = call.Context.User.CurrentOrgId;
                }
                else
                {
                    update.OrganizationId = default(Guid); 
                }
            }

            foreach (var item in formResult.Files)
            {
                string contenttype = item.ContentType;
                if (contenttype == null) { contenttype = "image"; }
                else { contenttype = contenttype.ToLower(); }

                if (contenttype.Contains("image"))
                {
                    TemplateUserImages image = new TemplateUserImages();
                    image.FileName = item.FileName;
                    image.Base64 = Convert.ToBase64String(item.Bytes);
                    update.NewImages.Add(image);
                }
                else if (contenttype.Contains("zip"))
                {
                    update.Bytes = item.Bytes;
                } 
            }

            if (formResult.FormData.ContainsKey("defaultimg"))
            {
                string defaultimage = formResult.FormData["defaultimg"];
                update.NewDefault = defaultimage;
            }

            if (formResult.FormData.ContainsKey("thumbnail"))
            {
                string defaultimage = formResult.FormData["thumbnail"];
                update.NewDefault = defaultimage;
            }

            if (update.NewImages.Count() > 0)
            {
                if (formResult.FormData.ContainsKey("defaultfile"))
                {
                    string defaultimg = formResult.FormData["defaultfile"];
                    int index = 0;
                    if (int.TryParse(defaultimg, out index))
                    {
                        if (update.NewImages.Count() > index)
                        {
                            update.NewImages[index].IsDefault = true;
                        }
                    }
                }
            }

            IndexedDB.Serializer.Simple.SimpleConverter<TemplateUpdateModel> converter = new IndexedDB.Serializer.Simple.SimpleConverter<TemplateUpdateModel>();

            var allbytes = converter.ToBytes(update);

            string Url = Kooboo.Lib.Helper.UrlHelper.Combine(Kooboo.Data.AppSettings.ThemeUrl, "/_api/receiver/updatetemplate");

            var response = HttpHelper.PostData(Url, null, allbytes);

            if (!response)
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("Update template failed", call.Context));
            }
        }
          
        public void Delete(ApiCall call)
        {
            var packageid = call.ObjectId;
            var userid = call.Context.User.Id;
            string Url = Kooboo.Lib.Helper.UrlHelper.Combine(Kooboo.Data.AppSettings.ThemeUrl, "/_api/template/Delete2");
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("userid", userid.ToString());
            para.Add("packageid", packageid.ToString());

            var ok = HttpHelper.Post<bool>(Url, para);
            return;
        }

        [Kooboo.Attributes.RequireParameters("SiteName", "RootDomain", "SubDomain", "DownloadCode")]
        public Guid Use(ApiCall call)
        {
            string SiteName = call.GetValue("SiteName");
            if (!Data.GlobalDb.WebSites.CheckNameAvailable(SiteName, call.Context.User.CurrentOrgId))
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("SiteName is taken", call.Context));
            }

            string RootDomain = call.GetValue("RootDomain");
            string SubDomain = call.GetValue("SubDomain");

            string FullDomain = RootDomain;
            if (!string.IsNullOrEmpty(SubDomain))
            {
                if (FullDomain.StartsWith("."))
                {
                    FullDomain = SubDomain + FullDomain;
                }
                else
                {
                    FullDomain = SubDomain + "." + FullDomain;
                }
            }

            string downloadcode = call.GetValue("DownloadCode");

            string url = UrlHelper.Combine(AppSettings.ThemeUrl, "/_api/download/package/" + downloadcode);

            if (call.Context.User != null)
            {
                url += "?userid=" + call.Context.User.Id.ToString();
            }

            var download = DownloadHelper.DownloadFile(url, "zip");
            if (download == null)
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("template package not found", call.Context));
            }

            MemoryStream memory = new MemoryStream(download);
            var newsite = ImportExport.ImportZip(memory, call.Context.User.CurrentOrgId, SiteName, FullDomain, call.Context.User.Id);
            return newsite.Id;
        }
          
    } 
}
