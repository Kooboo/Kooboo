//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Api;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Web.Api.Implementation
{
    public class KConfigApi : SiteObjectApi<KConfig>
    {
        public void Update(KConfig model, ApiCall call)
        {
            if (model.Id != default(Guid))
            {
                var sitedb = call.WebSite.SiteDb();
                var config = sitedb.KConfig.Get(model.Id);
                if (config != null)
                {
                    config.Binding = model.Binding;
                    sitedb.KConfig.AddOrUpdate(config);
                }
            }
        }

        public override object Get(ApiCall call)
        {
            var obj = base.Get(call);

            if (obj is KConfig)
            {
                var kconfig = obj as KConfig;

                var model = new KConfigEditModel(kconfig) {Id = kconfig.Id.ToString()};


                if (kconfig.TagName == "img")
                {
                    model.ControlType = Kooboo.Data.Definition.ControlTypes.MediaFile;
                }
                return model;
            }
            return obj;
        }

        public override List<object> List(ApiCall call)
        {
            List<KConfigItemViewModel> result = new List<KConfigItemViewModel>();

            var sitedb = call.WebSite.SiteDb();

            int storenamehash = Lib.Security.Hash.ComputeInt(sitedb.KConfig.StoreName);

            foreach (var item in sitedb.KConfig.All())
            {
                KConfigItemViewModel model = new KConfigItemViewModel
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
                };

                result.Add(model);
            }
            return result.ToList<object>();
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
    }
}