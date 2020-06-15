//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Render;
using Kooboo.Sites.Models;
using Kooboo.Sites.Cache;
using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using System.Collections;

namespace Kooboo.Sites.Routing
{
    public static class PageRoute
    {
        public static void UpdatePageRouteParameter(SiteDb SiteDb, Guid PageId, List<string> Paras)
        {
            var route = SiteDb.Routes.GetByObjectId(PageId, ConstObjectType.Page);

            if (route != null)
            {
                UpdateRouteParameters(SiteDb, route, Paras);
            }
        }

        public static void UpdatePageRouteParameter(SiteDb SiteDb, Guid PageId)
        {
            HashSet<string> paras = new HashSet<string>(); 
             
            var relativeViews = SiteDb.Relations.GetRelations(PageId, ConstObjectType.View);
            foreach (var item in relativeViews)
            {
                var viewparas = GetViewParameters(SiteDb, item.objectYId);
                foreach (var para in viewparas)
                {
                    if (!paras.Contains(para))
                    {
                        paras.Add(para);
                    }
                }
            }

            var page = SiteDb.Pages.Get(PageId);
            if (page !=null && page.RequestParas !=null)
            {
                foreach (var item in page.RequestParas)
                {
                    paras.Add(item); 
                }
            }

            UpdatePageRouteParameter(SiteDb, PageId, paras.ToList());
        }

        public static void UpdateRouteParameters(SiteDb SiteDb, Route Route, List<string> NewParas)
        {
            Dictionary<string, string> DictNewParas = new Dictionary<string, string>();
            foreach (var item in NewParas)
            {
                var key = Sites.DataSources.ParameterBinder.GetBindingKey(item); 
                if (string.IsNullOrWhiteSpace(key))
                {
                    key = item; 
                }
                DictNewParas.Add(key, item);
            }

            bool haschange = false;
            if (Route == null)
            {
                return;
            }
            // remote old ones.
            List<string> toberemoved = new List<string>();
            foreach (var item in Route.Parameters)
            {
                if (!DictNewParas.ContainsKey(item.Key))
                {
                    toberemoved.Add(item.Key);
                }
            }

            foreach (var item in toberemoved)
            {
                Route.Parameters.Remove(item);
                /// TODO: check whether need to remove items form the route name or not...
                haschange = true;
            }

            foreach (var item in DictNewParas)
            {
                if (!Route.Parameters.ContainsKey(item.Key))
                {
                    Route.Parameters.Add(item.Key, item.Value);
                    haschange = true;
                }
                else
                {
                    if (Route.Parameters[item.Key] != item.Value)
                    {
                        Route.Parameters[item.Key] = item.Value;
                        haschange = true;
                    }

                }
            }

            if (haschange)
            {
                SiteDb.Routes.AddOrUpdate(Route);
            }

        }

