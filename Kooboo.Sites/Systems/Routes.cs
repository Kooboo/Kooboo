//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Extensions;
using Kooboo.Sites.Repository;
using System.Collections.Generic;

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

            Routing.Route systemroute = new Routing.Route
            {
                Name = SystemRouteTemplate,
                objectId = "tempid".ToHashGuid(),
                DestinationConstType = ConstObjectType.KoobooSystem
            };

            routes.Add(systemroute);

            Routing.Route actionsystemroute = new Routing.Route
            {
                Name = SystemRouteTemplateWithAction,
                objectId = "tempid".ToHashGuid(),
                DestinationConstType = ConstObjectType.KoobooSystem
            };
            routes.Add(actionsystemroute);

            // add more if needed.
            return routes;
        }

        public static Dictionary<string, string> ParseSystemRoute(SiteDb siteDb, string relativeUrl)
        {
            var route = Sites.Routing.ObjectRoute.GetRoute(siteDb, relativeUrl);
            return route != null ? Sites.Routing.ObjectRoute.ParseParameters(route, relativeUrl) : null;
        }
    }
}