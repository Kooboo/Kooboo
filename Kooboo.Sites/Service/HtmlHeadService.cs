//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Dom;
using Kooboo.Sites.Models;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Service
{
    public class HtmlHeadService
    {
        //extract header information from html source...
        public static HtmlHeader AppendSourceToHeader(string htmlsource, HtmlHeader currentHeader)
        {
            var dom = Dom.DomParser.CreateDom(htmlsource);

            string htmltitle = GetTitle(dom);

            var currenttitle = currentHeader.GetTitle();

            if (string.IsNullOrEmpty(currenttitle))
            {
                currentHeader.SetTitle(htmltitle);
            }

            var metas = dom.head.getElementsByTagName("meta");

            foreach (var item in metas.item)
            {
                AppendMeta(item, currentHeader.Metas);
            }

            AppendScriptStyle(dom, currentHeader);

            return currentHeader;
        }

        public static string SetBaseHref(string htmlsource, string baseurl)
        {
            string strbasetag = "<base href=\"" + baseurl + "\">";
            var dom = Kooboo.Dom.DomParser.CreateDom(htmlsource);

            SourceUpdate update = new SourceUpdate();
            update.NewValue = strbasetag;
            update.EndIndex = -1;

            if (DomService.HasHeadTag(dom))
            {
                var tagBase = dom.head.getElementsByTagName("base");
                if (tagBase != null && tagBase.item.Any())
                {
                    var tagElement = tagBase.item[0];
                    string currentbase = tagElement.getAttribute("href");
                    if (!string.IsNullOrWhiteSpace(currentbase) && currentbase == strbasetag)
                    { return htmlsource; }
                    update.StartIndex = tagElement.location.openTokenStartIndex;
                    update.EndIndex = tagElement.location.endTokenEndIndex;
                }
                else
                {
                    var afterheaderlocation = dom.head.location.openTokenEndIndex + 1;
                    update.StartIndex = afterheaderlocation;
                }
            }
            else
            {
                var afterHtmlLocation = dom.documentElement.location.openTokenEndIndex + 1;
                update.StartIndex = afterHtmlLocation;
                update.NewValue = "<head>" + update.NewValue + "</head>";
            }

            List<SourceUpdate> updates = new List<SourceUpdate>();
            updates.Add(update);

            return Service.DomService.UpdateSource(htmlsource, updates);
        }

        public static string RemoveBaseHrel(string htmlsource)
        {
            var dom = Kooboo.Dom.DomParser.CreateDom(htmlsource);
            List<SourceUpdate> updates = new List<SourceUpdate>();
            var basetags = dom.head.getElementsByTagName("base");
            if (basetags != null && basetags.length > 0)
            {
                foreach (var item in basetags.item)
                {
                    SourceUpdate update = new SourceUpdate
                    {
                        StartIndex = item.location.openTokenStartIndex,
                        EndIndex = item.location.openTokenEndIndex,
                        NewValue = ""
                    };
                    updates.Add(update);
                }
            }
            return updates.Any() ? Service.DomService.UpdateSource(htmlsource, updates) : htmlsource;
        }

        private static void AppendScriptStyle(Document dom, HtmlHeader header)
        {
            var resources = GetScriptStyle(dom);
            foreach (var item in resources)
            {
                if (item.IsScript)
                {
                    if (!header.Scripts.Contains(item.Url))
                    {
                        header.Scripts.Add(item.Url);
                    }
                }
                else if (item.IsStyle)
                {
                    if (!header.Styles.Contains(item.Url))
                    {
                        header.Styles.Add(item.Url);
                    }
                }
            }
        }

        private static void AppendMeta(Element metaelement, List<HtmlMeta> metas)
        {
            string name = metaelement.getAttribute("name");
            string httpquive = metaelement.getAttribute("http-equiv");
            string content = metaelement.getAttribute("content");
            string charset = metaelement.getAttribute("charset");

            if (!string.IsNullOrWhiteSpace(name))
            {
                var find = metas.Find(o => o.name == name);
                if (find != null)
                {
                    if (!find.content.Values.Contains(content))
                    {
                        find.SetContent(content);
                    }
                    return;
                }
            }

            if (!string.IsNullOrWhiteSpace(httpquive))
            {
                var find = metas.Find(o => o.httpequiv == httpquive);
                if (find != null)
                {
                    if (!find.content.Values.Contains(content))
                    {
                        find.SetContent(content);
                    }
                    return;
                }
            }

            if (!string.IsNullOrWhiteSpace(charset) && metas.Any(o => !string.IsNullOrWhiteSpace(o.charset)))
            {
                return;
            }

            // add the new meta into List...
            HtmlMeta newmeta = new HtmlMeta {name = name, httpequiv = httpquive};
            newmeta.content.Add("", content);
            newmeta.charset = charset;
            metas.Add(newmeta);
        }

        public static string SetHeaderToHtml(string htmlsource, HtmlHeader header, string onlyEnableCulture = null)
        {
            if (!header.HasValue())
            {
                return htmlsource;
            }
            var dom = Kooboo.Dom.DomParser.CreateDom(htmlsource);

            string result = string.Empty;

            List<SourceUpdate> updates = GetUpdates(header, dom, onlyEnableCulture);

            if (updates.Count > 0)
            {
                return Service.DomService.UpdateSource(htmlsource, updates);
            }
            else
            { return htmlsource; }
        }

        internal static List<SourceUpdate> GetUpdates(HtmlHeader header, Document dom, string onlyEnableCulture)
        {
            List<SourceUpdate> result;
            if (DomService.HasHeadTag(dom))
            {
                result = GetUpdateWithDomHeader(header, dom, onlyEnableCulture);
            }
            else
            {
                result = GetUpdateWithOutDomHeader(header, dom, onlyEnableCulture);
            }

            return result;
        }

        internal static List<SourceUpdate> GetUpdateWithDomHeader(HtmlHeader header, Document dom, string onlyEnableCulture)
        {
            List<SourceUpdate> updates = new List<SourceUpdate>();

            string appendChanges = string.Empty;

            var titlevalue = GetOnlyOrDefault(header.Titles, onlyEnableCulture);
            if (!string.IsNullOrEmpty(titlevalue))
            {
                var titletag = DomService.GetTitleElement(dom);
                string newtitletag = $"<Title>{titlevalue}</Title>\r\n";
                if (titletag == null)
                {
                    appendChanges += newtitletag;
                }
                else
                {
                    if (titletag.InnerHtml != titlevalue)
                    {
                        updates.Add(new SourceUpdate() { StartIndex = titletag.location.openTokenStartIndex, EndIndex = titletag.location.endTokenEndIndex, NewValue = newtitletag });
                    }
                }
            }

            foreach (var item in header.Metas)
            {
                var checkresult = MetaCheck(item, onlyEnableCulture);

                var sametags = GetSameMetaTags(dom, item);

                string metatag = GenerateMetaTag(item, checkresult.ContentValue);

                if (sametags != null && sametags.Any())
                {
                    var tag = sametags[0];
                    if (!IsSameMeta(tag, item, checkresult.ContentValue))
                    {
                        updates.Add(new SourceUpdate() { StartIndex = tag.location.openTokenStartIndex, EndIndex = tag.location.endTokenEndIndex, NewValue = metatag });
                    }

                    if (sametags.Count() > 1)
                    {
                        int len = sametags.Count();
                        for (int i = 1; i < len; i++)
                        {
                            updates.Add(new SourceUpdate() { StartIndex = sametags[i].location.openTokenStartIndex, EndIndex = sametags[i].location.endTokenEndIndex, NewValue = null });
                        }
                    }
                }
                else
                {
                    appendChanges += metatag;
                }
            }

            WriteStyleScript(header, dom, updates, ref appendChanges);

            if (!string.IsNullOrEmpty(appendChanges))
            {
                var afterheaderlocation = dom.head.location.openTokenEndIndex + 1;

                updates.Add(new SourceUpdate() { StartIndex = afterheaderlocation, EndIndex = -1, NewValue = appendChanges });
            }
            return updates;
        }

        private static bool IsSameMeta(Element metaElement, HtmlMeta meta, string Content)
        {
            string name = metaElement.getAttribute("name");
            string httpequive = metaElement.getAttribute("http-equiv");
            string charset = metaElement.getAttribute("charset");
            string content = metaElement.getAttribute("content");

            return (SameStringValue(name, meta.name) && SameStringValue(httpequive, meta.httpequiv) && SameStringValue(charset, meta.charset) && SameStringValue(content, Content));
        }

        internal static List<SourceUpdate> GetUpdateWithOutDomHeader(HtmlHeader header, Document dom, string onlyEnableCulture)
        {
            List<SourceUpdate> updates = new List<SourceUpdate>();

            string appendChanges = string.Empty;

            var titlevalue = GetOnlyOrDefault(header.Titles, onlyEnableCulture);
            if (!string.IsNullOrEmpty(titlevalue))
            {
                appendChanges += $"<Title>{titlevalue}</Title>";
            }

            foreach (var item in header.Metas)
            {
                var checkresult = MetaCheck(item, onlyEnableCulture);

                if (checkresult.ShouldAppendToDom)
                {
                    appendChanges = appendChanges + "\r\n" + GenerateMetaTag(item, checkresult.ContentValue);
                }
            }

            WriteStyleScript(header, dom, updates, ref appendChanges);

            if (!string.IsNullOrEmpty(appendChanges))
            {
                appendChanges = "<head>\r\n" + appendChanges + "\r\n</head>";
                var afterHtmlLocation = dom.documentElement.location.openTokenEndIndex + 1;

                updates.Add(new SourceUpdate() { StartIndex = afterHtmlLocation, EndIndex = -1, NewValue = appendChanges });
            }
            return updates;
        }

        private static void WriteStyleScript(HtmlHeader header, Document dom, List<SourceUpdate> updates, ref string appendChanges)
        {
            var currentstylescript = GetScriptStyle(dom);
            foreach (var item in currentstylescript)
            {
                if (item.IsScript)
                {
                    if (header.Scripts.Contains(item.Url))
                    {
                        header.Scripts.Remove(item.Url);
                    }
                    else
                    {
                        // remove it...
                        updates.Add(new SourceUpdate() { StartIndex = item.Element.location.openTokenStartIndex, EndIndex = item.Element.location.endTokenEndIndex, NewValue = null });
                    }
                }
                else if (item.IsStyle)
                {
                    if (header.Styles.Contains(item.Url))
                    {
                        header.Styles.Remove(item.Url);
                    }
                    else
                    {
                        // remove it.
                        updates.Add(new SourceUpdate() { StartIndex = item.Element.location.openTokenStartIndex, EndIndex = item.Element.location.endTokenEndIndex, NewValue = null });
                    }
                }
            }

            foreach (var item in header.Scripts)
            {
                string tag = $"<script src=\"{ item}\"></script>";
                appendChanges += tag;
            }
            foreach (var item in header.Styles)
            {
                string tag = $"<link rel=\"stylesheet\" href=\"{item}\" />";
                appendChanges += tag;
            }
        }

        internal static MetaContentkResult MetaCheck(HtmlMeta meta, string onlyEnableCulture)
        {
            MetaContentkResult result = new MetaContentkResult {ShouldAppendToDom = true};
            if (meta.content.Count == 0)
            {
                return result;
            }

            string content = null;

            if (meta.content.Count() == 1)
            {
                content = meta.content.Values.First();
            }

            if (string.IsNullOrEmpty(onlyEnableCulture) || !meta.content.TryGetValue(onlyEnableCulture, out content))
            {
                meta.content.TryGetValue("", out content);
            }

            result.ContentValue = content;
            return result;
        }

        private static string GenerateMetaTag(HtmlMeta item, string metacontent)
        {
            string metaTag = "<Meta ";

            if (!string.IsNullOrEmpty(item.name))
            {
                metaTag += "Name=\"" + item.name + "\"";
            }

            if (!string.IsNullOrEmpty(item.httpequiv))
            {
                metaTag += "http-equiv=\"" + item.httpequiv + "\"";
            }
            if (!string.IsNullOrEmpty(item.charset))
            {
                metaTag += "charset=\"" + item.charset + "\"";
            }
            if (!string.IsNullOrEmpty(metacontent))
            {
                metaTag += "content=\"" + metacontent + "\"";
            }
            metaTag += " />\r\n";
            return metaTag;
        }

        private static List<Element> GetSameMetaTags(Document dom, HtmlMeta meta)
        {
            List<Element> result = new List<Element>();
            if (!DomService.HasHeadTag(dom))
            {
                return null;
            }

            var metas = dom.head.getElementsByTagName("meta").item;

            if (!string.IsNullOrEmpty(meta.name))
            {
                string lowermeta = meta.name.ToLower();
                foreach (var item in metas)
                {
                    var name = item.getAttribute("name");
                    if (!string.IsNullOrEmpty(name) && name.ToLower() == lowermeta)
                    {
                        result.Add(item);
                    }
                }
            }

            if (result.Any())
            {
                return result;
            }

            if (!string.IsNullOrEmpty(meta.httpequiv))
            {
                string lowermeta = meta.httpequiv.ToLower();
                foreach (var item in metas)
                {
                    var name = item.getAttribute("http-equiv");
                    if (!string.IsNullOrEmpty(name) && name.ToLower() == lowermeta)
                    {
                        result.Add(item);
                    }
                }
            }

            if (!string.IsNullOrEmpty(meta.charset))
            {
                //  string lowermeta = meta.charset.ToLower();
                foreach (var item in metas)
                {
                    var name = item.getAttribute("charset");
                    if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(meta.charset))
                    {
                        result.Add(item);
                    }
                }
            }

            return result;
        }

        internal static string GetOnlyOrDefault(Dictionary<string, string> multiLingualValues, string onlyEnableCulture = null)
        {
            if (multiLingualValues == null || multiLingualValues.Count == 0)
            {
                return null;
            }
            string value = null;
            if (multiLingualValues.Count == 1)
            {
                value = multiLingualValues.Values.First();
            }
            else
            {
                if (!string.IsNullOrEmpty(onlyEnableCulture))
                {
                    if (!multiLingualValues.TryGetValue(onlyEnableCulture, out value))
                    {
                        multiLingualValues.TryGetValue("", out value);
                    }
                }
                else
                {
                    multiLingualValues.TryGetValue("", out value);
                }
            }

            return value;
        }

        internal static string GetTitle(Dom.Document dom)
        {
            var titleTag = DomService.GetTitleElement(dom);
            return titleTag != null ? titleTag.InnerHtml : null;
        }

        public static List<HeaderBindings> GetHeaderBinding(Page page, string culture, bool isMultilingual = false)
        {
            List<HeaderBindings> bindings = new List<HeaderBindings>();

            var title = page.Headers.GetTitle(culture);

            if (!string.IsNullOrEmpty(title))
            {
                if (isMultilingual || page.HasLayout || Service.BindingService.IsBinding(title))
                {
                    HeaderBindings titlebinding = new HeaderBindings {IsTitle = true, Content = title};
                    bindings.Add(titlebinding);
                }
            }

            foreach (var item in page.Headers.Metas)
            {
                string metabindingValue = item.GetContent(culture);

                if (!string.IsNullOrEmpty(metabindingValue))
                {
                    if (isMultilingual || page.HasLayout || Service.BindingService.IsBinding(metabindingValue))
                    {
                        HeaderBindings metabinding = new HeaderBindings
                        {
                            MetaName = item.name,
                            Content = metabindingValue,
                            CharSet = item.charset,
                            HttpEquiv = item.httpequiv
                        };
                        bindings.Add(metabinding);
                    }
                }
            }

            string customerHeader = null;

            if (page.HasLayout)
            {
                foreach (var item in page.Headers.Styles)
                {
                    customerHeader += "\r\n<link rel=\"stylesheet\" type=\"text/css\" href=\"" + item + "\" />";
                }
                foreach (var item in page.Headers.Scripts)
                {
                    customerHeader += "\r\n<script type=\"text/javascript\" src=\"" + item + "\"></script>";
                }
            }

            customerHeader += page.Headers.CustomHeader;

            if (!string.IsNullOrEmpty(customerHeader))
            {
                HeaderBindings custombinding = new HeaderBindings {IsCustomHeader = true, Content = customerHeader};
                bindings.Add(custombinding);
            }

            return bindings;
        }

        private static bool SameStringValue(string x, string y)
        {
            if (string.IsNullOrWhiteSpace(x) && string.IsNullOrWhiteSpace(y))
            {
                return true;
            }
            return x == y;
        }

        private static List<ScriptStyle> GetScriptStyle(Document dom)
        {
            List<ScriptStyle> result = new List<ScriptStyle>();

            var scripts = dom.getElementsByTagName("script");

            foreach (var item in scripts.item)
            {
                var src = item.getAttribute("src");
                if (!string.IsNullOrEmpty(src))
                {
                    ScriptStyle script = new ScriptStyle {IsScript = true, Url = src, Element = item};
                    result.Add(script);
                }
                else
                {
                    string inner = item.InnerHtml;
                    if (string.IsNullOrWhiteSpace(inner))
                    {
                        ScriptStyle script = new ScriptStyle {IsScript = true, Url = string.Empty, Element = item};
                        result.Add(script);
                    }
                }
            }

            var styles = dom.getElementsByTagName("link");

            foreach (var item in styles.item)
            {
                if (item.hasAttribute("rel") && item.getAttribute("rel").ToLower().Contains("stylesheet"))
                {
                    string itemurl = item.getAttribute("href");

                    if (!string.IsNullOrEmpty(itemurl))
                    {
                        ScriptStyle style = new ScriptStyle {IsStyle = true, Url = itemurl, Element = item};
                        result.Add(style);
                    }
                }
            }

            return result;
        }
    }

    public class MetaContentkResult
    {
        public bool ShouldAppendToDom { get; set; }

        //public bool ShouldRemove { get; set; }

        public string ContentValue { get; set; }

        // public bool ShouldRender { get; set; }
    }

    public class ScriptStyle
    {
        public string Url { get; set; }

        public Element Element { get; set; }

        public bool IsScript { get; set; }

        public bool IsStyle { get; set; }
    }
}