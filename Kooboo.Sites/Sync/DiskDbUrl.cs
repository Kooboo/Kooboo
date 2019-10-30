//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Sync
{
    public class DiskDbUrl
    {
        public static string CorrectDomUrl(Data.Models.WebSite webSite, string pageSource, string filePath)
        {
            if (webSite == null || string.IsNullOrEmpty(webSite.DiskSyncFolder))
            {
                return pageSource;
            }

            if (string.IsNullOrEmpty(pageSource))
            {
                return pageSource;
            }
            string fileRelativePath = RemoveBaseCaseInsensitive(filePath, webSite.DiskSyncFolder);

            fileRelativePath = fileRelativePath.Replace("\\", "/");

            if (!fileRelativePath.StartsWith("/"))
            {
                fileRelativePath = "/" + fileRelativePath;
            }

            var dom = Kooboo.Dom.DomParser.CreateDom(pageSource);

            List<SourceUpdate> updates = new List<SourceUpdate>();

            foreach (var item in dom.Links.item)
            {
                string itemsrc = Service.DomUrlService.GetLinkOrSrc(item);

                if (string.IsNullOrEmpty(itemsrc))
                {
                    continue;
                }

                string rightsrc = itemsrc.Replace("\\", "/");

                string relativeUrl = Kooboo.Lib.Helper.UrlHelper.Combine(fileRelativePath, rightsrc);

                if (itemsrc != relativeUrl)
                {
                    string oldstring = Kooboo.Sites.Service.DomService.GetOpenTag(item);
                    string newstring = oldstring.Replace(itemsrc, relativeUrl);

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
                return Kooboo.Sites.Service.DomService.UpdateSource(pageSource, updates);
            }
            else
            {
                return pageSource;
            }
        }

        public static string CorrectCssUrl(Data.Models.WebSite website, string cssText, string filePath)
        {
            if (website == null || string.IsNullOrEmpty(website.DiskSyncFolder))
            {
                return cssText;
            }

            string fileRelativePath = RemoveBaseCaseInsensitive(filePath, website.DiskSyncFolder);

            fileRelativePath = Kooboo.Lib.Helper.UrlHelper.ReplaceBackSlash(fileRelativePath, true);
            return ProcessCssText(cssText, fileRelativePath);
        }

        private static string ProcessCssText(string cssText, string cssFileRelativeUrl)
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
                    string newRelativeUrl = Kooboo.Lib.Helper.UrlHelper.Combine(cssFileRelativeUrl, righturl);

                    if (item.PureUrl != newRelativeUrl)
                    {
                        string newvalue;
                        if (item.isUrlToken)
                        {
                            newvalue = "url(\"" + newRelativeUrl + "\")";
                        }
                        else
                        {
                            newvalue = "\"" + newRelativeUrl + "\"";
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

        private static string RemoveBaseCaseInsensitive(string fullUrl, string basePath)
        {
            int index = fullUrl.IndexOf(basePath, StringComparison.OrdinalIgnoreCase);
            if (index > -1)
            {
                int start = index + basePath.Length;

                return fullUrl.Substring(start);
            }
            else
            {
                return fullUrl;
            }
        }
    }
}