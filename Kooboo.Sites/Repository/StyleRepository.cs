//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.IndexedDB;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Repository
{
    public class StyleRepository : IEmbeddableRepositoryBase<Style>
    {
        public override IndexedDB.ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters param = new ObjectStoreParameters();
                param.AddColumn<Style>(it => it.OwnerObjectId);
                param.AddColumn<Style>(it => it.OwnerConstType);
                param.AddColumn<Style>(o => o.BodyHash);
                param.AddColumn<Style>(o => o.Id);
                param.AddColumn<Style>(o => o.IsEmbedded);
                param.AddColumn<Style>(o => o.LastModified);
                param.AddColumn("Name", 100);
                param.SetPrimaryKeyField<Style>(o => o.Id);
                return param;
            }
        }

        public override List<UsedByRelation> GetUsedBy(Guid objectId)
        {
            var style = this.Get(objectId);
            return GetUsedBy(style);
        }

        public List<UsedByRelation> GetUsedBy(Style style)
        {
            List<UsedByRelation> result = new List<UsedByRelation>();
            if (style == null)
            { return result; }

            // for embedded style, this is belong to one parent object.
            if (style.IsEmbedded)
            {
                var samestyles = this.GetSameEmbedded(style.BodyHash);

                foreach (var item in samestyles)
                {
                    UsedByRelation relation = new UsedByRelation
                    {
                        ObjectId = item.OwnerObjectId, ConstType = item.OwnerConstType
                    };
                    Helper.RelationHelper.SetNameUrl(this.SiteDb, relation);
                    relation.Remark = item.KoobooOpenTag;
                    result.Add(relation);
                }

                return result;
            }

            var relations = this.SiteDb.Relations.GetReferredBy(style);

            foreach (var item in relations)
            {
                //import rule has relation to cssrule only.
                //this style if reference by another style using import rule.
                if (item.ConstTypeX == ConstObjectType.CssRule)
                {
                    CmsCssRule cssrule = this.SiteDb.CssRules.Get(item.objectXId);
                    if (cssrule.ParentStyleId != default(Guid))
                    {
                        Style parentStyle = this.SiteDb.Styles.Get(cssrule.ParentStyleId);

                        if (parentStyle != null)
                        {
                            var parentresults = this.GetUsedBy(parentStyle);
                            if (parentresults != null && parentresults.Any())
                            {
                                result.AddRange(parentresults);
                            }

                            UsedByRelation relation = new UsedByRelation
                            {
                                ObjectId = parentStyle.Id, ConstType = parentStyle.ConstType
                            };
                            Helper.RelationHelper.SetNameUrl(this.SiteDb, relation);
                            relation.Remark = cssrule.CssText;
                            result.Add(relation);
                        }
                    }
                }
                else
                {
                    UsedByRelation relation = new UsedByRelation
                    {
                        ObjectId = item.objectXId, ConstType = item.ConstTypeX
                    };
                    Helper.RelationHelper.SetNameUrl(this.SiteDb, relation);
                    result.Add(relation);
                }
            }
            return result;
        }

        /// <summary>
        ///  get all the routed style ids that imported by current style id.
        /// </summary>
        /// <param name="styleId"></param>
        /// <returns></returns>
        public List<Guid> GetImports(Guid styleId)
        {
            List<Guid> result = new List<Guid>();

            var allimportrules = SiteDb.CssRules.Query.Where(o => o.ParentStyleId == styleId && o.ruleType == RuleType.ImportRule).SelectAll();

            foreach (var item in allimportrules)
            {
                var relation = SiteDb.Relations.Query.Where(o => o.objectXId == item.Id && o.ConstTypeY == ConstObjectType.Route).FirstOrDefault();
                if (relation != null && relation.objectYId != default(Guid))
                {
                    var route = SiteDb.Routes.Get(relation.objectYId);
                    if (route != null && route.DestinationConstType == ConstObjectType.Style)
                    {
                        if (route.objectId != default(Guid))
                        {
                            result.Add(route.objectId);
                            var subimports = GetImports(route.objectId);
                            if (subimports != null && subimports.Any())
                            {
                                result.AddRange(subimports);
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}