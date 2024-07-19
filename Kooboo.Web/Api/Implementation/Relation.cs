//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Linq;
using Kooboo.Api;
using Kooboo.Data.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Service;

namespace Kooboo.Web.Api.Implementation
{
    public class RelationApi : IApi
    {
        public string ModelName
        {
            get
            {
                return "relation";
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

        [Kooboo.Attributes.RequireParameters("id", "by", "type")]
        public List<UsedByRelation> ShowBy(ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();

            string type = call.GetValue("type");
            string by = call.GetValue("by");

            if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(by))
            {
                return null;
            }

            if (type.ToLower() == "page")
            {
                return PageRelation(sitedb, by, call.ObjectId);
            }

            var repo = sitedb.GetRepository(type);
            var siteObject = repo?.Get(call.ObjectId);

            if (repo == null || siteObject == null)
            {
                return null;
            }

            string baseUrl = call.WebSite.BaseUrl();

            byte constType = ConstTypeContainer.GetConstType(by);

            List<UsedByRelation> result = new List<UsedByRelation>();
            //var usedby = repo.GetUsedBy(call.ObjectId).Where(o=>o.ConstType == consttype).ToList(); 

            if (constType == ConstObjectType.CssRule)
            {
                constType = ConstObjectType.Style;
            }

            var usedBy = repo.GetUsedBy(call.ObjectId).ToList();

            var filter = usedBy.Where(o => o.ConstType == constType).ToList();
            if (!filter.Any())
            {
                filter = usedBy;
            }

            foreach (var item in filter)
            {
                if (item.ModelType == typeof(Kooboo.Sites.Routing.Route))
                {
                    item.Url = sitedb.WebSite.BaseUrl(item.Name);
                }
                else
                {
                    item.Url = sitedb.WebSite.BaseUrl(item.Url);
                }

            }
            return filter;
        }

        private List<UsedByRelation> PageRelation(SiteDb sitedb, string by, Guid PageId)
        {
            string baseUrl = sitedb.WebSite.BaseUrl();

            byte constType = ConstTypeContainer.GetConstType(by);

            List<UsedByRelation> result = new List<UsedByRelation>();

            var relations = sitedb.Relations.GetRelations(PageId, constType);

            foreach (var item in relations)
            {
                var objectInfo = ObjectService.GetObjectInfo(sitedb, item.objectYId, item.ConstTypeY);

                if (objectInfo != null)
                {
                    UsedByRelation relation = new()
                    {
                        Name = objectInfo.DisplayName,
                        ModelType = objectInfo.ModelType,
                        ObjectId = objectInfo.ObjectId,
                        ConstType = objectInfo.ConstType,
                    };
                    if (!string.IsNullOrEmpty(objectInfo.Url))
                    {
                        relation.Url = Lib.Helper.UrlHelper.Combine(baseUrl, objectInfo.Url);
                    }
                    result.Add(relation);
                }
            }
            return result;
        }
    }
}
