//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Api.ApiResponse;
using Kooboo.Sites.Extensions;

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
            res.Redirect("/_admin/");
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
                throw new Exception("Login required");
            }

            var sitedb = call.Context.WebSite.SiteDb();
            var siteid = call.GetValue<Guid>("SiteId");
            var pageid = call.GetValue<Guid>("PageId");
            sitedb.Routes.GetObjectPrimaryRelativeUrl(pageid);
            string baseurl = sitedb.WebSite.BaseUrl();
            var pageurl = sitedb.Routes.GetObjectPrimaryRelativeUrl(pageid);
            string fullpageurl = Kooboo.Lib.Helper.UrlHelper.Combine(baseurl, pageurl);
            call.Context.Request.Cookies.TryGetValue("jwt_token", out var token);
            string url = $"/_Admin/inline-design?SiteId={siteid}&pageId={pageid}&pageUrl={System.Net.WebUtility.UrlEncode(fullpageurl)}&access_token={token}";
            url = Lib.Helper.UrlHelper.Combine(baseurl, url);
            MetaResponse res = new MetaResponse();
            res.Redirect(url);
            return res;
        }
    }
}