        public static List<string> GetViewParameters(SiteDb sitedb, Guid ViewId)
        {
            HashSet<string> paras = new HashSet<string>();

            var datamethods = Cache.SiteObjectCache<ViewDataMethod>.List(sitedb).Where(o => o.ViewId == ViewId).ToList();

            if (datamethods != null)
            {
                foreach (var item in datamethods)
                {
                    var method = sitedb.DataMethodSettings.Get(item.MethodId);

                    if (method == null)
                    {
                        method = Data.GlobalDb.DataMethodSettings.Get(item.MethodId);
                    }

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
            var view = sitedb.Views.Get(ViewId);
            if (view != null && view.RequestParas != null)
            {
                foreach (var item in view.RequestParas)
                {
                    paras.Add(item);
                } 
            }

            return paras.ToList();
        }

        public static string GetRelativeUrl(string RelativeUrl, Render.FrontContext context)
        {
            var route = ObjectRoute.GetRoute(context.SiteDb, RelativeUrl);
            if (route == null)
            {
                return RelativeUrl;
            }

            return GetRelativeUrl(route, context);
        }

        public static string GetRelativeUrl(Route route, FrontContext context)
        {
            return GetRelativeUrl(route.Name, route.Parameters, context);
        }

        public static string GetRelativeUrl(string RouteName, Dictionary<string, string> Parameters, FrontContext context)
        {
            if (Parameters == null || Parameters.Count == 0)
            {
                return RouteName;
            }

            Dictionary<string, string> querystring = new Dictionary<string, string>();

            foreach (var item in Parameters)
            {
                string key = item.Key;
                string value = item.Value;
                string expressionkey = value;

                object ValueResult = GetValueFromContext(key, value, context.RenderContext);

                if (ValueResult != null)
                {
                    if (RouteName.IndexOf("{" + key + "}", StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        RouteName = Lib.Helper.StringHelper.ReplaceIgnoreCase(RouteName, "{" + key + "}", ValueResult.ToString());
                    }
                    else
                    {
                        querystring[key] = ValueResult.ToString();
                    }
                }
            }

            if (querystring.Count > 0)
            {
                RouteName = Kooboo.Lib.Helper.UrlHelper.AppendQueryString(RouteName, querystring);
            }

            return RouteName;
        }

        public static Dictionary<string, string> GetViewParameterValues(SiteDb SiteDb, View view, FrontContext Context)
        {
            Dictionary<string, string> result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (view == null)
            {
                return result; 
            }

            var datamethods = Cache.SiteObjectCache<ViewDataMethod>.List(SiteDb).Where(o => o.ViewId == view.Id).ToList();

            if (datamethods != null)
            {
                foreach (var item in datamethods)
                {

                    var method = SiteDb.DataMethodSettings.Get(item.MethodId);

                    if (method == null)
                    {
                        method = Data.GlobalDb.DataMethodSettings.Get(item.MethodId);
                    }

                    if (method != null)
                    {
                        foreach (var eachbinding in method.ParameterBinding)
                        {
                            if (DataSources.ParameterBinder.IsValueBinding(eachbinding.Value.Binding))
                            {
                                if (!result.ContainsKey(eachbinding.Key))
                                {
                                    object ValueResult = GetValueFromContext(eachbinding.Key, eachbinding.Value.Binding, Context.RenderContext);

                                    if (ValueResult != null)
                                    {
                                        result[eachbinding.Key] = ValueResult.ToString();
                                    }

                                }
                            }
                        }
                    }
                }
            }
             

            if (view !=null && view.RequestParas !=null)
            {
                foreach (var p in view.RequestParas)
                {
                    var pkey = p; 
                    if (pkey.IndexOf("}")>-1)
                    {
                        pkey = Kooboo.Sites.DataSources.ParameterBinder.GetBindingKey(pkey); 
                    }

                    if (!result.ContainsKey(pkey))
                    {
                        object ValueResult = GetValueFromContext(pkey, "{" + pkey + "}", Context.RenderContext);

                        if (ValueResult != null)
                        {
                            result[pkey] = ValueResult.ToString();
                        } 
                    } 
              
                }
            }
            
            return result;
        }

        public static object GetValueFromContext(string Key, string Value, RenderContext   context)
        {

            //if (!DataSources.ParameterBinder.IsValueBinding(Value))
            //{
            //    return Value;
            //}
            //object ValueResult = null;

            //string expression = DataSources.ParameterBinder.GetBindingKey(Value);

            //// rule 1, get the value directly from context.. 
            //ValueResult = context.RenderContext.DataContext.GetValue(expression);

            //// TODO: this is new method... should be replaced by this only in the near future... 
            //if (ValueResult == null)
            //{
            //    ValueResult = RenderContextHelper.GetValue(Key, Value, context.RenderContext);
            //}

           var ValueResult = RenderContextHelper.GetValue(Key, Value, context);

            //if (ValueResult == null)
            //{
            //    // rule 2, get the value as field value from all data objects... 
            //    // first check the value that has the same type as the datamethod return type.
            //    ValueResult = GetValueByMethodReturnType(Key, Value, context);
            //}

            //if (ValueResult == null)
            //{
            //    ValueResult = GetValueByNamingConvention(Key, Value, context);
            //}

            // last try to id alternative. 
            if (ValueResult == null)
            {
                //check for id == _id convertion. 
                string lower = Key.ToLower();
                if (lower == "_id")
                {
                    ValueResult = context.DataContext.GetValue("id"); 
                    if (ValueResult == null)
                    {
                        ValueResult = RenderContextHelper.GetValue("{id}", Value, context);
                    }
                }
                else if (lower == "id")
                {
                    ValueResult = context.DataContext.GetValue("_id");
                    if (ValueResult == null)
                    {
                        ValueResult = RenderContextHelper.GetValue("{_id}", Value, context);
                    }
                }
            }
            
            return ValueResult;
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
        public static object GetValue(string Key, string Value, RenderContext context)
        {

            if (!DataSources.ParameterBinder.IsValueBinding(Value))
            {
                return Value;
            }
            object ValueResult = null;

            string expression = DataSources.ParameterBinder.GetBindingKey(Value);

            // rule 1, get the value directly from context.. 
            ValueResult = context.DataContext.GetValue(expression);

            if (ValueResult == null)
            {
                // rule 2, get the value as field value from all data objects... 
                // first check the value that has the same type as the datamethod return type.
                ValueResult = ByMethodReturnType(Key, Value, context);
            }

            if (ValueResult == null)
            {
                ValueResult = ByNamingConvention(Key, Value, context);
            }

            return ValueResult;
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

            if (frontcontext == null)
            {
                return null; 
            }

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
