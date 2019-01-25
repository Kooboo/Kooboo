//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Kooboo.Extensions;
using Kooboo.Lib.Helper;

namespace Kooboo.Sites.SiteTransfer
{
    public interface IWebSiteScanService
    {
        CancellationToken CancelToken { get; set; }

        List<JsTreeDataItem> GetTreeResultAsync(string url, int depth, int pages, Action<List<JsTreeDataItem>> callback);
    }

    public class WebSiteScanService : IWebSiteScanService
    {
        public CancellationToken CancelToken { get; set; }

        public List<JsTreeDataItem> GetTreeResultAsync(string url, int depth, int pages, Action<List<JsTreeDataItem>> callback)
        {
            var list = new List<JsTreeDataItem>();
            var urlContentDictionary = new Dictionary<string, string>();
            return GetPageLinks(urlContentDictionary, url, list, 0, depth, pages, callback);
        }

        private List<JsTreeDataItem> GetPageLinks(Dictionary<string, string> urlContentDictionary, string rootUrl, List<JsTreeDataItem> list, int level, int maxLevel, int maxPages, Action<List<JsTreeDataItem>> callback)
        {
            level++;
            var content = DownloadHelper.DownloadUrl(rootUrl);
            if (content != null)
            {
                 
                var html = content.ContentString;
                if (urlContentDictionary.ContainsKey(rootUrl))
                {
                    return list;
                }
                urlContentDictionary[rootUrl] = html;
                var rootItem = new JsTreeDataItem
                {
                    Id = rootUrl.ToHashGuid().ToString(),
                    Parent = level == 1 ? "#" : rootUrl.ToHashGuid().ToString(),
                    Text = rootUrl
                };
                if (level == 1)
                {
                    rootItem.TreeState = new JsTreeState
                    {
                        Selected = true,
                        Disabled = true
                    };
                }
                list.Add(rootItem);
                callback(list);

                var links = TransferHelper.GetLinkElements(html);
                var rootUri = new Uri(rootUrl);
                var rootHost = rootUri.Host;
                IEnumerable<string> urls = links.Where(link => link.attributes != null &&
                    link.attributes.Any(attr => attr.name.Equals("href", StringComparison.OrdinalIgnoreCase)
                        && !attr.value.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase)
                        && !attr.value.StartsWith("#", StringComparison.OrdinalIgnoreCase)
                        )).Select(link =>
                        {
                            var firstOrDefault = link.attributes.FirstOrDefault(it => it.name.Equals("href", StringComparison.OrdinalIgnoreCase));
                            if (firstOrDefault == null)
                            {
                                return null;
                            }
                            var currentUri = new Uri(firstOrDefault.value, UriKind.RelativeOrAbsolute);
                            if (currentUri.IsAbsoluteUri)
                            {
                                if (!currentUri.Host.Equals(rootHost, StringComparison.OrdinalIgnoreCase))
                                {
                                    return null;
                                }
                            }
                            var absoluteUri = new Uri(new Uri(rootUrl), firstOrDefault.value);
                            return absoluteUri.AbsoluteUri;
                        }).Where(url => !String.IsNullOrEmpty(url))
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();

                foreach (var url in urls)
                {
                    if (CancelToken.IsCancellationRequested)
                    {
                        break;
                    }
                    if (!urlContentDictionary.ContainsKey(url))
                    {
                        var subContent = DownloadHelper.DownloadUrl(rootUrl);
                        if (subContent != null)
                        {
                            urlContentDictionary[url] = subContent.ContentString;
                            list.Add(new JsTreeDataItem
                            {
                                Id = url.ToHashGuid().ToString(),
                                Parent = rootUrl.ToHashGuid().ToString(),
                                Text = url
                            });
                            callback(list);

                            if (list.Count >= maxPages)
                            {
                                break;
                            }
                            if (list.Count < maxPages && level < maxLevel)
                            {
                                GetPageLinks(urlContentDictionary, url, list, level, maxLevel, maxPages, callback);
                            }
                        }
                        else
                        {
                            urlContentDictionary[url] = String.Empty;
                        }
                    }
                }
            }
            return list;
        }
    }

    [DataContract]
    public class JsTreeDataItem
    {
        public JsTreeDataItem()
        {
            TreeState = new JsTreeState();
        }

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "parent")]
        public string Parent { get; set; }

        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "state")]
        public JsTreeState TreeState { get; set; }
    }

    [DataContract]
    public class JsTreeState
    {
        public JsTreeState()
        {
            Opened = true;
            Selected = false;
            Disabled = false;
        }

        [DataMember(Name = "opened")]
        public bool Opened { get; set; }

        [DataMember(Name = "selected")]
        public bool Selected { get; set; }

        [DataMember(Name = "disabled")]
        public bool Disabled { get; set; }
    }
}
