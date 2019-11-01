//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Api;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Web.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Web.Api.Implementation
{
    public class LabelApi : SiteObjectApi<Label>
    {
        [Kooboo.Attributes.RequireParameters("id", "values")]
        public void Update(ApiCall call)
        {
            var label = call.WebSite.SiteDb().Labels.Get(call.ObjectId);
            if (label != null)
            {
                Dictionary<string, string> values = Lib.Helper.JsonHelper.Deserialize<Dictionary<string, string>>(call.GetValue("values"));
                foreach (var item in values)
                {
                    label.SetValue(item.Key, item.Value);
                }
                call.WebSite.SiteDb().Labels.AddOrUpdate(label, call.Context.User.Id);
            }
        }

        public override List<object> List(ApiCall call)
        {
            List<LabelItemViewModel> result = new List<LabelItemViewModel>();

            var sitedb = call.WebSite.SiteDb();

            int storenamehash = Lib.Security.Hash.ComputeInt(sitedb.Labels.StoreName);

            foreach (var item in sitedb.Labels.All())
            {
                LabelItemViewModel model = new LabelItemViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    KeyHash = Sites.Service.LogService.GetKeyHash(item.Id),
                    StoreNameHash = storenamehash,
                    LastModified = item.LastModified,
                    Values = item.Values,
                    Relations = Sites.Helper.RelationHelper.Sum(sitedb.Labels.GetUsedBy(item.Id))
                };
                result.Add(model);
            }
            return result.ToList<object>();
        }

        public List<string> Keys(ApiCall call)
        {
            List<string> keys = new List<string>();
            foreach (var item in call.WebSite.SiteDb().Labels.All())
            {
                keys.Add(item.Name);
            }
            return keys;
        }
    }
}