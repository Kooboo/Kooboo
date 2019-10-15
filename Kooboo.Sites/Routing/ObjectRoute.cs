//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Lib.Helper;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Routing
{
    public static class ObjectRoute
    {
        public static void Parse(Render.FrontContext context)
        {
            Route foundroute = null;

            var nameOrId = string.Empty;
            if (context.RenderContext.WebSite.EnableFrontEvents && context.RenderContext.IsSiteBinding)
            {
                foundroute = FrontEvent.Manager.RaiseRouteEvent(FrontEvent.enumEventType.RouteFinding, context.RenderContext);

                if (foundroute == null)
                {
                    foundroute = GetRoute(context, context.RenderContext.Request.RelativeUrl);
                    if (foundroute == null)
                    {
                        var relativeUrl = context.RenderContext.Request.RelativeUrl;
                        int questionMarkIndex = relativeUrl.IndexOf("?");
                        if (questionMarkIndex>0)
                        {
                            relativeUrl = relativeUrl.Substring(0, questionMarkIndex);
                        }
                        var lastSlashIndex= relativeUrl.LastIndexOf("/");
                        if (lastSlashIndex > -1)
                        {
                            var url = relativeUrl.Substring(0, lastSlashIndex);
                            nameOrId = relativeUrl.Substring(lastSlashIndex + 1);
                            foundroute = GetRoute(context, url);
                        }
                        
                    }
                    if (foundroute != null && foundroute.objectId != default)
                    // if (foundroute != null)
                    {
                        var foundRouteEventResult = FrontEvent.Manager.RaiseRouteEvent(FrontEvent.enumEventType.RouteFound, context.RenderContext, foundroute);


                        if (foundRouteEventResult != null && foundRouteEventResult.objectId != default)
                        {
                            foundroute = foundRouteEventResult;
                        }

                    }
                    else
                    {
                        foundroute = FrontEvent.Manager.RaiseRouteEvent(FrontEvent.enumEventType.RouteNotFound, context.RenderContext, null);
                    }
                }
            }

            if (foundroute == null)
            {
                foundroute = GetRoute(context, context.RenderContext.Request.RelativeUrl);
            }

            if (foundroute == null)
            {
                return;
            }

            foundroute = VerifyRoute(context.SiteDb, foundroute);

            if (foundroute == null)
            {
                return;
            }

            var newroute = CopyRouteWithoutParameter(foundroute);

            newroute.Parameters = ParseParameters(foundroute, context.RenderContext.Request.RelativeUrl);
            if (!string.IsNullOrEmpty(nameOrId))
            {
                newroute.Parameters.Add("nameOrId", nameOrId);
            }
            context.Route = newroute;
            context.Log.ConstType = foundroute.DestinationConstType;
            context.Log.ObjectId = foundroute.objectId;
        }

        public static Route VerifyRoute(SiteDb siteDb, Route originalRoute)
        {
            if (originalRoute.DestinationConstType == ConstObjectType.Route)
            {
                int counter = 0;
                var dest = GetDestinationRoute(siteDb, originalRoute, ref counter);
                if (dest != null)
                {
                    if (dest.DestinationConstType == ConstObjectType.Route)
                    {
                        if (string.IsNullOrEmpty(originalRoute.Name) || originalRoute.Name == "/" || originalRoute.Name == "\\" || originalRoute.Name.StartsWith("/?"))
                        {
                            return GetDefaultRoute(siteDb);
                        }

                        return null;
                    }

                    return dest;

                }
            }
            else
            {
                return originalRoute;
            }
            return null;
        }

        public static Route GetDestinationRoute(SiteDb sitedb, Route originalRoute, ref int counter)
        {

            if (originalRoute.Id == originalRoute.objectId)
            {
                return originalRoute;
            }

            Route destinationroute = sitedb.Routes.Get(originalRoute.objectId);

            if (destinationroute != null)
            {
                if (destinationroute.DestinationConstType == ConstObjectType.Route)
                {
                    counter += 1;
                    return counter > 5 ? destinationroute : GetDestinationRoute(sitedb, destinationroute, ref counter);
                }

                return destinationroute;
            }

            return originalRoute;
        }

        private static Route GetRoute(Render.FrontContext context, string url)
        {
            Route foundroute = GetRoute(context.WebSite.SiteDb(), url);

            if (foundroute == null || foundroute.objectId == default)
            {
                if (string.IsNullOrEmpty(url) || url == "/" || url == "\\" || url.StartsWith("/?"))
                {
                    var route = GetDefaultRoute(context.RenderContext.WebSite.SiteDb());
                    if (route != null)
                    {
                        foundroute = route;
                        context.RenderContext.Request.RelativeUrl = foundroute.Name;
                    }
                }
            }

            return foundroute;
        }

        private static Route CopyRouteWithoutParameter(Route original)
        {
            Route newroute = new Route
            {
                Name = original.Name,
                DestinationConstType = original.DestinationConstType,
                objectId = original.objectId
            };
            return newroute;
        }

        private static Dictionary<string, string> PraseQueryString(string queryString)
        {
            var parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (!string.IsNullOrEmpty(queryString))
            {
                var array = queryString.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in array)
                {
                    var keyValuePairs = item.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    if (keyValuePairs.Length == 1)
                    {
                        parameters[keyValuePairs[0]] = String.Empty;
                    }
                    else if (keyValuePairs.Length == 2)
                    {
                        parameters[keyValuePairs[0]] = keyValuePairs[1];
                    }
                }
            }
            return parameters;
        }

        public static Route GetRoute(SiteDb sitedb, string relativeurl)
        {
            var tempid = Kooboo.Data.IDGenerator.GetRouteId(relativeurl);
            var temproute = sitedb.Routes.GetFromCache(tempid);
            if (temproute != null && temproute.objectId != default)
            {
                return temproute;
            }

            var routeid = sitedb.RouteTree().FindRouteId(relativeurl, true);
            if (routeid != default)
            {
                return sitedb.Routes.Get(routeid);
            }

            // find system route...  
            return GetSystemRoute(relativeurl);
            // return null;
        }

        // if other routes does not match... find the system route. 
        public static Route GetSystemRoute(List<String> segments, SiteDb sitedb = null)
        {
            // public static string SystemRouteTemplate = "/__kb/{objecttype}/{nameorid}"; /{objecttype}/{nameorid}
            // public static string SystemRouteTemplateWithAction = "/__kb/{objecttype}/{nameorid}/{action}";  //{objecttype}/{nameorid}/{action}

            //Routing.Route actionsystemroute = new Routing.Route();
            //actionsystemroute.Name = SystemRouteTemplateWithAction;
            //actionsystemroute.objectId = "tempid".ToHashGuid();
            //actionsystemroute.DestinationConstType = ConstObjectType.KoobooSystem;
            //routes.Add(actionsystemroute); 

            if (segments == null || segments.Count < 2)
            {
                return null;
            }

            string routename = string.Empty;

            var start = segments[0];

            Routing.Route sysRoute = new Routing.Route
            {
                objectId = Lib.Security.Hash.ComputeGuidIgnoreCase("___tempid_fake____"),
                DestinationConstType = ConstObjectType.KoobooSystem
            };

            if (start == "__kb")
            {
                if (segments.Count == 3)
                {
                    sysRoute.Name = "/__kb/{objecttype}/{nameorid}";
                }
                else if (segments.Count() == 4)
                {
                    sysRoute.Name = "/__kb/{objecttype}/{nameorid}/{action}";
                }
                else if (segments.Count() > 4)
                {
                    sysRoute.Name = "/__kb/{objecttype}/{path}";
                }
            }
            else
            {
                Kooboo.Data.Interface.IRepository repo = null;
                string nameorid = null;

                if (byte.TryParse(start, out var output))
                {
                    if (ConstTypeContainer.ByteTypes.ContainsKey(output))
                    {
                        if (sitedb != null)
                        {
                            repo = sitedb.GetRepository(output);
                        }

                        if (segments.Count == 2)
                        {
                            sysRoute.Name = "/{objecttype}/{nameorid}";
                            nameorid = segments[1];
                        }
                        else if (segments.Count() == 3)
                        {
                            sysRoute.Name = "/{objecttype}/{nameorid}/{action}";
                            nameorid = segments[1];
                        }
                    }
                }
                else
                {
                    if (ConstTypeContainer.NameTypes.ContainsKey(start))
                    {
                        if (sitedb != null)
                        {
                            repo = sitedb.GetRepository(start);
                        }

                        if (segments.Count == 2)
                        {
                            sysRoute.Name = "/{objecttype}/{nameorid}";
                            nameorid = segments[1];
                        }
                        else if (segments.Count() == 3)
                        {
                            sysRoute.Name = "/{objecttype}/{nameorid}/{action}";
                            nameorid = segments[1];
                        }
                    }
                }

                // this kind of route require one more check. 
                if (sitedb != null && repo != null && nameorid != null)
                {
                    var siteobj = repo.GetByNameOrId(nameorid);
                    if (siteobj == null)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }

            }


            return sysRoute.Name != null ? sysRoute : null;
        }

        public static Route GetSystemRoute(string relativeUrl, SiteDb sitedb = null)
        {
            var segs = GetSegments(relativeUrl);
            return GetSystemRoute(segs.ToList(), sitedb);
        }


        public static string[] GetSegments(string relativeurl)
        {
            int questionMark = relativeurl.IndexOf("?");
            if (questionMark > 0)
            {
                relativeurl = relativeurl.Substring(0, questionMark);
            }

            relativeurl = relativeurl.Replace("\\", "/").ToLower();

            var sep = "/".ToCharArray();

            return relativeurl.Split(sep, StringSplitOptions.RemoveEmptyEntries);
        }


        public static Dictionary<string, string> ParseParameters(Route route, string relativeUrl)
        {
            int indexmark = relativeUrl.IndexOf("?");
            Dictionary<string, string> paras = new Dictionary<string, string>();
            if (indexmark > 0)
            {
                string parastring = relativeUrl.Substring(indexmark + 1);
                paras = PraseQueryString(parastring);

                relativeUrl = relativeUrl.Substring(0, indexmark);
            }

            Dictionary<string, string> parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            List<string> segroute = UrlHelper.getSegments(route.Name);

            List<string> segurl = UrlHelper.getSegments(relativeUrl);

            int maxi = segroute.Count;
            if (segurl.Count < maxi)
            {
                maxi = segurl.Count;
            }

            for (int i = 0; i < maxi; i++)
            {
                string para = segroute[i];
                string paravalue = segurl[i];

                if (!string.IsNullOrEmpty(para))
                {
                    var dict = GetParameterValues(para, paravalue);
                    if (dict != null)
                    {
                        foreach (var item in dict)
                        {
                            parameters[item.Key] = item.Value;
                        }
                    }
                }
            }

            foreach (var item in paras)
            {
                parameters[item.Key] = item.Value;
            }

            return parameters;
        }

        public static Dictionary<string, string> GetParameterValues(string ParaPattern, string RealUrl)
        {
            if (string.IsNullOrEmpty(ParaPattern) || string.IsNullOrEmpty(RealUrl))
            {
                return null;
            }
            List<string> sourceKeys = new List<string>();
            List<string> RealKeys = new List<string>();

            int sourceposition = 0;
            int SourceindexStart = ParaPattern.IndexOf("{");
            int SourceindexEnd = ParaPattern.IndexOf("}");

            if (SourceindexStart == 1)
            {
                return null;
            }

            Dictionary<string, string> result = new Dictionary<string, string>();

            while (SourceindexStart >= 0 && SourceindexEnd >= 0)
            {
                string startwith = null;
                int len = SourceindexStart - sourceposition;
                if (len < 0)
                {
                    break;
                }

                if (len > 0)
                {
                    startwith = ParaPattern.Substring(sourceposition, len);

                    string sourcekey = ParaPattern.Substring(SourceindexStart + 1, SourceindexEnd - SourceindexStart - 1);
                    sourceKeys.Add(sourcekey);

                    var realIndexStart = RealUrl.IndexOf(startwith, StringComparison.OrdinalIgnoreCase);

                    if (realIndexStart == -1)
                    {
                        break;
                    }
                    if (realIndexStart > 0)
                    {
                        string part = RealUrl.Substring(0, realIndexStart);
                        RealKeys.Add(part);
                    }
                    if (RealUrl.Length > realIndexStart + startwith.Length)
                    {
                        RealUrl = RealUrl.Substring(realIndexStart + startwith.Length);
                    }
                    else
                    { RealUrl = string.Empty; }
                }
                else
                {
                    string sourcekey = ParaPattern.Substring(SourceindexStart + 1, SourceindexEnd - SourceindexStart - 1);
                    sourceKeys.Add(sourcekey);
                }

                sourceposition = SourceindexEnd + 1;
                SourceindexStart = ParaPattern.IndexOf("{", sourceposition);
                SourceindexEnd = ParaPattern.IndexOf("}", sourceposition);

            }

            if (ParaPattern.Length > sourceposition)
            {
                string endstring = ParaPattern.Substring(sourceposition, ParaPattern.Length - sourceposition);

                if (RealUrl.EndsWith(endstring, StringComparison.OrdinalIgnoreCase))
                {
                    int leftlen = RealUrl.Length - endstring.Length;
                    RealUrl = leftlen > 0 ? RealUrl.Substring(0, leftlen) : string.Empty;
                }

            }

            RealKeys.Add(RealUrl);

            for (int i = 0; i < sourceKeys.Count; i++)
            {
                var key = sourceKeys[i];
                var value = RealKeys[i];
                result.Add(key, value);
            }
            return result;
        }

        public static Route GetDefaultRoute(SiteDb sitedb)
        {
            var pages = sitedb.Pages.Query.Where(o => o.DefaultStart).SelectAll();

            foreach (var route in pages.Select(item => sitedb.Routes.GetByObjectId(item.Id)).Where(route => route != null))
            {
                return route;
            }
            List<string> startNameConventions = new List<string>();

            var allpageroutes = sitedb.Routes.Query.Where(o => o.DestinationConstType == ConstObjectType.Page).SelectAll();
            List<Route> foundresult = (from item in allpageroutes where !string.IsNullOrEmpty(item.Name) let lower = item.Name.ToLower() where lower.StartsWith("/index") || lower.StartsWith("/default") || lower.StartsWith("/home") select item).ToList();

            return foundresult.Count > 0 ? foundresult.OrderBy(o => o.Name.Length).First() : null;
        }
    }
}
