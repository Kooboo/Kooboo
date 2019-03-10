//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Sites.Extensions;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Data.Models;
using System;
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
            var siteobject = repo?.Get(call.ObjectId);

            if (repo == null || siteobject == null)
            {
                return null;
            }

            string baseurl = call.WebSite.BaseUrl();
     
            byte consttype = ConstTypeContainer.GetConstType(by);
    
            List<UsedByRelation> result = new List<UsedByRelation>();

            var usedby = repo.GetUsedBy(call.ObjectId).Where(o=>o.ConstType == consttype).ToList(); 
 
            foreach (var item in usedby)
            {
                item.Url = sitedb.WebSite.BaseUrl(item.Url);
            } 
            return usedby; 
        }

        private List<UsedByRelation> PageRelation(SiteDb sitedb, string by, Guid PageId)
        {  
            string baseurl = sitedb.WebSite.BaseUrl();
              
            byte consttype = ConstTypeContainer.GetConstType(by);

            List<UsedByRelation> result = new List<UsedByRelation>();

            var relations = sitedb.Relations.GetRelations(PageId, consttype);

            foreach (var item in relations)
            {
                var objectinfo = ObjectService.GetObjectInfo(sitedb, item.objectYId, item.ConstTypeY);

                if (objectinfo != null)
                {
                    UsedByRelation relation = new UsedByRelation();
                    relation.Name = objectinfo.DisplayName;
                    relation.Url = objectinfo.Url;
                    if (!string.IsNullOrEmpty(relation.Url))
                    {
                        relation.Url = Lib.Helper.UrlHelper.Combine(baseurl, relation.Url);
                    }
                    relation.ModelType = objectinfo.ModelType;
                    relation.ObjectId = objectinfo.ObjectId;
                    relation.ConstType = objectinfo.ConstType;
                    result.Add(relation);
                }
            }
            return result;
        }
    }
}
