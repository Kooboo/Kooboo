//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Repository;
using Kooboo.Sites.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Cache
{
   public static class RouteTreeCache
    {
        private static Dictionary<string, PathTree> _RouteTree = new Dictionary<string, PathTree>();

        private static Dictionary<Guid, Dictionary<byte, PathTree>> SiteTypeRouteTrees = new Dictionary<Guid, Dictionary<byte, PathTree>>(); 

        private static object _object = new object();

        public static PathTree RouteTree(SiteDb sitedb, byte ConstType = 0)
        {

            lock(_object)
            {
                Dictionary<byte, PathTree> sitetree; 
                if (SiteTypeRouteTrees.ContainsKey(sitedb.Id))
                {
                    sitetree = SiteTypeRouteTrees[sitedb.Id]; 
                }
                else
                {
                    sitetree = new Dictionary<byte, PathTree>();
                    SiteTypeRouteTrees[sitedb.Id] = sitetree; 
                }

                PathTree pathtree; 

                if (sitetree.ContainsKey(ConstType))
                {
                    pathtree = sitetree[ConstType]; 
                }
                else
                {
                    pathtree = GetRouteTree(sitedb, ConstType);
                    sitetree[ConstType] = pathtree; 
                }
                return pathtree; 
            } 
          
        }

        private static PathTree GetRouteTree(SiteDb db, byte ObjectType = 0)
        {
            var filter = db.Routes.Query;
            if (ObjectType != 0)
            {
                filter.Where(o => o.DestinationConstType == ObjectType);
            }

            var routelist = filter.SelectAll();

            PathTree tree = new PathTree();
            foreach (var item in routelist)
            {
                tree.AddOrUpdate(item);
            }

            // append system routes... 
            //if (ObjectType == 0)
            //{
            //    var systemroutes = Kooboo.Sites.Systems.Routes.DefaultRoutes();
            //    foreach (var item in systemroutes)
            //    {
            //        tree.AddOrUpdate(item);

            //        Cache.SiteObjectCache<Route>.AddOrUpdate(db, item);
            //    }
            //}

            return tree;
        }
         
        public static void RemoveSiteDb(Guid id)
        {
            SiteTypeRouteTrees.Remove(id); 
        }
    }
}
