//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Dom;
using Kooboo.Lib.Helper;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.SiteTransfer
{
    public class InlineAnalyzer : ITransferAnalyzer
    {
        public void Execute(AnalyzerContext context)
        {
            List<Kooboo.Dom.Element> doneitems = new List<Dom.Element>();

            foreach (var item in context.Dom.Links.item)
            {
                doneitems.Add(item);

                Dictionary<string, string> replace = new Dictionary<string, string>();
                var linkreplace = ReplaceLinks(item, context);
                var cssreplace = ReplaceInlineCssUrl(item, context);

                if (linkreplace != null && linkreplace.Count() > 0)
                {
                    foreach (var linkrep in linkreplace)
                    {
                        replace[linkrep.Key] = linkrep.Value;
                    }
                }

                if (cssreplace != null && cssreplace.Any())
                {
                    foreach (var cssrep in cssreplace)
                    {
                        replace[cssrep.Key] = cssrep.Value;
                    }
                }

                if (replace.Any())
                {
                    string oldstring = Kooboo.Sites.Service.DomService.GetOpenTag(item);
                    string newstring = oldstring;
                    foreach (var repitem in replace)
                    {
                        newstring = newstring.Replace(repitem.Key, repitem.Value);
                    }

                    context.Changes.Add(new AnalyzerUpdate()
                    {
                        StartIndex = item.location.openTokenStartIndex,
                        EndIndex = item.location.openTokenEndIndex,
                        NewValue = newstring
                    });
                }
            }

            // process inline css, but exclusive items that has been in the links.
            HTMLCollection inlinestyle = context.Dom.getElementByAttribute("style");

            foreach (var item in inlinestyle.item)
            {
                if (doneitems.Where(o => o.isEqualNode(item)).Any())
                {
                    continue;
                }

                var updates = ReplaceInlineCssUrl(item, context);

                if (updates != null && updates.Any())
                {
                    string oldstring = Kooboo.Sites.Service.DomService.GetOpenTag(item);
                    string newstring = oldstring;
                    foreach (var repitem in updates)
                    {
                        newstring = newstring.Replace(repitem.Key, repitem.Value);
                    }

                    context.Changes.Add(new AnalyzerUpdate()
                    {
                        StartIndex = item.location.openTokenStartIndex,
                        EndIndex = item.location.openTokenEndIndex,
                        NewValue = newstring
                    });
                }
            }
        }

        private Dictionary<string, string> ReplaceLinks(Kooboo.Dom.Element element, AnalyzerContext context)
        {
            string itemsrc = Service.DomUrlService.GetLinkOrSrc(element);

            if (string.IsNullOrEmpty(itemsrc))
            {
                return null;
            }

            Dictionary<string, string> replace = new Dictionary<string, string>();

            string absoluteurl = UrlHelper.Combine(context.AbsoluteUrl, itemsrc);

            bool issamehost = UrlHelper.isSameHost(absoluteurl, context.OriginalImportUrl);

            var objectType = Service.ConstTypeService.GetConstTypeByUrl(absoluteurl);

            if (issamehost)
            {
                string relativeurl = UrlHelper.RelativePath(absoluteurl, issamehost);

                if (itemsrc != relativeurl)
                {
                    replace.Add(itemsrc, relativeurl);
                }
            }
            else
            {
                if (itemsrc != absoluteurl)
                {
                    replace.Add(itemsrc, absoluteurl);
                }
            }

            return replace;
        }

        private Dictionary<string, string> ReplaceInlineCssUrl(Kooboo.Dom.Element element, AnalyzerContext context)
        {
            string csstext = element.getAttribute("style");
            if (string.IsNullOrEmpty(csstext))
            {
                return null;
            }

            Dictionary<string, string> replace = new Dictionary<string, string>();

            var urlInfos = Service.CssService.GetUrlInfos(csstext);

            foreach (var item in urlInfos)
            {
                if (string.IsNullOrEmpty(item.PureUrl) || item.PureUrl.Trim().ToLower().StartsWith("#"))
                {
                    continue;
                }

                string newurl = string.Empty;
                if (item.isImportRule)
                {
                    newurl = CssManager.AddImport(item.PureUrl, context.AbsoluteUrl, context.DownloadManager, context.ObjectId);
                }
                else
                {
                    if (Kooboo.Lib.Utilities.DataUriService.isDataUri(item.PureUrl))
                    {
                        newurl = CssManager.ParseDataUri(item.PureUrl, context.DownloadManager);
                    }
                    else
                    {
                        newurl = CssManager.DownloadCssFile(item.PureUrl, context.AbsoluteUrl, context.DownloadManager, context.ObjectId);
                    }
                }

                if (newurl != item.PureUrl)
                {
                    replace.Add(item.PureUrl, newurl);
                }
            }

            return replace;
        }
    }
}