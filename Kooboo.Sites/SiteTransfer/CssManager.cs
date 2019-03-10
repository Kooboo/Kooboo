//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Lib.Helper;
using Kooboo.Sites.Models;
using Kooboo.Sites.SiteTransfer.Download;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.SiteTransfer
{
    public static class CssManager
    {
        public static void ProcessResource(ref string cssText, string baseurl, DownloadManager manager, Guid OwnerObjectId)
        {
            if (string.IsNullOrEmpty(cssText))
            {
                return;
            }
            Dictionary<string, string> replaces = new Dictionary<string, string>();
            List<AnalyzerUpdate> updates = new List<AnalyzerUpdate>();

            var urlInfos = Service.CssService.GetUrlInfos(cssText);

            foreach (var item in urlInfos)
            {
                if (item.isImportRule)
                {
                    string newurl = AddImport(item.PureUrl, baseurl, manager, OwnerObjectId);
                    if (newurl != item.PureUrl)
                    {
                        string newvalue;
                        if (item.isUrlToken)
                        {
                            newvalue = "url(\"" + newurl + "\")";
                        }
                        else
                        {
                            newvalue = "\"" + newurl + "\"";
                        }

                        updates.Add(new AnalyzerUpdate { StartIndex = item.StartIndex, EndIndex = item.EndIndex, NewValue = newvalue });
                    }
                }
                else
                {
                    DownloadUrl(item, replaces, updates, baseurl, manager, OwnerObjectId);
                }
            }

            if (updates.Count > 0)
            {
                cssText = AnalyzerManager.ParseChanges(cssText, updates);
            }

            foreach (var item in replaces)
            {
                cssText = cssText.Replace(item.Key, item.Value);
            }
        }

        public static List<AnalyzerUpdate> ProcessInlineResource(Kooboo.Dom.Element inlineElement, string baseurl, DownloadManager manager, Guid OwnerObjectId)
        {
            List<AnalyzerUpdate> updates = new List<AnalyzerUpdate>();

            string csstext = inlineElement.getAttribute("style");

            if (string.IsNullOrEmpty(csstext))
            {
                return updates;
            }

            var urlInfos = Service.CssService.GetUrlInfos(csstext);

            foreach (var item in urlInfos)
            {
                if (item.isImportRule)
                {
                    string newurl = AddImport(item.PureUrl, baseurl, manager, OwnerObjectId);
                    if (newurl != item.PureUrl)
                    {
                        updates.Add(new AnalyzerUpdate { StartIndex = inlineElement.location.openTokenStartIndex, EndIndex = inlineElement.location.openTokenEndIndex, IsReplace = true, OldValue = item.PureUrl, NewValue = newurl });
                    }
                }
                else
                {
                    string newurl = string.Empty;
                    if (Kooboo.Lib.Utilities.DataUriService.isDataUri(item.PureUrl))
                    {
                        newurl = ParseDataUri(item.PureUrl, manager);
                    }
                    else
                    {
                        newurl = DownloadCssFile(item.PureUrl, baseurl, manager, OwnerObjectId);
                    }

                    updates.Add(new AnalyzerUpdate { StartIndex = inlineElement.location.openTokenStartIndex, EndIndex = inlineElement.location.openTokenEndIndex, IsReplace = true, OldValue = item.PureUrl, NewValue = newurl });

                }
            }

            return updates;

        }
         
        public static string AddImport(string importurl, string baseurl, DownloadManager manager, Guid OwnerObjectId)
        {
            string fullimporturl = UrlHelper.Combine(baseurl, importurl);

            string OriginalImportUrl = string.IsNullOrEmpty(manager.OriginalImportUrl) ? baseurl : manager.OriginalImportUrl;

            bool issamehost = UrlHelper.isSameHost(fullimporturl, OriginalImportUrl);

            string relativeimporturl = UrlHelper.RelativePath(fullimporturl, issamehost);

            DownloadTask newdownload = new DownloadTask();
            newdownload.AbsoluteUrl = fullimporturl;
            newdownload.ConstType = ConstObjectType.Style;
            newdownload.RelativeUrl = relativeimporturl;
            newdownload.OwnerObjectId = OwnerObjectId;
            manager.AddTask(newdownload);
            return relativeimporturl;
        }

        private static void DownloadUrl(Service.UrlInfo item, Dictionary<string, string> replaces, List<AnalyzerUpdate> updates, string baseurl, DownloadManager manager, Guid OwnerObjectId)
        {
            string newurl = string.Empty;
            if (Kooboo.Lib.Utilities.DataUriService.isDataUri(item.PureUrl))
            {
                if (!replaces.ContainsKey(item.PureUrl))
                {
                    newurl = ParseDataUri(item.PureUrl, manager);
                    replaces.Add(item.PureUrl, newurl);
                }
                return;
            }
            else
            {
                newurl = DownloadCssFile(item.PureUrl, baseurl, manager, OwnerObjectId);

                if (newurl != item.PureUrl)
                {
                    string newvalue = newurl;
                    if (item.isUrlToken)
                    {
                        newvalue = "url(\"" + newvalue + "\")";
                    }
                    else
                    {
                        newvalue = "\"" + newvalue + "\"";
                    }
                    updates.Add(new AnalyzerUpdate { StartIndex = item.StartIndex, EndIndex = item.EndIndex, NewValue = newvalue });
                }
            }


        }

        public static string ParseDataUri(string datastring, DownloadManager manager)
        {
            var datauri = Kooboo.Lib.Utilities.DataUriService.PraseDataUri(datastring);

            if (datauri != null)
            {
                if (datauri.isBase64)
                {
                    Guid id = Lib.Security.Hash.ComputeGuidIgnoreCase(datastring);
                    Image koobooimage = new Image
                    {
                        Extension = UrlHelper.GetImageExtensionFromMine(datauri.MineType),
                        ContentBytes = Convert.FromBase64String(datauri.DataString),
                        Id = id,
                        Name = id.ToString()
                    };
                    string url = "/images/base64/style/" + koobooimage.Id.ToString();
                    manager.SiteDb.Routes.AddOrUpdate(url, koobooimage, manager.UserId);
                    manager.SiteDb.Images.AddOrUpdate(koobooimage, manager.UserId);
                    return url;
                }
                else
                {
                    // TODO: other encoding not implemented yet. 
                }
            }
            return null;
        }

        public static string DownloadCssFile(string ResourceUrl, string BaseUrl, DownloadManager manager, Guid OwnerObjectId)
        {
            string fullurl = UrlHelper.Combine(BaseUrl, ResourceUrl);

            string OriginalImportUrl = string.IsNullOrEmpty(manager.OriginalImportUrl) ? BaseUrl : manager.OriginalImportUrl;

            bool issamehost = UrlHelper.isSameHost(fullurl, OriginalImportUrl);

            string relativeurl = UrlHelper.RelativePath(fullurl, issamehost);

            var minetype = IOHelper.MimeType(relativeurl);

            DownloadTask newdownload = new DownloadTask();
            newdownload.AbsoluteUrl = fullurl;

            newdownload.RelativeUrl = relativeurl;
            newdownload.OwnerObjectId = OwnerObjectId;

            if (Kooboo.Lib.Helper.UrlHelper.IsImage(fullurl))
            {
                newdownload.ConstType = ConstObjectType.Image;
            }
            else
            {
                newdownload.ConstType = ConstObjectType.CmsFile;
            }
            manager.AddTask(newdownload);
            return relativeurl;
        }

    }
}
