//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using Kooboo.Extensions;
using Kooboo.Sites.Repository;

namespace Kooboo.Sites.Systems
{
    public class Routes
    {
         
        public static string SystemRouteTemplate = "/__kb/{objecttype}/{nameorid}";
        public static string SystemRouteTemplateWithAction = "/__kb/{objecttype}/{nameorid}/{action}";

        //prefix to determine that this is an system route...
        public static string SystemRoutePrefix = "/__kb/"; 


        public static List<Routing.Route> DefaultRoutes()
        {
            var routes = new List<Routing.Route>();

            Routing.Route systemroute = new Routing.Route();
            systemroute.Name = SystemRouteTemplate;
            systemroute.objectId = "tempid".ToHashGuid();
            systemroute.DestinationConstType = ConstObjectType.KoobooSystem;

            routes.Add(systemroute);

            Routing.Route actionsystemroute = new Routing.Route();
            actionsystemroute.Name = SystemRouteTemplateWithAction;
            actionsystemroute.objectId = "tempid".ToHashGuid();
            actionsystemroute.DestinationConstType = ConstObjectType.KoobooSystem;
            routes.Add(actionsystemroute);

            // add more if needed.  
            return routes;
        }
         
        public static Dictionary<string, string> ParseSystemRoute(SiteDb SiteDb, string RelativeUrl)
        {
            var route = Sites.Routing.ObjectRoute.GetRoute(SiteDb, RelativeUrl); 
            if (route != null)
            {
                return Sites.Routing.ObjectRoute.ParseParameters(route, RelativeUrl); 

            }
            return null; 
        } 
    }
}
