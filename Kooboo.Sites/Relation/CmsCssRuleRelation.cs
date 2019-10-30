//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Relation
{
    public static class CmsCssRuleRelation
    {
        /// <summary>
        /// Get the object used by Page.
        /// </summary>
        /// <param name="siteDb"></param>
        /// <param name="pageId"></param>
        /// <param name="styleRuleOnly"></param>
        /// <returns></returns>
        public static List<CmsCssRule> ListUsedByPage(SiteDb siteDb, Guid pageId, bool styleRuleOnly = true)
        {
            var allobjectids = siteDb.Pages.GetRelatedOwnerObjectIds(pageId);
            // inline or embedded style rules.

            var inRules = siteDb.CssRules.Query.WhereIn<Guid>(o => o.OwnerObjectId, allobjectids).SelectAll();

            if (styleRuleOnly)
            {
                List<int> removeindex = new List<int>();
                int count = inRules.Count();
                for (int i = 0; i < count; i++)
                {
                    if (inRules[i].ruleType != RuleType.StyleRule)
                    {
                        removeindex.Add(i);
                    }
                }

                foreach (var item in removeindex.OrderByDescending(o => o))
                {
                    inRules.RemoveAt(item);
                }
            }

            return inRules;
        }

        public static void ComputeUrl(CmsCssRule cmsCssRule, string baseurl, SiteDb sitedb)
        {
            bool canhaveuri = false;

            var currentRelations = sitedb.Relations.GetRelations(cmsCssRule.Id);

            foreach (var item in cmsCssRule.Properties)
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

            var urls = Kooboo.Sites.Service.CssService.GetUrlInfos(cmsCssRule.RuleText).Select(o => o.PureUrl).ToList();

            urls = urls.Where(o => Kooboo.Lib.Utilities.DataUriService.isDataUri(o) == false).ToList();

            ComputeUrlRelation(sitedb, cmsCssRule.Id, cmsCssRule.ConstType, urls);

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
            List<Guid> externalResource = new List<Guid>();

            var oldRouteRelations = sitedb.Relations.GetRelationViaRoutes(objectId);
            var oldExternalResourceRelations = sitedb.Relations.GetExternalRelations(objectId, 0);

            byte finalDestConstType = 0;

            foreach (var item in urllist.Distinct())
            {
                finalDestConstType = ConstTypeService.GetConstTypeByUrl(item); ;

                if (Service.DomUrlService.IsExternalLink(item))
                {
                    Guid externalid = Kooboo.Data.IDGenerator.Generate(item, ConstObjectType.ExternalResource);
                    externalResource.Add(externalid);
                    if (oldExternalResourceRelations.Find(o => o.objectYId == externalid) == null)
                    {
                        sitedb.ExternalResource.AddOrUpdate(item, finalDestConstType);
                    }
                }
                else
                {
                    var routeids = DomRelation.GetRouteIds(sitedb, item);
                    internalRoutes.AddRange(routeids);
                    if (routeids.Count == 1 && oldRouteRelations.Find(o => o.objectYId == routeids[0]) == null)
                    {
                        sitedb.Routes.EnsureExists(item, finalDestConstType);
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
                    sitedb.Relations.AddOrUpdate(objectId, item, constType, ConstObjectType.Route, finalDestConstType);
                }
            }

            foreach (var item in oldExternalResourceRelations)
            {
                if (!externalResource.Contains(item.objectYId))
                {
                    sitedb.Relations.Delete(item.Id);
                }
            }

            foreach (var item in externalResource)
            {
                if (oldExternalResourceRelations.Find(o => o.objectYId == item) == null)
                {
                    sitedb.Relations.AddOrUpdate(objectId, item, constType, ConstObjectType.ExternalResource, finalDestConstType);
                }
            }
        }
    }
}