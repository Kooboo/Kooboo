//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Api.ApiResponse;
using Kooboo.Sites.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.Api.Implementation
{
    public class Redirect : IApi
    {
        public string ModelName
        {
            get
            {
                return "Redirect";
            }
        }

        public bool RequireSite
        {
            get
            {
                return false;
            }
        }

        public bool RequireUser
        {
            get
            {
                return false;
            }
        }

        public MetaResponse Login(ApiCall call)
        {
            // get the redirect url.  
            MetaResponse res = new MetaResponse();
            res.Redirect("/_admin/sites");
            return res;
        }

        public MetaResponse GoTo()
        {
            MetaResponse res = new MetaResponse();
            return res;
        }

        [Kooboo.Attributes.RequireParameters("SiteId", "PageId")]
        public MetaResponse Inline(ApiCall call)
        {
            if (call.Context.User == null)
            {
                throw new Exception("Require user login first");
            }

            var sitedb = call.Context.WebSite.SiteDb();
            var siteid = call.GetValue<Guid>("SiteId");
            var pageid = call.GetValue<Guid>("PageId");
            var isMobile = call.GetValue<bool>("isMobile");//for test using browser
            var route = sitedb.Routes.GetByObjectId(pageid);

            sitedb.Routes.GetObjectPrimaryRelativeUrl(pageid);

            string baseurl = sitedb.WebSite.BaseUrl();

            var pageurl = sitedb.Routes.GetObjectPrimaryRelativeUrl(pageid);

            string fullpageurl = Kooboo.Lib.Helper.UrlHelper.Combine(baseurl, pageurl);

            var newtoken = Kooboo.Data.Cache.AccessTokenCache.GetNewToken(call.Context.User.Id);

            string url = null;

            if (IsMobile(call)|| isMobile)
            {
                url = "/_Admin/Phone/Index";
            }
            else
            {
                url = "/_Admin/Sites/Edit";
            }

            url += "?SiteId=" + siteid.ToString() + "&pageId=" + pageid.ToString();

            url += "&accessToken=" + newtoken;

            url += "&pageUrl=" +System.Net.WebUtility.UrlEncode(fullpageurl);

            url = Lib.Helper.UrlHelper.Combine(baseurl, url);
            /// /_Admin/Sites/Edit?SiteId=55b15904-3725-6cd8-9dfe-c58c52e7861f&pageId=5b4335d3-becc-4831-97d0-57adbc065cfd&pageUrl=http%3A%2F%2Fsss.kooboo%3A82%2Fsdfsdfsd&accessToken=ycc0xnXIkEinKo_G851TwQ  
            //var url = "/"  
            MetaResponse res = new MetaResponse();
            res.Redirect(url);
            return res;
        }

        private bool IsMobile(ApiCall call)
        {
            var agent = call.Context.Request.Headers.Get("User-Agent");
            if (agent == null)
            {
                return false;
            }

            agent = agent.ToLower();

            string[] mobiles =
          new[]
              {
                    "midp", "j2me", "avant", "docomo",
                    "novarra", "palmos", "palmsource",
                    "240x320", "opwv", "chtml",
                    "pda", "windows ce", "mmp/",
                    "blackberry", "mib/", "symbian",
                    "wireless", "nokia", "hand", "mobi",
                    "phone", "cdm", "up.b", "audio",
                    "SIE-", "SEC-", "samsung", "HTC",
                    "mot-", "mitsu", "sagem", "sony"
                    , "alcatel", "lg", "eric", "vx",
                    "NEC", "philips", "mmm", "xx",
                    "panasonic", "sharp", "wap", "sch",
                    "rover", "pocket", "benq", "java",
                    "pt", "pg", "vox", "amoi",
                    "bird", "compal", "kg", "voda",
                    "sany", "kdd", "dbt", "sendo",
                    "sgh", "gradi", "jb", "dddi",
                    "moto", "iphone","ipad","ipod","android",
                    "webos","windows phone","symbianos"
              };

            foreach (var item in mobiles)
            {
                if (agent.Contains(item))
                {
                    return true;
                }
            }

            return false;

        }

    }
}
