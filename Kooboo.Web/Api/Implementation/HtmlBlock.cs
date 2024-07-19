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
    public class HtmlBlockApi : SiteObjectApi<HtmlBlock>
    {

        [Kooboo.Attributes.RequireParameters("id", "values")]
        [Permission(Feature.HTML_BLOCK, Action = Data.Permission.Action.EDIT)]
        public override Guid Post(ApiCall apiCall)
        {
            Guid id = apiCall.ObjectId;
            var strvalues = apiCall.GetValue("values");
            if (string.IsNullOrEmpty(strvalues))
            {
                return default(Guid);
            }

            Dictionary<string, object> values = Lib.Helper.JsonHelper.Deserialize<Dictionary<string, object>>(strvalues);

            if (id != default(Guid))
            {
                var current = apiCall.WebSite.SiteDb().HtmlBlocks.Get(id);
                if (current != null)
                {
                    foreach (var item in values)
                    {
                        current.SetValue(item.Key, item.Value);
                    }
                    apiCall.WebSite.SiteDb().HtmlBlocks.AddOrUpdate(current, apiCall.Context.User.Id);
                    return id;
                }
            }
            else
            {
                string name = apiCall.GetValue("name");
                HtmlBlock newblock = new HtmlBlock();
                newblock.Name = name;
                newblock.Values = values;

                apiCall.WebSite.SiteDb().HtmlBlocks.AddOrUpdate(newblock, apiCall.Context.User.Id);

                return newblock.Id;
            }

            return default(Guid);
        }

        [Permission(Feature.HTML_BLOCK, Action = Data.Permission.Action.VIEW)]
        public override List<object> List(ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();

            int storenamehash = Lib.Security.Hash.ComputeInt(sitedb.HtmlBlocks.StoreName);

            return sitedb.HtmlBlocks
                .All()
                .OrderByDescending(it => it.LastModified)
                .Select(item => new HtmlBlockItemViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    KeyHash = Sites.Service.LogService.GetKeyHash(item.Id),
                    StoreNameHash = storenamehash,
                    LastModified = item.LastModified,
                    Values = item.Values,
                    Relations = Sites.Helper.RelationHelper.Sum(sitedb.HtmlBlocks.GetUsedBy(item.Id))
                })
                .ToList<object>();
        }

        [Permission(Feature.HTML_BLOCK, Action = Data.Permission.Action.EDIT)]
        public override Guid AddOrUpdate(ApiCall call)
        {
            return base.AddOrUpdate(call);
        }

        [Permission(Feature.HTML_BLOCK, Action = Data.Permission.Action.DELETE)]
        public override bool Delete(ApiCall call)
        {
            return base.Delete(call);
        }

        [Permission(Feature.HTML_BLOCK, Action = Data.Permission.Action.DELETE)]
        public override bool Deletes(ApiCall call)
        {
            return base.Deletes(call);
        }

        [Permission(Feature.HTML_BLOCK, Action = Data.Permission.Action.VIEW)]
        public override object Get(ApiCall call)
        {
            return base.Get(call);
        }

        [Permission(Feature.HTML_BLOCK, Action = Data.Permission.Action.EDIT)]
        public override Guid put(ApiCall call)
        {
            return base.put(call);
        }
    }
}
