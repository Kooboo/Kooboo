//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq; 

namespace Kooboo.Web.Api.Implementation
{
    public class ContentFolderApi : SiteObjectApi<ContentFolder>
    {

        public override List<object> List(ApiCall call)
        {
            
            var sitedb = call.Context.WebSite.SiteDb(); 

            List<ContentFolderViewModel> result = new List<ContentFolderViewModel>();

            var all = sitedb.ContentFolders.All();

            foreach (var item in all)
            {
                ContentFolderViewModel model = ContentFolderViewModel.Create(item); 
                 
                model.Relations = Sites.Helper.RelationHelper.Sum(sitedb.ContentFolders.GetUsedBy(item.Id));

                result.Add(model); 
            }

            return result.ToList<object>(); 
        }


        [Kooboo.Attributes.RequireParameters("id")]
        public List<ColumnViewModel> Columns(ApiCall apicall)
        {
            Guid FolderId = apicall.ObjectId;
            var folder = apicall.WebSite.SiteDb().ContentFolders.Get(FolderId);
            if (folder == null)
            {
                return null;
            } 
            var columns = apicall.WebSite.SiteDb().ContentTypes.GetColumns(folder.ContentTypeId);

            List<ColumnViewModel> result = new List<ColumnViewModel>();

            foreach (var item in columns)
            {
                if (item.Name.ToLower() != "online")
                { 
                    ColumnViewModel model = new ColumnViewModel();
                    model.Name = item.Name;
                    model.DisplayName = item.DisplayName;
                    model.DataType = item.DataType;
                    result.Add(model);
                }
            }

            var lastmodified = result.Find(o => o.Name == SystemFields.LastModified.Name); 

            if (lastmodified == null)
            { 
                ColumnViewModel model = new ColumnViewModel();
                model.Name = SystemFields.LastModified.Name;  
                model.DisplayName = SystemFields.LastModified.DisplayName;
                model.DataType = SystemFields.LastModified.DataType; 
                result.Insert(0, model); 
            }
             
            return result; 
        }
         
        [Kooboo.Attributes.RequireModel(typeof(CreateContentFolderViewModel))]
        public override Guid Post(ApiCall call)
        {  
            CreateContentFolderViewModel model = call.Context.Request.Model as CreateContentFolderViewModel;
           
            ContentFolder folder = new ContentFolder();
            folder.Name = model.Name;
            folder.ContentTypeId = model.ContentTypeId; 
            folder.DisplayName = model.DisplayName;
            folder.Embedded = model.Embedded;
            folder.Category = model.Category; 

            if (model.Id != default(Guid))
            {
                folder.Id = model.Id; 
            }

            call.WebSite.SiteDb().ContentFolders.AddOrUpdate(folder,call.Context.User.Id);

            return folder.Id;
        }
    }
}
