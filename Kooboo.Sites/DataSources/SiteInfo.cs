//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.Sites.DataSources
{
    public class SiteInfo : SiteDataSource
    {
        public List<MultilingualInfo> Multilingual()
        {
            var site = this.Context.WebSite;

            List<MultilingualInfo> result = new List<MultilingualInfo>();

            foreach (var item in site.Culture)
            {
                MultilingualInfo info = new MultilingualInfo {Key = item.Key, Name = item.Value};

                if (item.Key == this.Context.RenderContext.Culture)
                {
                    info.IsActive = true;
                }

                info.CurrentUrl = GetUrl(this.Context.RenderContext.Request.RelativeUrl, item.Key, site.EnableSitePath);

                result.Add(info);
            }

            return result;
        }

        private string GetUrl(string relativeurl, string culture, bool enableSitePath)
        {
            if (enableSitePath)
            {
                return "/" + culture + relativeurl;
            }
            else
            {
                Dictionary<string, string> lang = new Dictionary<string, string> {{"lang", culture}};
                return Lib.Helper.UrlHelper.AppendQueryString(relativeurl, lang);
            }
        }
    }

    public class MultilingualInfo
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public string CurrentUrl { get; set; }

        public bool IsActive { get; set; }
    }
}