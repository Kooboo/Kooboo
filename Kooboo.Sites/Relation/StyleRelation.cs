//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Dom;
using Kooboo.Dom.CSS;
using Kooboo.IndexedDB;
using Kooboo.Sites.Extensions;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Models;
using Kooboo.Sites.Routing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Service;

namespace Kooboo.Sites.Relation
{
    /// <summary>
    /// Recalculate the style relation. Style has relations to css rules. 
    /// </summary>
    public static class StyleRelation
    {
        /// <summary>
        /// Calculate or reclculate the relation to css rule. 
        /// </summary>
        /// <param name="style"></param>
        /// <param name="sitedb"></param>
        public static void Compute(Style style, Repository.SiteDb sitedb)
        {
            var body = style.Body; 

            var covnerted = Kooboo.Sites.Service.CssService.ConvertCss(body, style.Id);

            if (style.OwnerObjectId != default(Guid))
            {
                foreach (var item in covnerted)
                {
                    item.CmsRule.OwnerObjectId = style.OwnerObjectId;
                    item.CmsRule.OwnerObjectConstType = style.OwnerConstType;
                }
            }

            // remove not any more valid rules. 
            RemoveOldRules(style, sitedb, covnerted);

            string baseurl = ObjectService.GetObjectRelativeUrl(sitedb, style);

            foreach (var item in covnerted)
            {
                ///compute import relation or the style image/font definition... 
                if (item.CmsRule.ruleType == RuleType.ImportRule)
                {
                    var importrule = item.CssRule as Kooboo.Dom.CSS.CSSImportRule;
                    if (importrule != null)
                    {
                        string url = importrule.href;

                        if (!string.IsNullOrEmpty(url))
                        {
                            string objecturl = Kooboo.Lib.Helper.UrlHelper.Combine(baseurl, url);

                            Route temprouteid = new Route() { Name = objecturl };
                            Guid routeid = temprouteid.Id;

                            List<ObjectRelation> oldrelations = sitedb.Relations.GetRelations(item.RuleId);

                            if (oldrelations.Count == 1 && oldrelations[0].objectYId == routeid && oldrelations[0].ConstTypeY == ConstObjectType.Route)
                            {
                                // the relation already there. 
                                continue;
                            }

                            foreach (var relation in oldrelations)
                            {
                                sitedb.Relations.Delete(relation.Id);
                            }

                            sitedb.Relations.AddOrUpdate(item.CmsRule.Id, routeid, item.CmsRule.ConstType, ConstObjectType.Route, ConstObjectType.Style);
                        }
                    }
                }
                else if (item.CmsRule.ruleType == RuleType.StyleRule)
                {
                    Relation.CmsCssRuleRelation.ComputeUrl(item.CmsRule, baseurl, sitedb);
                }
                sitedb.CssRules.AddOrUpdate(item.CmsRule);
            }
        }

        /// <summary>
        /// Remove the cssrules that is not used any more. 
        /// </summary>
        private static void RemoveOldRules(Style style, SiteDb sitedb, List<CssConvertResult> covnertedRules)
        {
            List<CmsCssRule> existingrules = sitedb.CssRules.Query.Where(o => o.ParentStyleId == style.Id).SelectAll();

            foreach (var item in existingrules)
            {
                if (covnertedRules.Find(o => o.CmsRule.Id == item.Id) == null)
                {
                    sitedb.CssRules.Delete(item.Id, false);
                }
            }
        }

    }

}
