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
                    
                    if (foundroute != null && foundroute.objectId != default(Guid)) 
                    {
                        var foundRouteEventResult = FrontEvent.Manager.RaiseRouteEvent(FrontEvent.enumEventType.RouteFound, context.RenderContext, foundroute);


                        if (foundRouteEventResult != null && foundRouteEventResult.objectId != default(Guid))
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

        public static Route VerifyRoute(SiteDb siteDb, Route OriginalRoute)
        {
            if (OriginalRoute.DestinationConstType == ConstObjectType.Route)
            {
                int counter = 0;
                var dest = GetDestinationRoute(siteDb, OriginalRoute, ref counter);
                if (dest != null)
                {
                    if (dest.DestinationConstType == ConstObjectType.Route)
                    {
                        if (string.IsNullOrEmpty(OriginalRoute.Name) || OriginalRoute.Name == "/" || OriginalRoute.Name == "\\" || OriginalRoute.Name.StartsWith("/?"))
                        {
                            return GetDefaultRoute(siteDb);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return dest;
                    }

                }
            }
            else
            {
                return OriginalRoute;
            }
            return null;
        }

        public static Route GetDestinationRoute(SiteDb sitedb, Route OriginalRoute, ref int counter)
        {

            if (OriginalRoute.Id == OriginalRoute.objectId)
            {
                return OriginalRoute;
            }

            Route destinationroute = sitedb.Routes.Get(OriginalRoute.objectId);

            if (destinationroute != null)
            {
                if (destinationroute.DestinationConstType == ConstObjectType.Route)
                {
                    counter += 1;
                    if (counter > 5)
                    {
                        return destinationroute;
                    }
                    else
                    {
                        return GetDestinationRoute(sitedb, destinationroute, ref counter);
                    }
                }
                else
                {
                    return destinationroute;
                }
            }

            return OriginalRoute;
        }

        private static Route GetRoute(Render.FrontContext context, string url)
        {
            Route foundroute = GetRoute(context.WebSite.SiteDb(), url);

            if (foundroute == null || foundroute.objectId == default(Guid))
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
            Route newroute = new Route();
            newroute.Name = original.Name;
            newroute.DestinationConstType = original.DestinationConstType;
            newroute.objectId = original.objectId;
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
            if (temproute != null && temproute.objectId != default(Guid))
            {
                return temproute;
            }

            var routeid = sitedb.RouteTree().FindRouteId(relativeurl, true);
            if (routeid != default(Guid))
            {
                return sitedb.Routes.Get(routeid);
            } 
            return GetSystemRoute(relativeurl); 
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

            Routing.Route sysRoute = new Routing.Route();
            sysRoute.objectId = Lib.Security.Hash.ComputeGuidIgnoreCase("___tempid_fake____");
            sysRoute.DestinationConstType = ConstObjectType.KoobooSystem;

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
                byte output = 0;

                Kooboo.Data.Interface.IRepository repo = null;
                string nameorid = null;

                if (byte.TryParse(start, out output))
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
                    if (ConstTypeContainer.nameTypes.ContainsKey(start))
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


            if (sysRoute.Name != null)
            {
                return sysRoute;
            }

            return null;

        }

        public static Route GetSystemRoute(string relativeUrl, SiteDb sitedb = null)
        {
            var segs = GetSegments(relativeUrl);
            return GetSystemRoute(segs.ToList(), sitedb);
        }


        public static string[] GetSegments(string relativeurl)
        {
            int QuestionMark = relativeurl.IndexOf("?");
            if (QuestionMark > 0)
            {
                relativeurl = relativeurl.Substring(0, QuestionMark);
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

            List<string> segurl = UrlHelper.getSegments(relativeUrl, false);

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
            List<string> SourceKeys = new List<string>();
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
                    SourceKeys.Add(sourcekey);

                    var RealIndexStart = RealUrl.IndexOf(startwith, StringComparison.OrdinalIgnoreCase);

                    if (RealIndexStart == -1)
                    {
                        break;
                    }
                    if (RealIndexStart > 0)
                    {
                        string part = RealUrl.Substring(0, RealIndexStart);
                        RealKeys.Add(part);
                    }
                    if (RealUrl.Length > RealIndexStart + startwith.Length)
                    {
                        RealUrl = RealUrl.Substring(RealIndexStart + startwith.Length);
                    }
                    else
                    { RealUrl = string.Empty; }
                }
                else
                {
                    string sourcekey = ParaPattern.Substring(SourceindexStart + 1, SourceindexEnd - SourceindexStart - 1);
                    SourceKeys.Add(sourcekey);
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
                    if (leftlen > 0)
                    {
                        RealUrl = RealUrl.Substring(0, leftlen);
                    }
                    else
                    {
                        RealUrl = string.Empty;
                    }
                }

            }

            RealKeys.Add(RealUrl);

            for (int i = 0; i < SourceKeys.Count; i++)
            {
                var key = SourceKeys[i];
                var value = RealKeys[i];
                result.Add(key, value);
            }
            return result;
        }

        public static Route GetDefaultRoute(SiteDb sitedb)
        {
            var pages = sitedb.Pages.Query.Where(o => o.DefaultStart == true).SelectAll();

            foreach (var item in pages)
            {
                var route = sitedb.Routes.GetByObjectId(item.Id);
                if (route != null)
                {
                    return route;
                }
            }
            List<string> startNameConventions = new List<string>();

            var allpageroutes = sitedb.Routes.Query.Where(o => o.DestinationConstType == ConstObjectType.Page).SelectAll();
            List<Route> foundresult = new List<Route>();
            foreach (var item in allpageroutes)
            {
                if (!string.IsNullOrEmpty(item.Name))
                {
                    string lower = item.Name.ToLower();
                    if (lower.StartsWith("/index") || lower.StartsWith("/default") || lower.StartsWith("/home"))
                    {
                        foundresult.Add(item);
                    }
                }
            }

            if (foundresult.Count > 0)
            {
                return foundresult.OrderBy(o => o.Name.Length).First();
            }
            return null;
        }
    }
}
