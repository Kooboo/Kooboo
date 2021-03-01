//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Models;
using Kooboo.Dom;
using Kooboo.Dom.CSS;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Service
{
    public static class CleanerService
    {
        public static void CleanDataMethod(SiteDb sitedb)
        {
            var allmethods = sitedb.DataMethodSettings.Query.Where(o => o.IsPublic == false).SelectAll();
            var allviewmethods = sitedb.ViewDataMethods.All();
            var allmethodids = GetAllMethods(allviewmethods);

            foreach (var item in allmethods)
            {
                if (!allmethodids.Contains(item.Id))
                {
                    sitedb.DataMethodSettings.Delete(item.Id);
                }
            }
        }

        public static List<Guid> GetAllMethods(List<ViewDataMethod> viewmethods)
        {
            List<Guid> result = new List<Guid>();
            foreach (var item in viewmethods)
            {
                AddToList(item, ref result);
            }
            return result;
        }

        private static void AddToList(ViewDataMethod method, ref List<Guid> result)
        {
            if (method.MethodId != default(Guid))
            {
                result.Add(method.MethodId);
                if (method.HasChildren)
                {
                    foreach (var item in method.Children)
                    {
                        AddToList(item, ref result);
                    }
                }
            }
        }

        public static async Task<List<CmsCssRule>> GetUnusedRules(RenderContext context)
        {
            var sitedb = context.WebSite.SiteDb();
 
            context.MockData = true;
            var doms = await GetDoms(sitedb, context);

            var rules = sitedb.CssRules.All().Where(o => !o.IsInline).ToList();

            Dictionary<CmsCssRule, List<simpleSelector>> preselector = new Dictionary<CmsCssRule, List<simpleSelector>>();

            foreach (var item in rules)
            {
                if (!string.IsNullOrWhiteSpace(item.SelectorText))
                {
                    var selectorlist = SelectorParser.parseSelectorGroup(item.SelectorText);
                    if (selectorlist != null)
                    { preselector.Add(item, selectorlist); }
                }
            }

            List<CmsCssRule> unused = new List<CmsCssRule>();

            foreach (var item in preselector)
            {
                bool checkused = IsUsed(item.Value, doms); 
                if (!checkused)
                {
                    unused.Add(item.Key);
                }
            }
            return unused;
        }

        private static async Task<List<Kooboo.Dom.Document>> GetDoms(SiteDb sitedb, RenderContext context)
        {
            List<Kooboo.Dom.Document> doms = new List<Document>();

            foreach (var item in sitedb.Pages.All())
            {

                RenderContext newcontext = new RenderContext();
                newcontext.WebSite = sitedb.WebSite;  
                var html = await Kooboo.Sites.Render.PageRenderer.RenderMockAsync(newcontext, item);

                var dom = Kooboo.Dom.DomParser.CreateDom(html);

                doms.Add(dom);
            }
            return doms;
        }


        public static bool IsUsed(List<simpleSelector> selector, List<Kooboo.Dom.Document> docs)
        {
            bool itemUsed = false;
            foreach (var item in docs)
            {
                var checkresult = IsUsed(selector, item);
                if (checkresult)
                {
                    itemUsed = true;
                    break;
                }
            }
            return itemUsed;
        }


        public static bool IsUsed(List<simpleSelector> selector, Kooboo.Dom.Document doc)
        {
            var testUsed = IsMatch(doc.body, selector);
            return testUsed;
        }

        public static bool IsMatch(Kooboo.Dom.Element el, List<simpleSelector> selectors)
        {
            if (selectorMatch.Match(el, selectors))
            {
                return true;
            }

            foreach (var item in el.childNodes.item)
            {
                if (item.nodeType == Dom.enumNodeType.ELEMENT)
                {
                    var subel = item as Element;
                    var testok = IsMatch(subel, selectors);
                    if (testok)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
