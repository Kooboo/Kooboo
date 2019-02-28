//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Sync
{
    public class DiskDbUrl
    {

        public static string CorrectDomUrl(Data.Models.WebSite WebSite, string PageSource, string FilePath)
        {
            if (WebSite == null || string.IsNullOrEmpty(WebSite.DiskSyncFolder))
            {
                return PageSource;
            }

            if (string.IsNullOrEmpty(PageSource))
            {
                return PageSource; 
            }
            string FileRelativePath = RemoveBaseCaseInsensitive(FilePath, WebSite.DiskSyncFolder);

            FileRelativePath = FileRelativePath.Replace("\\", "/");

            if (!FileRelativePath.StartsWith("/"))
            {
                FileRelativePath = "/" + FileRelativePath;
            }

            var dom = Kooboo.Dom.DomParser.CreateDom(PageSource);

            List<SourceUpdate> updates = new List<SourceUpdate>();

            foreach (var item in dom.Links.item)
            {
                string itemsrc = Service.DomUrlService.GetLinkOrSrc(item);

                if (string.IsNullOrEmpty(itemsrc))
                {
                    continue;
                }

                string rightsrc = itemsrc.Replace("\\", "/");

                string RelativeUrl = Kooboo.Lib.Helper.UrlHelper.Combine(FileRelativePath, rightsrc);

                if (itemsrc != RelativeUrl)
                {
                    string oldstring = Kooboo.Sites.Service.DomService.GetOpenTag(item);
                    string newstring = oldstring.Replace(itemsrc, RelativeUrl);


                    updates.Add(new SourceUpdate()
                    {
                        StartIndex = item.location.openTokenStartIndex,
                        EndIndex = item.location.openTokenEndIndex,
                        NewValue = newstring

                    });

                }
            }

            if (updates.Count > 0)
            {

                return Kooboo.Sites.Service.DomService.UpdateSource(PageSource, updates);
            }
            else
            {
                return PageSource;
            } 

        }

        public static string CorrectCssUrl(Data.Models.WebSite website, string CssText, string FilePath)
        {
            if (website == null || string.IsNullOrEmpty(website.DiskSyncFolder))
            {
                return CssText;
            }

            string FileRelativePath = RemoveBaseCaseInsensitive(FilePath, website.DiskSyncFolder);

            FileRelativePath = Kooboo.Lib.Helper.UrlHelper.ReplaceBackSlash(FileRelativePath, true); 
            return ProcessCssText(CssText, FileRelativePath);
        }
         
        private static string ProcessCssText(string cssText, string CssFileRelativeUrl)
        {
            if (string.IsNullOrEmpty(cssText))
            {
                return cssText;
            }

            List<SourceUpdate> updates = new List<SourceUpdate>();

            var urlInfos = Service.CssService.GetUrlInfos(cssText);

            foreach (var item in urlInfos)
            {

                if (!Kooboo.Lib.Utilities.DataUriService.isDataUri(item.PureUrl))
                {
                    string righturl = Kooboo.Lib.Helper.UrlHelper.ReplaceBackSlash(item.PureUrl);  
                    string NewRelativeUrl = Kooboo.Lib.Helper.UrlHelper.Combine(CssFileRelativeUrl, righturl);

                    if (item.PureUrl != NewRelativeUrl)
                    {
                        string newvalue;
                        if (item.isUrlToken)
                        {
                            newvalue = "url(\"" + NewRelativeUrl + "\")";
                        }
                        else
                        {
                            newvalue = "\"" + NewRelativeUrl + "\"";
                        }

                        updates.Add(new SourceUpdate { StartIndex = item.StartIndex, EndIndex = item.EndIndex, NewValue = newvalue });

                    }

                }
            }


            if (updates.Count > 0)
            {
                return Service.DomService.UpdateSource(cssText, updates);
            }

            else
            {
                return cssText;
            }


        }

        private static string RemoveBaseCaseInsensitive(string FullUrl, string BasePath)
        {
            int index = FullUrl.IndexOf(BasePath, StringComparison.OrdinalIgnoreCase);
            if (index > -1)
            {
                int start = index + BasePath.Length;

                return FullUrl.Substring(start);
            }
            else
            {
                return FullUrl;
            }

        }
         
    }
}
