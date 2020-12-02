using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.OAuth2.Weibo
{
    public class WeiboSetting : ISiteSetting
    {
        public string Name => "WeiboLoginSetting";
        public string Appid { get; set; }
        public string Secret { get; set; }
        public string RedirectUri { get; set; }
    }
}
