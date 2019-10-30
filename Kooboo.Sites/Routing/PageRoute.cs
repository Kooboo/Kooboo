//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Sites.Cache;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Render;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Routing
{
    public static class PageRoute
    {
        public static void UpdatePageRouteParameter(SiteDb siteDb, Guid pageId, List<string> paras)
        {
            var route = siteDb.Routes.GetByObjectId(pageId, ConstObjectType.Page);

            if (route != null)
            {
                UpdateRouteParameters(siteDb, route, paras);
            }
        }

        public static void UpdatePageRouteParameter(SiteDb siteDb, Guid pageId)
        {
            HashSet<string> paras = new HashSet<string>();

            var relativeViews = siteDb.Relations.GetRelations(pageId, ConstObjectType.View);
            foreach (var item in relativeViews)
            {
                var viewparas = GetViewParameters(siteDb, item.objectYId);
                foreach (var para in viewparas)
                {
                    if (!paras.Contains(para))
                    {
                        paras.Add(para);
                    }
                }
            }

            var page = siteDb.Pages.Get(pageId);
            if (page?.RequestParas != null)
            {
                foreach (var item in page.RequestParas)
                {
                    paras.Add(item);
                }
            }

            UpdatePageRouteParameter(siteDb, pageId, paras.ToList());
        }

        public static void UpdateRouteParameters(SiteDb siteDb, Route route, List<string> newParas)
        {
            Dictionary<string, string> dictNewParas = new Dictionary<string, string>();
            foreach (var item in newParas)
            {
                var key = Sites.DataSources.ParameterBinder.GetBindingKey(item);
                if (string.IsNullOrWhiteSpace(key))
                {
                    key = item;
                }
                dictNewParas.Add(key, item);
            }

            bool haschange = false;
            if (route == null)
            {
                return;
            }
            // remote old ones.
            List<string> toberemoved = new List<string>();
            foreach (var item in route.Parameters)
            {
                if (!dictNewParas.ContainsKey(item.Key))
                {
                    toberemoved.Add(item.Key);
                }
            }

            foreach (var item in toberemoved)
            {
                route.Parameters.Remove(item);
                // TODO: check whether need to remove items form the route name or not...
                haschange = true;
            }

            foreach (var item in dictNewParas)
            {
                if (!route.Parameters.ContainsKey(item.Key))
                {
                    route.Parameters.Add(item.Key, item.Value);
                    haschange = true;
                }
                else
                {
                    if (route.Parameters[item.Key] != item.Value)
                    {
                        route.Parameters[item.Key] = item.Value;
                        haschange = true;
                    }
                }
            }

            if (haschange)
            {
                siteDb.Routes.AddOrUpdate(route);
            }
        }

        public static List<string> GetViewParameters(SiteDb sitedb, Guid viewId)
        {
            HashSet<string> paras = new HashSet<string>();

            var datamethods = Cache.SiteObjectCache<ViewDataMethod>.List(sitedb).Where(o => o.ViewId == viewId).ToList();

            if (datamethods != null)
            {
                foreach (var item in datamethods)
                {
                    var method = sitedb.DataMethodSettings.Get(item.MethodId) ?? Data.GlobalDb.DataMethodSettings.Get(item.MethodId);

                    if (method != null)
                    {
                        foreach (var eachbinding in method.ParameterBinding)
                        {
                            if (DataSources.ParameterBinder.IsValueBinding(eachbinding.Value.Binding))
                            {
                                paras.Add(eachbinding.Value.Binding);
                            }
                        }
                    }
                }
            }

            // add the kscript part into the parameters.
            var view = sitedb.Views.Get(viewId);
            if (view?.RequestParas != null)
            {
                foreach (var item in view.RequestParas)
                {
                    paras.Add(item);
                }
            }

            return paras.ToList();
        }

        public static string GetRelativeUrl(string relativeUrl, Render.FrontContext context)
        {
            var route = ObjectRoute.GetRoute(context.SiteDb, relativeUrl);
            if (route == null)
            {
                return relativeUrl;
            }

            return GetRelativeUrl(route, context);
        }

        public static string GetRelativeUrl(Route route, FrontContext context)
        {
            return GetRelativeUrl(route.Name, route.Parameters, context);
        }

        public static string GetRelativeUrl(string routeName, Dictionary<string, string> parameters, FrontContext context)
        {
            if (parameters == null || parameters.Count == 0)
            {
                return routeName;
            }

            Dictionary<string, string> querystring = new Dictionary<string, string>();

            foreach (var item in parameters)
            {
                string key = item.Key;
                string value = item.Value;
                string expressionkey = value;

                object ValueResult = GetValueFromContext(key, value, context);

                if (ValueResult != null)
                {
                    if (routeName.IndexOf("{" + key + "}", StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        routeName = Lib.Helper.StringHelper.ReplaceIgnoreCase(routeName, "{" + key + "}", ValueResult.ToString());
                    }
                    else
                    {
                        querystring[key] = ValueResult.ToString();
                    }
                }
            }

            if (querystring.Count > 0)
            {
                routeName = Kooboo.Lib.Helper.UrlHelper.AppendQueryString(routeName, querystring);
            }

            return routeName;
        }

        public static Dictionary<string, string> GetViewParameterValues(SiteDb siteDb, View view, FrontContext context)
        {
            Dictionary<string, string> result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (view == null)
            {
                return result;
            }

            var datamethods = Cache.SiteObjectCache<ViewDataMethod>.List(siteDb).Where(o => o.ViewId == view.Id).ToList();

            if (datamethods != null)
            {
                foreach (var item in datamethods)
                {
                    var method = siteDb.DataMethodSettings.Get(item.MethodId) ?? Data.GlobalDb.DataMethodSettings.Get(item.MethodId);

                    if (method != null)
                    {
                        foreach (var eachbinding in method.ParameterBinding)
                        {
                            if (DataSources.ParameterBinder.IsValueBinding(eachbinding.Value.Binding))
                            {
                                if (!result.ContainsKey(eachbinding.Key))
                                {
                                    object valueResult = GetValueFromContext(eachbinding.Key, eachbinding.Value.Binding, context);

                                    if (valueResult != null)
                                    {
                                        result[eachbinding.Key] = valueResult.ToString();
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (view?.RequestParas != null)
            {
                foreach (var p in view.RequestParas)
                {
                    var pkey = p;
                    if (pkey.IndexOf("}") > -1)
                    {
                        pkey = Kooboo.Sites.DataSources.ParameterBinder.GetBindingKey(pkey);
                    }

                    if (!result.ContainsKey(pkey))
                    {
                        object valueResult = GetValueFromContext(pkey, "{" + pkey + "}", context);

                        if (valueResult != null)
                        {
                            result[pkey] = valueResult.ToString();
                        }
                    }
                }
            }

            return result;
        }

        private static object GetValueFromContext(string key, string value, FrontContext context)
        {
            if (!DataSources.ParameterBinder.IsValueBinding(value))
            {
                return value;
            }

            string expression = DataSources.ParameterBinder.GetBindingKey(value);

            // rule 1, get the value directly from context..
            object valueResult = context.RenderContext.DataContext.GetValue(expression) ?? RenderContextHelper.GetValue(key, value, context.RenderContext);

            // TODO: this is new method... should be replaced by this only in the near future...

            if (valueResult == null)
            {
                // rule 2, get the value as field value from all data objects...
                // first check the value that has the same type as the datamethod return type.
                valueResult = GetValueByMethodReturnType(key, value, context);
            }

            if (valueResult == null)
            {
                valueResult = GetValueByNamingConvention(key, value, context);
            }

            // last try to id alternative.
            if (valueResult == null)
            {
                //check for id == _id convertion.
                string lower = expression.ToLower();
                if (lower == "_id")
                {
                    valueResult = context.RenderContext.DataContext.GetValue("id");
                    if (valueResult == null)
                    {
                        valueResult = RenderContextHelper.GetValue("{id}", value, context.RenderContext);
                    }
                }
                else if (lower == "id")
                {
                    valueResult = context.RenderContext.DataContext.GetValue("_id");
                    if (valueResult == null)
                    {
                        valueResult = RenderContextHelper.GetValue("{_id}", value, context.RenderContext);
                    }
                }
            }

            return valueResult;
        }

        /// <summary>
        /// For example, Id ==> ReturnType(News).Id;
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private static object GetValueByMethodReturnType(string key, string value, FrontContext context)
        {
            string bindingkey = "{" + key + "}";

            string expression = DataSources.ParameterBinder.GetBindingKey(value);

            object result = null;

            var viewmethods = context.ViewDataMethods;
            foreach (var item in viewmethods)
            {
                var method = CompileMethodCache.GetCompiledMethod(context.SiteDb, item.MethodId);
                bool hasmatch = false;
                foreach (var methodparaitem in method.ParameterBindings)
                {
                    if (methodparaitem.Value.Binding == bindingkey)
                    {
                        hasmatch = true;
                        break;
                    }
                }

                if (!hasmatch)
                {
                    continue;
                }

                var returntype = method.ReturnType;

                string newkey;
                int dotindex = expression.IndexOf(".");

                if (dotindex > 0)
                {
                    newkey = returntype.Name + expression.Substring(dotindex);
                }
                else
                {
                    newkey = returntype.Name + "." + expression;
                }
                result = context.RenderContext.DataContext.GetValue(newkey);
                if (result != null)
                {
                    return result;
                }

                dotindex = key.IndexOf(".");

                if (dotindex > 0)
                {
                    newkey = returntype.Name + key.Substring(dotindex);
                }
                else
                {
                    newkey = returntype.Name + "." + key;
                }
                result = context.RenderContext.DataContext.GetValue(newkey);

                if (result != null)
                {
                    return result;
                }
            }

            return result;
        }

        /// <summary>
        /// Change ProductName into Product.Name to get the product information..
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private static object GetValueByNamingConvention(string key, string value, FrontContext context)
        {
            // in the case of contains ., it is already
            if (key.Contains("."))
            {
                return null;
            }

            int length = key.Length;
            for (int i = 1; i < length; i++)
            {
                if (Kooboo.Lib.Helper.CharHelper.isUppercaseAscii(key[i]))
                {
                    string main = key.Substring(0, i);
                    string sub = key.Substring(i);
                    string newkey = main + "." + sub;
                    var result = context.RenderContext.DataContext.GetValue(newkey);
                    if (result != null)
                    {
                        return result;
                    }
                    newkey = sub;
                    result = context.RenderContext.DataContext.GetValue(newkey);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }
    }

    public static class RenderContextHelper
    {
        public static object GetValue(string key, string value, RenderContext context)
        {
            if (!DataSources.ParameterBinder.IsValueBinding(value))
            {
                return value;
            }
            object valueResult = null;

            string expression = DataSources.ParameterBinder.GetBindingKey(value);

            // rule 1, get the value directly from context..
            valueResult = context.DataContext.GetValue(expression);

            if (valueResult == null)
            {
                // rule 2, get the value as field value from all data objects...
                // first check the value that has the same type as the datamethod return type.
                valueResult = ByMethodReturnType(key, value, context);
            }

            if (valueResult == null)
            {
                valueResult = ByNamingConvention(key, value, context);
            }

            return valueResult;
        }

        /// <summary>
        /// For example, Id ==> ReturnType(News).Id;
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private static object ByMethodReturnType(string key, string value, RenderContext context)
        {
            string bindingkey = "{" + key + "}";

            string expression = DataSources.ParameterBinder.GetBindingKey(value);

            var frontcontext = context.GetItem<FrontContext>();

            object result = null;

            var viewmethods = frontcontext.ViewDataMethods;
            foreach (var item in viewmethods)
            {
                var method = CompileMethodCache.GetCompiledMethod(context.WebSite.SiteDb(), item.MethodId);
                bool hasmatch = false;
                foreach (var methodparaitem in method.ParameterBindings)
                {
                    if (methodparaitem.Value.Binding == bindingkey)
                    {
                        hasmatch = true;
                        break;
                    }
                }

                if (!hasmatch)
                {
                    continue;
                }

                var returntype = method.ReturnType;

                string newkey;
                int dotindex = expression.IndexOf(".");

                if (dotindex > 0)
                {
                    newkey = returntype.Name + expression.Substring(dotindex);
                }
                else
                {
                    newkey = returntype.Name + "." + expression;
                }
                result = context.DataContext.GetValue(newkey);
                if (result != null)
                {
                    return result;
                }

                dotindex = key.IndexOf(".");

                if (dotindex > 0)
                {
                    newkey = returntype.Name + key.Substring(dotindex);
                }
                else
                {
                    newkey = returntype.Name + "." + key;
                }
                result = context.DataContext.GetValue(newkey);

                if (result != null)
                {
                    return result;
                }
            }
            return result;
        }

        /// <summary>
        /// Change ProductName into Product.Name to get the product information..
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private static object ByNamingConvention(string key, string value, RenderContext context)
        {
            // in the case of contains ., it is already
            if (key.Contains("."))
            {
                return null;
            }

            int length = key.Length;
            for (int i = 1; i < length; i++)
            {
                if (Kooboo.Lib.Helper.CharHelper.isUppercaseAscii(key[i]))
                {
                    string main = key.Substring(0, i);
                    string sub = key.Substring(i);
                    string newkey = main + "." + sub;
                    var result = context.DataContext.GetValue(newkey);
                    if (result != null)
                    {
                        return result;
                    }
                    newkey = sub;
                    result = context.DataContext.GetValue(newkey);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }
    }
}