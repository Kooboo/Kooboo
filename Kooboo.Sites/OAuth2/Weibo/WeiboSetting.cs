using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.OAuth2.Weibo
{
    public class WeiboSetting : ISiteSetting, ISettingDescription
    {
        public string Name => "WeiboLoginSetting";
        public string Appid { get; set; }
        public string Secret { get; set; }
        public string CallbackCodeName { get; set; }

        public string Group => "OAuth2";

        public string GetAlert(RenderContext renderContext)
        {
            return $@"
Please set the callback address to:
'{renderContext.WebSite.BaseUrl()}_api/oauth2callback/WeiboLogin'
";
        }
    }
}
