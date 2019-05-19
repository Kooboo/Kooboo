//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Dom;
using Kooboo.Lib.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Service
{
    /// <summary>
    /// Get all the url relation resource within the dom document. 
    /// </summary>
    public static class DomUrlService
    {

        /// <summary>
        /// get all image links
        /// </summary>
        /// <param name="dom"></param>
        /// <returns></returns>
        public static List<string> GetImages(Document dom)
        {
            var imageurls = GetImageSrcs(dom);

            List<string> imgurls = new List<string>(); 

            foreach (var item in imageurls)
            { 
                if (!string.IsNullOrEmpty(item.Value))
                {
                    if (!Kooboo.Lib.Utilities.DataUriService.isDataUri(item.Value))
                    {
                        imgurls.Add(item.Value);
                    }
                }
            }
            return imgurls;
        }

        public static List<string> GetLinks(Document dom)
        {
            List<string> urls = new List<string>();

            foreach (var item in dom.Links.item)
            {
                string itemsrc = GetLinkOrSrc(item);

                if (!string.IsNullOrEmpty(itemsrc))
                {
                    urls.Add(itemsrc);
                }
            }
            return urls;
        }


        public static string GetLinkOrSrc(Element item)
        {
            string href = null;
            string tagname = item.tagName.ToLower();

            if (tagname == "script" || tagname == "img" || tagname == "embed")
            {
                href = item.getAttribute("src");
            }
            else if (tagname == "a")
            {
                href = item.getAttribute("href");
                if (string.IsNullOrEmpty(href))
                {
                    href = item.getAttribute(ConstTALAttributes.href);
                }
            }
            else
            {
                href = item.getAttribute("href");
                if (string.IsNullOrEmpty(href))
                {
                    href = item.getAttribute("src");
                }
            }

            if (string.IsNullOrEmpty(href))
            {
                return null;
            }

            var lowerhref = href.ToLower();
            if (lowerhref == "#" || lowerhref.StartsWith("#") || lowerhref.StartsWith("javascript:"))
            {
                return null;
            }

            href = href.Replace("\r", "");
            href = href.Replace("\n", "");

            string tempwithoutBracket = href.Replace("{", "");
            tempwithoutBracket = tempwithoutBracket.Replace("}", "");
            if (Lib.Helper.UrlHelper.IsValidUrl(tempwithoutBracket))
            {
                return href;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get all external style urls
        /// </summary>
        /// <param name="dom"></param>
        /// <returns></returns>
        public static List<string> GetReferenceStyles(Document dom)
        {
            List<string> urlList = new List<string>();

            if (dom != null)
            {
                HTMLCollection styletags = dom.getElementsByTagName("link");

                foreach (var item in styletags.item)
                {
                    if (item.hasAttribute("rel") && item.getAttribute("rel").ToLower().Contains("stylesheet"))
                    {
                        string itemurl = DomUrlService.GetLinkOrSrc(item);

                        if (!string.IsNullOrEmpty(itemurl))
                        {
                            urlList.Add(itemurl);
                        }
                    }
                }
            }
            return urlList;
        }

        /// <summary>
        /// get the list of script tag that has src defined. 
        /// </summary>
        /// <param name="dom"></param>
        /// <returns></returns>
        public static List<string> GetReferenceScripts(Document dom)
        {
            List<string> urlList = new List<string>();
            if (dom != null)
            {
                HTMLCollection scripttags = dom.getElementsByTagName("script");

                foreach (var item in scripttags.item)
                {
                    if (item.hasAttribute("src"))
                    {
                        string itemurl = item.getAttribute("src");

                        if (!string.IsNullOrEmpty(itemurl))
                        {
                            urlList.Add(itemurl);
                        }
                    }
                }
            }
            return urlList;
        }

        public static string MakeUrlRelative(string Url, string BaseUrl)
        {
            if (string.IsNullOrEmpty(BaseUrl))
            {
                if (!IsExternalLink(Url))
                {
                    if (!Url.StartsWith("/"))
                    {
                        Url = "/" + Url;
                    }
                }
                return Url;
            }
            else
            {
                string newbase = BaseUrl.ToLower();

                if (!IsExternalLink(newbase))
                {
                    newbase = UrlHelper.Combine("http://www.koobootempnonexists.com", newbase);
                }

                Url = UrlHelper.Combine(newbase, Url);

                if (UrlHelper.isSameHost(Url, newbase))
                {
                    return UrlHelper.RelativePath(Url);
                }
                else
                {
                    return Url;
                }
            }
        }

        public static void MakeAllUrlRelative(List<string> UrlList, string BaseUrl)
        {
            if (string.IsNullOrEmpty(BaseUrl))
            {
                BaseUrl = "/";
            }
            int counter = UrlList.Count;
            for (int i = 0; i < counter; i++)
            {
                UrlList[i] = MakeUrlRelative(UrlList[i], BaseUrl);
            }
        }

        public static bool IsExternalLink(string link)
        {
            if (string.IsNullOrWhiteSpace(link))
            {
                return false;
            }

            string lower = link.ToLower().Trim();

            if  (lower.StartsWith("http://") || lower.StartsWith("https://"))

            {
                return true; 
            } 
            return IsSpecialUrl(lower);  
        }

        public static bool IsSpecialUrl(string url)
        {
            if (url == null)
            {
                return true;
            }

            url = url.ToLower();
            if (url.StartsWith("#") || url.StartsWith("mailto:") || url.StartsWith("javascript:") || url.StartsWith("file://") || url.StartsWith("ftp://") )
            {
                return true;
            }
            return false;
        }

         

        private static Dictionary<Kooboo.Dom.Element, string> EnsureImageNonSrcUrl(Dictionary<Kooboo.Dom.Element, string> orgResult)
        {
            // when all values are the same, possible non-src is the right value. 
            Dictionary<Element, string> newvalue = new Dictionary<Element, string>(); 

            if (orgResult.Count >= 2)
            {
                List<string> sameurls = new List<string>();

                foreach (var group in orgResult.GroupBy(o=>o.Value))
                {
                    if (group.Count()>1 && AllHasNonSrc(group.ToList()))
                    {
                        foreach (var item in group)
                        {
                           newvalue[item.Key]  = GetImageNonSrcUrl(item.Key); 
                        }
                    }
                } 
            }

            if (newvalue.Any())
            {
                foreach (var item in newvalue)
                {
                    orgResult[item.Key] = item.Value; 
                }
            }
             

            return orgResult;
        }

        private static bool AllHasNonSrc(List<KeyValuePair<Element, string>> group)
        {
            foreach (var item in group)
            {
                if (GetImageNonSrcUrl(item.Key) == null)
                {
                    return false; 
                }
            }
            return true; 
        }
     
    
        public static string GetImageNonSrcUrl(Kooboo.Dom.Element imagetag)
        {
            if (imagetag == null)
            {
                return null;
            }

            foreach (var item in imagetag.attributes)
            {
                if (item != null && item.name != null)
                {
                    string name = item.name.Trim().ToLower();
                    if (name != "src" && name.Contains("src"))
                    {
                        return item.value;
                    }
                }
            }
            return null;
        }

        public static Dictionary<Kooboo.Dom.Element, string> GetImageSrcs(Document doc)
        {
            var imagetags = doc.images.item; 

            Dictionary<Kooboo.Dom.Element, string> result = new Dictionary<Dom.Element, string>();

            foreach (var item in imagetags)
            {
                var url = item.getAttribute("src");

                if (string.IsNullOrWhiteSpace(url))
                {
                    url = GetImageNonSrcUrl(item);
                }

                if (!string.IsNullOrWhiteSpace(url))
                {
                    result.Add(item, url);
                }
            }

            return EnsureImageNonSrcUrl(result);
        }

    }
}
