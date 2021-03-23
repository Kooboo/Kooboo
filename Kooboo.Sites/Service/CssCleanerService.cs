using Kooboo.Data.Context;
using Kooboo.Dom;
using Kooboo.Dom.CSS;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kooboo.Sites.Service
{
    public static class CssCleanerService
    {
        public static async Task<List<CmsCssRule>> GetUnusedRules(RenderContext context)
        {
            var sitedb = context.WebSite.SiteDb();

            context.MockData = true;
            var doms = await GetDoms(sitedb, context);
            var JsCssNames = JsClassList(sitedb);

            var rules = sitedb.CssRules.All().Where(o => !o.IsInline).ToList();
            var styles = sitedb.Styles.All();

            List<StyleRules> prerules = new List<StyleRules>();

            var groupby = rules.GroupBy(o => o.ParentStyleId).ToList();
            foreach (var item in groupby)
            {
                var styleid = item.Key;
                var style = sitedb.Styles.Get(styleid);
                StyleRules stylerule = new StyleRules();
                stylerule.Style = style;
                stylerule.StyleId = styleid;
                var list = item.ToList();

                foreach (var rule in list)
                {
                    var selectorlist = SelectorParser.parseSelectorGroup(rule.SelectorText);
                    if (selectorlist != null)
                    {
                        stylerule.PreSelector.Add(rule, selectorlist);
                    }
                }
                prerules.Add(stylerule);
            }

            List<CmsCssRule> unused = new List<CmsCssRule>();

            foreach (var item in prerules)
            {
                var checkdoms = new Dictionary<Guid, Kooboo.Dom.Document>();
                if (item.Style != null)
                {
                    var pageids = sitedb.Styles.GetUsedByPageId(item.Style);
                    foreach (var onepagedom in doms)
                    {
                        if (pageids.Contains(onepagedom.Key))
                        {
                            checkdoms.Add(onepagedom.Key, onepagedom.Value);
                        }
                    }
                }
                else
                {
                    checkdoms = doms;
                }

                foreach (var rule in item.PreSelector)
                {
                    try
                    {
                        bool checkDomused = IsUsed(rule.Value, checkdoms.Values.ToList());

                        if (!checkDomused)
                        {
                            bool checkJsUsed = IsUsed(rule.Value, JsCssNames);
                            if (!checkJsUsed)
                            {
                                unused.Add(rule.Key);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Kooboo.Data.Log.Instance.Exception.WriteException(ex);
                    }
                }
            }
             
            return unused;
        }


        private static async Task<Dictionary<Guid, Kooboo.Dom.Document>> GetDoms(SiteDb sitedb, RenderContext context)
        {
            Dictionary<Guid, Kooboo.Dom.Document> pagedoms = new Dictionary<Guid, Document>();
            List<Kooboo.Dom.Document> doms = new List<Document>();
            foreach (var item in sitedb.Pages.All())
            {
                RenderContext newcontext = new RenderContext();
                newcontext.WebSite = sitedb.WebSite;
                var html = await Kooboo.Sites.Render.PageRenderer.RenderMockAsync(newcontext, item);
                var dom = Kooboo.Dom.DomParser.CreateDom(html);
                pagedoms.Add(item.Id, dom);
                doms.Add(dom);
            }
            return pagedoms;
        }

        public static bool IsUsed(List<simpleSelector> selector, HashSet<string> JsClassNames)
        {
            foreach (var item in selector)
            {
                if (item.Type == enumSimpleSelectorType.classSelector)
                {
                    var clsSelector = item as classSelector;
                    if (clsSelector != null)
                    {
                        foreach (var cls in clsSelector.classList)
                        {
                            if (JsClassNames.Contains(cls))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
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

        public static HashSet<string> JsClassList(SiteDb Sitedb)
        {
            HashSet<string> result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var alljs = Sitedb.Scripts.All();

            foreach (var item in alljs)
            {
                var list = Lib.Utilities.JsStringScanner.ScanStringList(item.Body);
                foreach (var name in list)
                {
                    result.Add(name);
                }
            }
            return result;
        }
    }


    public class PageDomResult
    {
        public Kooboo.Dom.Document Dom { get; set; }

        public List<Guid> StyleId { get; set; }

        public Guid PageId { get; set; }
    }

    public class StyleRules
    {
        public Guid StyleId { get; set; }

        public Style Style { get; set; }

        public Dictionary<CmsCssRule, List<simpleSelector>> PreSelector = new Dictionary<CmsCssRule, List<simpleSelector>>();

    }

}
