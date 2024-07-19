using Kooboo.Data.Context;
using Kooboo.Data.Models;
using Kooboo.Sites.Models;
using System.Linq;
using System.Collections.Generic;
using Kooboo.Dom.CSS;
using Kooboo.Sites.Extensions;

namespace Kooboo.Web.Lighthouse.CssMinimizer
{

    public class CssReducer
    {

        public static CssReducer instance { get; set; } = new CssReducer();

        private long lastversion { get; set; }
        private HashSet<string> clsNames { get; set; }

        private object _locker = new object();

        public string Render(RenderContext context, System.Guid Id, string body)
        {
            var sitedb = context.WebSite.SiteDb();
            var siteversion = sitedb.DatabaseDb.Log.Store.LastKey;

            if (clsNames == null)
            {
                lock(_locker)
                {
                    if (clsNames == null)
                    {
                        var names = new HashSet<string>();
                        var items = sitedb.CssClassName.All(); 
                        foreach (var item in items)
                        {
                            if (item !=null && item.ClassName !=null)
                            {
                                var name = item.ClassName.ToLower(); 
                                names.Add(name);
                            }
                        }
                        lastversion = siteversion; 
                        clsNames=names; 
                    }
                } 
            }
            else
            {
                if (lastversion != siteversion)
                {
                    clsNames = null;
                    return Render(context, Id, body); 
                }
            }
              
            //TODO: add cache here.  
            return ReduceCssText(body, clsNames); 
        }
         

        //classNames must be lower case. 
        public string ReduceCssText(string CssBody, HashSet<string> classNames)
        { 
            if (classNames == null || classNames.Count() ==0)
            {
                return CssBody; 
            }

            var parts = ParseClassRules(CssBody);

            if (parts == null || parts.Count == 0)
            {
                return CssBody;
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            int totalLen = CssBody.Length;
            int currentIndex = 0;

            foreach (var item in parts.OrderBy(o => o.StartIndex))
            {
                if (!classNames.Contains(item.ClassName))  
                {
                    var len = item.StartIndex - currentIndex;

                    if (len > 0)
                    {
                        var current = CssBody.Substring(currentIndex, item.StartIndex - currentIndex);

                        sb.Append(current);
                    }
                    currentIndex = item.EndIndex + 1;
                } 
            }

            if (currentIndex == 0)
            {
                return CssBody;
            }
            else if (currentIndex < totalLen - 1)
            {
                sb.Append(CssBody.Substring(currentIndex, totalLen - currentIndex));
            }

            return sb.ToString();
        }


        public List<CssClassRule> ParseClassRules(string cssBody)
        {
            List<CssClassRule> parts = new List<CssClassRule>();

            var style = Kooboo.Dom.CSSParser.ParseCSSStyleSheet(cssBody);

            foreach (var item in style.cssRules.item)
            {
                if (item.type == enumCSSRuleType.STYLE_RULE)
                {
                    var rule = item as CSSStyleRule;
                    if (rule != null)
                    {
                        if (rule.selectors != null && rule.selectors.Count() == 1)
                        {

                            var selector = rule.selectors[0];
                            if (selector != null && selector.Type == enumSimpleSelectorType.classSelector)
                            { 
                                if (selector.wholeText != null)
                                {
                                    CssClassRule part = new CssClassRule();
                                    part.StartIndex = rule.StartIndex;
                                    part.EndIndex = rule.EndIndex;
                                    part.ClassName = selector.wholeText.ToLower().Trim().Trim('.');

                                    parts.Add(part);
                                } 
                            }
                        }
                    }
                }
            }

            return parts;
        }
         

    }

    public class CssClassRule
    {
        public int StartIndex { get; set; }

        public int EndIndex { get; set; }
        public string ClassName { get; set; }

    }


}
