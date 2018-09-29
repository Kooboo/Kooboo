//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Sites.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Dom.CSS;
using Kooboo.Dom;
using System.Linq;
using Kooboo.Data.Models;
using Kooboo.Data.Extensions;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Routing;
using Kooboo.Sites.Service;
using Kooboo.Sites.Contents.Models;

namespace Kooboo.Sites.Relation
{
    public static class CmsCssRuleRelation
    {
        /// <summary>
        /// Get the object used by Page.
        /// </summary>
        /// <param name="website"></param>
        /// <param name="PageId"></param>
        /// <returns></returns>
        public static List<CmsCssRule> ListUsedByPage(SiteDb SiteDb, Guid PageId, bool StyleRuleOnly = true)
        {
            var allobjectids = SiteDb.Pages.GetRelatedOwnerObjectIds(PageId);
            // inline or embedded style rules.
            List<CmsCssRule> InRules = new List<CmsCssRule>();

            InRules = SiteDb.CssRules.Query.WhereIn<Guid>(o => o.OwnerObjectId, allobjectids).SelectAll();

            if (StyleRuleOnly)
            {
                List<int> removeindex = new List<int>();
                int count = InRules.Count();
                for (int i = 0; i < count; i++)
                {
                    if (InRules[i].ruleType != RuleType.StyleRule)
                    {
                        removeindex.Add(i);
                    }
                }

                foreach (var item in removeindex.OrderByDescending(o => o))
                {
                    InRules.RemoveAt(item);
                }
            }

            return InRules;
        }
          
        public static void ComputeUrl(CmsCssRule CmsCssRule, string baseurl, SiteDb sitedb)
        {
            bool canhaveuri = false;

            var currentRelations = sitedb.Relations.GetRelations(CmsCssRule.Id);

            foreach (var item in CmsCssRule.Properties)
            {
                if (Kooboo.Sites.Tag.Property.CanHaveUri(item))
                { canhaveuri = true; break; }
            }
            if (!canhaveuri)
            {
                foreach (var item in currentRelations)
                {
                    sitedb.Relations.Delete(item.Id);
                }
                return;
            }

            var urls = Kooboo.Sites.Service.CssService.GetUrlInfos(CmsCssRule.RuleText).Select(o => o.PureUrl).ToList();

            urls = urls.Where(o => Kooboo.Lib.Utilities.DataUriService.isDataUri(o) == false).ToList(); 

            ComputeUrlRelation(sitedb, CmsCssRule.Id, CmsCssRule.ConstType, urls); 

            //Dictionary<Guid, string> routelist = null;

            //if (urls.Count == 0)
            //{
            //    routelist = new Dictionary<Guid, string>();
            //}
            //else
            //{
            //    routelist = Service.CssService.ConvertUrlsToRoutes(urls, baseurl);
            //}

            //foreach (var item in currentRelations)
            //{
            //    if (!routelist.Keys.Contains(item.objectYId))
            //    {
            //        sitedb.Relations.Delete(item.Id); 
            //    }
            //}

            //foreach (var item in routelist)
            //{
            //    byte consttype = 0;
            //    var objecttype = Kooboo.Lib.Helper.UrlHelper.GetFileType(item.Value);

            //    if (objecttype == Lib.Helper.UrlHelper.UrlFileType.Image)
            //    {
            //        consttype = ConstObjectType.Image;
            //    }
            //    else
            //    {
            //        consttype = ConstObjectType.File;
            //    }

            //    if (currentRelations.Find(o => o.objectYId == item.Key) == null)
            //    {
            //        sitedb.Routes.EnsureExists(item.Value, consttype);
            //        sitedb.Relations.AddOrUpdate(CmsCssRule.Id, item.Key, CmsCssRule.ConstType, ConstObjectType.Route, consttype);
            //    }
            //}
        }

        public static void ComputeUrlRelation(SiteDb sitedb, Guid objectId, byte constType, List<string> urllist)
        {
            List<Guid> internalRoutes = new List<Guid>();
            List<Guid> ExternalResource = new List<Guid>();
            
            var oldRouteRelations = sitedb.Relations.GetRelationViaRoutes(objectId);
            var oldExternalResourceRelations = sitedb.Relations.GetExternalRelations(objectId, 0);

            byte FinalDestConstType = 0; 

            foreach (var item in urllist.Distinct())
            {
                  FinalDestConstType = ConstTypeService.GetConstTypeByUrl(item); ;
                
                if (Service.DomUrlService.IsExternalLink(item))
                {
                    Guid externalid = Kooboo.Data.IDGenerator.Generate(item, ConstObjectType.ExternalResource);
                    ExternalResource.Add(externalid);
                    if (oldExternalResourceRelations.Find(o => o.objectYId == externalid) == null)
                    {
                        sitedb.ExternalResource.AddOrUpdate(item, FinalDestConstType);
                    }
                }
                else
                {
                    var routeids = DomRelation.GetRouteIds(sitedb, item);
                    internalRoutes.AddRange(routeids);
                    if (routeids.Count == 1 && oldRouteRelations.Find(o => o.objectYId == routeids[0]) == null)
                    {
                        sitedb.Routes.EnsureExists(item, FinalDestConstType);
                    }
                }
            }

            foreach (var item in oldRouteRelations)
            {
                if (!internalRoutes.Contains(item.objectYId))
                {
                    sitedb.Relations.Delete(item.Id);
                }
            }

            foreach (var item in internalRoutes)
            {
                if (oldRouteRelations.Find(o => o.objectYId == item) == null)
                {
                    sitedb.Relations.AddOrUpdate(objectId, item, constType, ConstObjectType.Route, FinalDestConstType);
                }
            }


            foreach (var item in oldExternalResourceRelations)
            {
                if (!ExternalResource.Contains(item.objectYId))
                {
                    sitedb.Relations.Delete(item.Id);
                }
            }

            foreach (var item in ExternalResource)
            {
                if (oldExternalResourceRelations.Find(o => o.objectYId == item) == null)
                {
                    sitedb.Relations.AddOrUpdate(objectId, item, constType, ConstObjectType.ExternalResource, FinalDestConstType);
                }
            }
        }

        
    }

}
