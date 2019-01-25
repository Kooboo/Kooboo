//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Lib.Helper; 
using System.Collections.Generic;
using System.Linq; 
using Kooboo.Dom;

namespace Kooboo.Sites.SiteTransfer
{ 
    public class InlineAnalyzer : ITransferAnalyzer
    {
        public void Execute(AnalyzerContext Context)
        {
            List<Kooboo.Dom.Element> doneitems = new List<Dom.Element>(); 

            foreach (var item in Context.Dom.Links.item)
            {
                doneitems.Add(item); 

                Dictionary<string, string> replace = new Dictionary<string, string>(); 
                var linkreplace = ReplaceLinks(item, Context); 
                var cssreplace = ReplaceInlineCssUrl(item, Context); 

                if (linkreplace !=null && linkreplace.Count()>0)
                {
                    foreach (var linkrep in linkreplace)
                    {
                        replace[linkrep.Key] = linkrep.Value; 
                    }
                }

                if (cssreplace != null && cssreplace.Count()>0)
                {
                    foreach (var cssrep in cssreplace)
                    {
                        replace[cssrep.Key] = cssrep.Value; 
                    }
                }

                if (replace.Count()>0)
                {
                    string oldstring = Kooboo.Sites.Service.DomService.GetOpenTag(item); 
                    string newstring = oldstring;
                    foreach (var repitem in replace)
                    {
                        newstring = newstring.Replace(repitem.Key, repitem.Value);  
                    }
 
                    Context.Changes.Add(new AnalyzerUpdate()
                    {
                        StartIndex = item.location.openTokenStartIndex,
                        EndIndex = item.location.openTokenEndIndex,
                        NewValue = newstring
                    }); 
                } 
            }

            // process inline css, but exclusive items that has been in the links. 
            HTMLCollection inlinestyle = Context.Dom.getElementByAttribute("style");

            foreach (var item in inlinestyle.item)
            {
                if (doneitems.Where(o=>o.isEqualNode(item)).Any())
                {
                    continue; 
                }

                var updates = ReplaceInlineCssUrl(item, Context);

                if (updates != null && updates.Count() > 0)
                {
                    string oldstring = Kooboo.Sites.Service.DomService.GetOpenTag(item);
                    string newstring = oldstring;
                    foreach (var repitem in updates)
                    {
                        newstring = newstring.Replace(repitem.Key, repitem.Value);
                    }

                    Context.Changes.Add(new AnalyzerUpdate()
                    {
                        StartIndex = item.location.openTokenStartIndex,
                        EndIndex = item.location.openTokenEndIndex,
                        NewValue = newstring
                    });
                }
            } 

        }

        private Dictionary<string, string> ReplaceLinks(Kooboo.Dom.Element element, AnalyzerContext Context)
        {
            string itemsrc = Service.DomUrlService.GetLinkOrSrc(element);

            if (string.IsNullOrEmpty(itemsrc))
            {
                return null;
            }

            Dictionary<string, string> replace = new Dictionary<string, string>(); 

            string absoluteurl = UrlHelper.Combine(Context.AbsoluteUrl, itemsrc);

            bool issamehost = UrlHelper.isSameHost(absoluteurl, Context.OriginalImportUrl);

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
         
        private Dictionary<string, string> ReplaceInlineCssUrl(Kooboo.Dom.Element element, AnalyzerContext Context)
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
                    newurl = CssManager.AddImport(item.PureUrl, Context.AbsoluteUrl,Context.DownloadManager, Context.ObjectId);
                }
                else
                {
                    if (Kooboo.Lib.Utilities.DataUriService.isDataUri(item.PureUrl))
                    {
                        newurl = CssManager.ParseDataUri(item.PureUrl, Context.DownloadManager);
                    }
                    else
                    {
                        newurl = CssManager.DownloadCssFile(item.PureUrl, Context.AbsoluteUrl, Context.DownloadManager, Context.ObjectId); 
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
