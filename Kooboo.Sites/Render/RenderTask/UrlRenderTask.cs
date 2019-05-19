//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Extensions;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Routing;
using System.Collections.Generic;
using System;
using Kooboo.Sites.Relation;
using Kooboo.Sites.Models;

namespace Kooboo.Sites.Render
{
    public class UrlRenderTask : IRenderTask
    {
        private string Url;

        private ValueRenderTask RenderUrlValueTask { get; set; }

        private bool IsExternalLink { get; set; }

        // special, no change. 
        private bool IsSpecial { get; set; }

        public bool ClearBefore
        {
            get
            {
                return false;
            }
        }

        public UrlRenderTask(string Url)
        {
            this.Url = Url;

          this.IsSpecial = Kooboo.Sites.Service.DomUrlService.IsSpecialUrl(Url);

            if (!IsSpecial)
            {
                if (Functions.FunctionHelper.IsFunction(this.Url))
                {
                    /// might be a function.  
                    this.RenderUrlValueTask = new ValueRenderTask(this.Url); 
                }

                if (Service.DomUrlService.IsExternalLink(Url) || Url == "#")
                {
                    this.IsExternalLink = true;
                }
            }
        }

        private Dictionary<string, string> UrlPara;

        public UrlRenderTask(ValueRenderTask GetValueTask)
        {
            this.RenderUrlValueTask = GetValueTask;
        }

        public string Render(RenderContext context)
        {
            string result = string.Empty;

            if (IsSpecial)
            {
                return this.Url;
            }

            else if (this.RenderUrlValueTask != null)
            {
                result = RenderUrlValueTask.Render(context);
            }
            else
            {
                if (this.IsExternalLink || context.WebSite == null)
                {
                    result = this.Url;
                }
                else
                {
                    var route = Routing.ObjectRoute.GetRoute(context.WebSite.SiteDb(), Url);

                    if (route != null)
                    {
                        if (route.DestinationConstType == ConstObjectType.KoobooSystem)
                        {
                            result = RenderSystemLink(context, route);
                        }
                        else
                        {
                            if (UrlPara == null)
                            {
                                UrlPara = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                                var para = Kooboo.Sites.Routing.ObjectRoute.ParseParameters(route, this.Url);
                                foreach (var p in para)
                                {
                                    UrlPara[p.Key] = p.Value;
                                }
                            }

                            result = RenderPageRoute(context, route, UrlPara);
                        }
                    }
                    else
                    {
                        if (PossibleKey(Url))
                        {
                            var contextvalue = context.DataContext.GetValue(Url);
                            if (contextvalue != null)
                            {
                                result = contextvalue.ToString();
                            }
                            else
                            {
                                result = Url;
                            }
                        }
                        else
                        {
                            result = Url;
                        }
                    }
                }
            }

            result = this.PraseBracket(context, result);

            if (!IsExternalLink)
            {
                if (context.WebSite.EnableMultilingual && context.WebSite.Culture.Count > 1)
                {
                    if (context.WebSite.EnableSitePath)
                    {
                        string path = context.Request.SitePath;
                        if (string.IsNullOrEmpty(path))
                        {
                            path = context.Culture;
                        }

                        if (string.IsNullOrEmpty(path))
                        {
                            path = context.WebSite.DefaultCulture;
                        }

                        return "/" + path + result;
                    }
                    else
                    {
                        string culture = context.Culture;
                        if (!string.IsNullOrWhiteSpace(culture))
                        {
                            return Lib.Helper.UrlHelper.AppendQueryString(result, "lang", culture);
                        }
                    }
                }

            }

            return result;
        }

        private bool PossibleKey(string key)
        {
            if (key == null)
            {
                return false;
            }

            for (int i = 0; i < key.Length; i++)
            {
                var currentchar = key[i];
                if (!Lib.Helper.CharHelper.isAlphanumeric(currentchar) && currentchar != '_' && currentchar != '.')
                {
                    return false;
                }
            }
            return true;
        }

        private string RenderPageRoute(RenderContext context, Route route, Dictionary<string, string> urlpara = null)
        {
            Render.FrontContext frontContext = context.GetItem<Render.FrontContext>();

            string result;
            string tempurl = route.Name;
            Dictionary<string, string> Parameters = CopyParameters(route.Parameters);

            int questionmark = Url.IndexOf("?");

            if (questionmark > 0)
            {
                string querystring = Url.Substring(questionmark + 1);

                var namevalues = System.Web.HttpUtility.ParseQueryString(querystring);
                foreach (var item in namevalues.AllKeys)
                {
                    if (string.IsNullOrWhiteSpace(item))
                    {
                        continue;
                    }
                    string value = namevalues.Get(item);

                    Parameters[item] = value;
                }
            }

            if (urlpara != null && urlpara.Count > 0)
            {

                foreach (var item in urlpara)
                {
                    var value = urlpara[item.Key];

                    if (!DataSources.ParameterBinder.IsValueBinding(value))
                    {
                        if (Parameters.ContainsKey(item.Key))
                        {
                            var paravalue = Parameters[item.Key];

                            if (DataSources.ParameterBinder.IsValueBinding(paravalue))
                            {
                                Parameters[item.Key] = value;
                            }
                        }
                    }
                    else
                    {
                        // if (!Parameters.ContainsKey(item.Key))
                        //{
                        Parameters[item.Key] = item.Value;
                        // }
                    }

                }

            }

            result = Routing.PageRoute.GetRelativeUrl(route.Name, Parameters, frontContext);
            return result;
        }

