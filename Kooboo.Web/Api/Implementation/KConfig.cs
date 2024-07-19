//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Linq;
using Kooboo.Api;
using Kooboo.Data.Permission;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Web.ViewModel;

namespace Kooboo.Web.Api.Implementation
{
    public class KConfigApi : SiteObjectApi<KConfig>
    {
        [Permission(Feature.TEXT, Action = Data.Permission.Action.EDIT)]
        public void Update(KConfig model, ApiCall call)
        {
            if (model.Id != default(Guid))
            {
                var sitedb = call.WebSite.SiteDb();
                var config = sitedb.KConfig.Get(model.Id);
                if (config != null)
                {
                    config.Binding = model.Binding;
                    sitedb.KConfig.AddOrUpdate(config, call.Context.User.Id);
                }
            }
        }

        [Permission(Feature.TEXT, Action = Data.Permission.Action.VIEW)]
        public override object Get(ApiCall call)
        {
            var obj = base.Get(call);

            if (obj is KConfig)
            {
                var kconfig = obj as KConfig;

                var model = new KConfigEditModel(kconfig);

                model.Id = kconfig.Id.ToString();

                if (kconfig.TagName == "img")
                {
                    model.ControlType = Kooboo.Data.Definition.ControlTypes.MediaFile;
                }
                return model;
            }
            return obj;
        }

        [Permission(Feature.TEXT, Action = Data.Permission.Action.VIEW)]
        public override List<object> List(ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();

            int storenamehash = Lib.Security.Hash.ComputeInt(sitedb.KConfig.StoreName);

            return sitedb
                .KConfig
                .All()
                .OrderByDescending(it => it.LastModified)
                .Select(item => new KConfigItemViewModel
                {
                    Id = item.Id,
                    TagName = item.TagName,
                    TagHtml = item.TagHtml,
                    Name = item.Name,
                    KeyHash = Sites.Service.LogService.GetKeyHash(item.Id),
                    StoreNameHash = storenamehash,
                    LastModified = item.LastModified,
                    Binding = item.Binding,

                    Relations = Sites.Helper.RelationHelper.Sum(sitedb.KConfig.GetUsedBy(item.Id))
                })
                .ToList<object>();
        }

        public List<string> Keys(ApiCall call)
        {
            List<string> keys = new List<string>();
            foreach (var item in call.WebSite.SiteDb().KConfig.All())
            {
                keys.Add(item.Name);
            }
            return keys;
        }

        [Permission(Feature.TEXT, Action = Data.Permission.Action.EDIT)]
        public override Guid AddOrUpdate(ApiCall call)
        {
            return base.AddOrUpdate(call);
        }

        [Permission(Feature.TEXT, Action = Data.Permission.Action.DELETE)]
        public override bool Delete(ApiCall call)
        {
            return base.Delete(call);
        }

        [Permission(Feature.TEXT, Action = Data.Permission.Action.DELETE)]
        public override bool Deletes(ApiCall call)
        {
            return base.Deletes(call);
        }

        [Permission(Feature.TEXT, Action = Data.Permission.Action.EDIT)]
        public override Guid Post(ApiCall call)
        {
            return base.Post(call);
        }

        [Permission(Feature.TEXT, Action = Data.Permission.Action.EDIT)]
        public override Guid put(ApiCall call)
        {
            return base.put(call);
        }
    }
}
