//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Relation
{
  public static  class GroupRelation
    { 
        public static void Compute(ResourceGroup group, Repository.SiteDb sitedb)
        {
            var routes = group.Children.Select(o => o.Key).ToList();

            var existings = sitedb.Relations.GetRelations(group.Id);

            foreach (var item in existings)
            {
                if (!routes.Contains(item.objectYId))
                {
                    sitedb.Relations.Delete(item.Id); 
                }
            } 
            foreach (var item in routes)
            {
                sitedb.Relations.AddOrUpdate(group.Id, item, group.ConstType, ConstObjectType.Route, group.Type); 
            } 
        } 
    }
     
}