        private Dictionary<string, string> CopyParameters(Dictionary<string, string> orginal)
        {
            Dictionary<string, string> newdict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in orginal)
            {
                newdict.Add(item.Key, item.Value);
            }
            return newdict;
        }

        private string PraseBracket(RenderContext context, string result)
        {
            if (result == null)
            {
                return null;
            }
            int start = result.IndexOf("{");
            if (start == -1)
            {
                return result;
            }

            int end = result.IndexOf("}");

            if (end > start && start > -1)
            {
                string key = result.Substring(start + 1, end - start - 1);
                object value = context.DataContext.GetValue(key);

                string strvalue = null;
                if (value != null)
                {
                    strvalue = value.ToString();
                }

                if (!string.IsNullOrEmpty(strvalue) && !strvalue.Contains("{") && !strvalue.Contains("}"))
                {
                    string replace = "{" + key + "}";

                    string newvalue = result.Replace(replace, strvalue);

                    return PraseBracket(context, newvalue);
                }
                else
                {
                    return result;
                }

            }
            else
            {
                return result;
            }
        }


        public string RenderSystemLink(RenderContext context, Routing.Route route)
        {
            Render.FrontContext frontContext = context.GetItem<Render.FrontContext>();

            route.Parameters = ObjectRoute.ParseParameters(route, this.Url);

            var constTypeString = route.Parameters.GetValue("objecttype");
            byte constType = ConstObjectType.Unknown;
            if (!byte.TryParse(constTypeString, out constType))
            {
                constType = ConstTypeContainer.GetConstType(constTypeString);
            }
            var id = route.Parameters.GetValue("nameorid");

            if (constType == ConstObjectType.View)
            {
                var view = context.WebSite.SiteDb().Views.GetByNameOrId(id);

                if (view != null)
                { 
                    var relation = GetViewPageRelation(context, view, context.WebSite.SiteDb().Log.Store.LastKey);

                    if (relation != null && relation.Count > 0)
                    {
                        var pageid = relation[0].objectXId;

                        var pageroute = context.WebSite.SiteDb().Routes.GetByObjectId(pageid);

                        if (pageroute != null)
                        {
                            return RenderPageRoute(context, pageroute);
                        }
                    }
                    /// if view was not rendered within and by the page... try to render with rendercode.  
                    if (frontContext.Page != null && frontContext.ExecutingView != null)
                    {
                        string currenturl = context.Request.RelativeUrl;

                        var values = PageRoute.GetViewParameterValues(context.WebSite.SiteDb(), view, frontContext);

                        var alternativeviewcode = Cache.ViewInSamePosition.GetAlternativeCode(frontContext.ExecutingView.Id, view.Id);

                        values.Add(SiteConstants.AlternativeViewQueryName, alternativeviewcode.ToString());
                        return Kooboo.Lib.Helper.UrlHelper.AppendQueryString(currenturl, values);
                    }

                    else if (frontContext.Page == null)
                    {
                        var values = PageRoute.GetViewParameterValues(context.WebSite.SiteDb(), view, frontContext);

                        return Kooboo.Lib.Helper.UrlHelper.AppendQueryString(this.Url, values);
                    }
                }
            }
            return null;
        }

        public void AppendResult(RenderContext context, List<RenderResult> result)
        {
            result.Add(new RenderResult() { Value = Render(context) });
        }


        // relation cache... 
        private long lastLog { get; set; } = 0; 
        public List<ObjectRelation> GetViewPageRelation(RenderContext context, View view, long LastChange)
        {
            if (LastChange != lastLog)
            {
                ViewPageRelationCache = new Dictionary<Guid, List<ObjectRelation>>(); 
            }

            if (!ViewPageRelationCache.ContainsKey(view.Id))
            {
                var relation = context.WebSite.SiteDb().Relations.GetReferredBy(view, ConstObjectType.Page);
                ViewPageRelationCache[view.Id] = relation;
                return relation; 
            }
            else
            {
                return ViewPageRelationCache[view.Id]; 
            }
        }

        public Dictionary<Guid, List<ObjectRelation>> ViewPageRelationCache { get; set; } = new Dictionary<Guid, List<ObjectRelation>>(); 

    }
}
