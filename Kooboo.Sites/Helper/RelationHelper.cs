//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.Sites.Models;
using Kooboo.Sites.Relation;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Helper
{
    public static class RelationHelper
    {
        public static void SetNameUrl(SiteDb SiteDb, UsedByRelation relation)
        {
            if (relation.ObjectId == default(Guid) || relation.ConstType == default(byte))
            {
                return;
            }
            var objectinfo = Service.ObjectService.GetObjectInfo(SiteDb, relation.ObjectId, relation.ConstType);
            if (objectinfo != null)
            {
                relation.Name = objectinfo.DisplayName;
                relation.Url = objectinfo.Url;
            }
        }

        public static Dictionary<string, int> Sum(List<UsedByRelation> relations)
        {

            Dictionary<string, int> result = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase); 

            if (relations !=null && relations.Any())
            {
                foreach (var item in relations.GroupBy(o=>o.ConstType))
                {            
                    var consttype = item.Key;

                    var type = ConstTypeService.GetModelType(consttype);

                    if (type != null)
                    {
                        var name = type.Name;
                        var count = item.Count();
                        result[name] = count; 
                    }    
                }      
            }

            return result;  

        }

        public static List<UsedByRelation> ShowUsedBy(SiteDb sitedb, List<Relation.ObjectRelation> objectrelations)
        {
            List<UsedByRelation> relations = new List<UsedByRelation>();

            foreach (var item in objectrelations)
            {
                UsedByRelation relation = new UsedByRelation();

                if (item.ConstTypeX == ConstObjectType.CssRule)
                {
                    CmsCssRule cssrule = sitedb.CssRules.Get(item.objectXId);
                    if (cssrule == null)
                    {
                        continue;
                    }
                    var declarations = Kooboo.Dom.CSS.CSSSerializer.deserializeDeclarationBlock(cssrule.CssText);

                    string remark = cssrule.SelectorText + "{";
                    foreach (var decl in declarations.item)
                    {
                        if (Sites.Tag.Property.CanHaveUri(decl.propertyname))
                        {
                            remark += decl.propertyname + ":" + decl.value;
                        }
                    }
                    remark += "}";
                    relation.Remark = remark;
                }

                var objectinfo = Sites.Service.ObjectService.GetObjectInfo(sitedb, item.objectXId, item.ConstTypeX);

                if (objectinfo !=null)
                {
                    relation.Name = objectinfo.DisplayName;
                    relation.Url = objectinfo.Url;
                    relation.ModelType = objectinfo.ModelType;
                    relation.ObjectId = objectinfo.ObjectId;
                    relation.ConstType = objectinfo.ConstType;
                }       

                relations.Add(relation);
            }

            return relations;
        }

    }
}
