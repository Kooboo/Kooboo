//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Linq;
using Kooboo.Api;
using Kooboo.Data.Permission;
using Kooboo.Dom;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Service;
using Kooboo.Web.ViewModel;

namespace Kooboo.Web.Api.Implementation
{
    public class LayoutApi : SiteObjectApi<Layout>
    {
        [Permission(Feature.LAYOUT, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.PAGES, Action = Data.Permission.Action.VIEW)]
        public override object Get(ApiCall call)
        {
            Layout result = null;

            if (call.ObjectId != default)
            {
                var layoutobject = base.Get(call);
                if (layoutobject != null)
                {
                    var layout = layoutobject as Layout;
                    result = layout.Clone<Layout>();
                }
            }
            else
            {
                var name = call.GetValue("id");
                if (!string.IsNullOrEmpty(name))
                {
                    result = call.WebSite.SiteDb().Layouts.GetByNameOrId(name);
                }
                if (result == null)
                {
                    result = new Layout
                    {
                        Body = @"<!DOCTYPE html>
<html lang=""en"">

<head>
    <meta charset=""UTF-8"">
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Document</title>
</head>

<body>
    <div k-placeholder=""Main""> Sample text inside the layout.. </div>
</body>

</html>"
                    };
                }

            }
            AppendDesignScript(result, call);
            return result;
        }

        private static void AppendDesignScript(Layout layout, ApiCall call)
        {
            var design = call.GetValue("design") == "true";
            if (!design)
            {
                return;
            }

            var html = layout.Body;
            var script = VisualEditorHelper.GetInjects(call);

            var dom = DomParser.CreateDom(html);
            var changes = new List<SourceUpdate> {
                new SourceUpdate
                {
                    StartIndex = dom.head.location.endTokenStartIndex - 1,
                    EndIndex = dom.head.location.endTokenStartIndex - 1,
                    NewValue = $"\t{script}\n"
                }
            };

            var placeholders = dom.Select("[k-placeholder],[v-placeholder]");
            foreach (var item in placeholders.item)
            {
                var placeholder = item.getAttribute("v-placeholder");
                if (!string.IsNullOrEmpty(placeholder))
                {
                    item.setAttribute("k-placeholder", placeholder);
                    item.removeAttribute("v-placeholder");
                }
                var attrs = item.attributes.Select(it => $"{it.name}=\"{it.value}\"").ToArray();
                changes.Add(new SourceUpdate
                {
                    StartIndex = item.location.openTokenStartIndex,
                    EndIndex = item.location.openTokenEndIndex,
                    NewValue = $"<ve-placeholder {string.Join(" ", attrs)}>"
                });
                changes.Add(new SourceUpdate
                {
                    StartIndex = item.location.endTokenStartIndex,
                    EndIndex = item.location.endTokenEndIndex,
                    NewValue = "</ve-placeholder>"
                });
            }

            layout.Body = DomService.UpdateSource(html, changes);
        }

        [Kooboo.Attributes.RequireModel(typeof(LayoutViewModel))]
        [Permission(Feature.LAYOUT, Action = Data.Permission.Action.EDIT)]
        public override Guid Post(ApiCall call)
        {
            var value = (LayoutViewModel)call.Context.Request.Model;
            value.Body = Sites.Service.HtmlHeadService.RemoveBaseHrel(value.Body);

            if (value.Id != Guid.Empty)
            {
                var oldValue = call.WebSite.SiteDb().Layouts.Get(value.Id);
                if (oldValue != null)
                {
                    (value as IDiffChecker).CheckDiff(oldValue);
                }
            }

            call.WebSite.SiteDb().Layouts.AddOrUpdate(value, call.Context.User.Id);
            return value.Id;
        }

        [Permission(Feature.LAYOUT, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.PAGES, Action = Data.Permission.Action.VIEW)]
        public override List<object> List(ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();

            int storenamehash = Lib.Security.Hash.ComputeInt(sitedb.Layouts.StoreName);

            return sitedb
                .Layouts
                .All()
                .SortByNameOrLastModified(call)
                .Select(item => new LayoutItemViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    KeyHash = Sites.Service.LogService.GetKeyHash(item.Id),
                    StoreNameHash = storenamehash,
                    LastModified = item.LastModified,
                    Relations = Sites.Helper.RelationHelper.Sum(sitedb.Layouts.GetUsedBy(item.Id))
                })
                .ToList<object>();
        }

        [Kooboo.Attributes.RequireParameters("id", "name")]
        [Permission(Feature.LAYOUT, Action = Data.Permission.Action.EDIT)]
        public LayoutItemViewModel Copy(ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();

            var layout = sitedb.Layouts.Get(call.ObjectId);
            if (layout != null)
            {
                var newlayout = Lib.Serializer.Copy.DeepCopy<Layout>(layout);
                newlayout.CreationDate = DateTime.UtcNow;
                newlayout.LastModified = DateTime.UtcNow;

                newlayout.Name = call.GetValue("name");
                sitedb.Layouts.AddOrUpdate(newlayout, call.Context.User.Id);

                int storenamehash = Lib.Security.Hash.ComputeInt(call.WebSite.SiteDb().Layouts.StoreName);

                LayoutItemViewModel model = new LayoutItemViewModel();
                model.Id = newlayout.Id;
                model.Name = newlayout.Name;
                model.KeyHash = Sites.Service.LogService.GetKeyHash(newlayout.Id);
                model.StoreNameHash = storenamehash;
                model.LastModified = newlayout.LastModified;
                model.Relations = Sites.Helper.RelationHelper.Sum(sitedb.Layouts.GetUsedBy(newlayout.Id));

                return model;
            }
            return null;
        }

        [Permission(Feature.LAYOUT, Action = Data.Permission.Action.DELETE)]
        public override bool Deletes(ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();

            string json = call.GetValue("ids");
            if (string.IsNullOrEmpty(json))
            {
                json = call.Context.Request.Body;
            }
            List<Guid> ids = Lib.Helper.JsonHelper.Deserialize<List<Guid>>(json);

            if (ids != null && ids.Count() > 0)
            {
                foreach (var item in ids)
                {
                    var relations = sitedb.Relations.GetReferredBy(this.ModelType, item);
                    if (relations != null && relations.Count > 0)
                    {
                        throw new Exception(Data.Language.Hardcoded.GetValue("Layout is being used, can not be deleted", call.Context));
                    }

                    sitedb.Layouts.Delete(item, call.Context.User.Id);
                }
                return true;
            }
            return false;
        }

        [Permission(Feature.LAYOUT, Action = Data.Permission.Action.EDIT)]
        public override Guid AddOrUpdate(ApiCall call)
        {
            return base.AddOrUpdate(call);
        }

        [Permission(Feature.LAYOUT, Action = Data.Permission.Action.DELETE)]
        public override bool Delete(ApiCall call)
        {
            return base.Delete(call);
        }

        public override bool IsUniqueName(ApiCall call)
        {
            return base.IsUniqueName(call);
        }

        [Permission(Feature.LAYOUT, Action = Data.Permission.Action.EDIT)]
        public override Guid put(ApiCall call)
        {
            return base.put(call);
        }
    }
}
