//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Web.ViewModel;
using Kooboo.Lib.Helper;
using Kooboo.Api;
using Kooboo.Sites.Extensions;
using System.IO;

namespace Kooboo.Web.Api
{
    public class UploadApi : IApi
    {
        public string ModelName
        {
            get
            {
                return "upload";
            }
        }

        public bool RequireSite
        {
            get
            {
                return true;
            }
        }

        public bool RequireUser
        {
            get
            {
                return true;
            }
        }


        public List<IEmbeddableItemListViewModel> Style(ApiCall call)
        {
            var files = Kooboo.Lib.NETMultiplePart.FormReader.ReadFile(call.Context.Request.PostData);
            List<IEmbeddableItemListViewModel> result = new List<IEmbeddableItemListViewModel>();

            foreach (var f in files)
            {
                var siteobject = call.WebSite.SiteDb().Styles.Upload(f.Bytes, f.FileName, call.Context.User.Id);
                result.Add(new IEmbeddableItemListViewModel(call.WebSite.SiteDb(), siteobject));
            }
            return result;
        }

        public List<IEmbeddableItemListViewModel> Script(ApiCall call)
        {
            var files = Kooboo.Lib.NETMultiplePart.FormReader.ReadFile(call.Context.Request.PostData);

            List<IEmbeddableItemListViewModel> result = new List<IEmbeddableItemListViewModel>();

            foreach (var f in files)
            {

                var siteobject = call.WebSite.SiteDb().Scripts.Upload(f.Bytes, f.FileName, call.Context.User.Id);
                result.Add(new IEmbeddableItemListViewModel(call.WebSite.SiteDb(), siteobject));
            }
            return result;
        }


        public bool Image(ApiCall call)
        {
            var result = Kooboo.Lib.NETMultiplePart.FormReader.ReadForm(call.Context.Request.PostData);

            string Folder =result.FormData.ContainsKey("Folder")? result.FormData["Folder"]:"";

            if (!Folder.EndsWith("/") && !Folder.EndsWith("\\"))
            {
                Folder = Folder + "/";
            }

            foreach (var item in result.Files)
            {
                string filename = StringHelper.ToValidFileName(item.FileName);

                filename =  UrlHelper.Combine(Folder, filename);

                call.WebSite.SiteDb().Images.UploadImage(item.Bytes, filename, call.Context.User.Id);
            }

            return true;
        }

        public bool File(ApiCall call)
        {
            var result = Kooboo.Lib.NETMultiplePart.FormReader.ReadForm(call.Context.Request.PostData);

            string Folder = result.FormData["Folder"];

            if (!Folder.EndsWith("/") && !Folder.EndsWith("\\"))
            {
                Folder = Folder + "/";
            }

            foreach (var item in result.Files)
            {
                string filename = StringHelper.ToValidFileName(item.FileName);

                filename = UrlHelper.Combine(Folder, filename);
                 
                call.WebSite.SiteDb().Files.Upload(item.Bytes, filename, call.Context.User.Id);
            }

            return true;
        }

        public bool Package(ApiCall call)
        {
            var files = Kooboo.Lib.NETMultiplePart.FormReader.ReadFile(call.Context.Request.PostData);

            if (files != null && files.Count > 0)
            {
                foreach (var f in files)
                {
                    var bytes = f.Bytes;
                    string filename = f.FileName;

                    string extension = System.IO.Path.GetExtension(filename);
                    if (!string.IsNullOrEmpty(extension))
                    {
                        extension = extension.ToLower();
                    }
                    
                   if (extension == ".zip" || extension == ".rar")
                    {
                        MemoryStream memory = new MemoryStream(bytes);
                    

                        Kooboo.Sites.Sync.ImportExport.ImportZip(memory, call.WebSite); 
                    } 
                 
                }
            }

            return true; 
        } 
    }
}
