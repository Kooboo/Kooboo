using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.OAuth2.Facebook
{
    public class FacebookSetting : ISiteSetting, ISettingDescription
    {
        public string Appid { get; set; }
        public string Secret { get; set; }
        public string CallbackCodeName { get; set; }
        public string Fields { get; set; }

        public string Name => "FacebookLoginSetting";

        public string Group => "OAuth2";

        public string GetAlert(RenderContext renderContext)
        {
            return $@"1. You must enable the site to force SSL 
2. Please set the callback address to:
'{renderContext.WebSite.BaseUrl()}_api/oauth2callback/FacebookLogin'
";
        }
    }
}
