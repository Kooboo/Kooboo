//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Linq;
using Kooboo.Api;
using Kooboo.Data.Permission;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Web.ViewModel;

namespace Kooboo.Web.Api.Implementation
{
    public class ContentFolderApi : SiteObjectApi<ContentFolder>
    {
        [Permission(Feature.CONTENT, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.CONTENT_TYPE, Action = Data.Permission.Action.VIEW)]
        public override List<object> List(ApiCall call)
        {
            var siteDb = call.Context.WebSite.SiteDb();
            List<ContentFolderViewModel> result = new List<ContentFolderViewModel>();
            var all = siteDb.ContentFolders.All();

            foreach (var item in all)
            {
                ContentFolderViewModel model = ContentFolderViewModel.Create(item);
                var relations = siteDb.ContentFolders.GetUsedBy(item.Id);

                if (relations != null)
                {
                    model.Relations = Sites.Helper.RelationHelper.Sum(relations);
                }

                if (item.IsContent)
                {
                    model.DefaultContentId = siteDb.TextContent.Query.Where(w => w.FolderId == item.Id).FirstOrDefault()?.Id;
                }

                result.Add(model);
            }

            return [.. result.OrderBy(it => string.IsNullOrWhiteSpace(it.DisplayName) ? it.Name : it.DisplayName)];
        }

        [Kooboo.Attributes.RequireParameters("id")]
        [Permission(Feature.CONTENT, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.CONTENT_TYPE, Action = Data.Permission.Action.VIEW)]
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
        [Permission(Feature.CONTENT_TYPE, Action = Data.Permission.Action.EDIT)]
        public override Guid Post(ApiCall call)
        {
            CreateContentFolderViewModel model = call.Context.Request.Model as CreateContentFolderViewModel;

            ContentFolder folder = new ContentFolder
            {
                Name = model.Name,
                ContentTypeId = model.ContentTypeId,
                DisplayName = model.DisplayName,
                Embedded = model.Embedded,
                Category = model.Category,
                Hidden = model.Hidden,
                Sortable = model.Sortable,
                SortField = model.SortField,
                Ascending = model.Ascending,
                PageSize = model.PageSize,
                IsContent = model.IsContent,
                FieldsOrder = model.FieldsOrder,
                Group = model.Group,
                PreviewUrl=model.PreviewUrl
            };

            var all = call.WebSite.SiteDb().ContentFolders.All();

            if (model.Id != default(Guid))
            {
                folder.Id = model.Id;
                var exist = all.FirstOrDefault(f => f.Id == model.Id);
                if (exist != null) folder.Order = exist.Order;
            }
            else
            {
                var minOrder = all.OrderBy(o => o.Order).FirstOrDefault()?.Order ?? 0;
                folder.Order = minOrder - 1;
            }

            call.WebSite.SiteDb().ContentFolders.AddOrUpdate(folder, call.Context.User.Id);

            return folder.Id;
        }

        [Permission(Feature.CONTENT_TYPE, Action = Data.Permission.Action.EDIT)]
        public override Guid AddOrUpdate(ApiCall call)
        {
            return base.AddOrUpdate(call);
        }

        [Permission(Feature.CONTENT_TYPE, Action = Data.Permission.Action.EDIT)]
        public override bool Delete(ApiCall call)
        {
            return base.Delete(call);
        }

        [Permission(Feature.CONTENT_TYPE, Action = Data.Permission.Action.EDIT)]
        public override bool Deletes(ApiCall call)
        {
            return base.Deletes(call);
        }

        [Permission(Feature.CONTENT, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.CONTENT_TYPE, Action = Data.Permission.Action.VIEW)]
        public override object Get(ApiCall call)
        {
            return base.Get(call);
        }

        [Permission(Feature.CONTENT_TYPE, Action = Data.Permission.Action.EDIT)]
        public override Guid put(ApiCall call)
        {
            return base.put(call);
        }

        [Permission(Feature.CONTENT_TYPE, Action = Data.Permission.Action.EDIT)]
        public void Sort(List<string> ordered, ApiCall call)
        {
            var siteDb = call.WebSite.SiteDb();
            var all = siteDb.ContentFolders.All();
            foreach (var item in all)
            {
                var order = ordered.IndexOf(item.Name);
                if (order == -1 && item.Hidden) continue;
                if (order != item.Order)
                {
                    item.Order = order;
                    siteDb.ContentFolders.AddOrUpdate(item);
                }
            }
        }
    }
}
