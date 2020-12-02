using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.OAuth2.Google
{
    public class GoogleSetting : ISiteSetting
    {
        public string Appid { get; set; }
        public string Secret { get; set; }
        public string RedirectUri { get; set; }
        public string Scope { get; set; }
        public string Name => "GoogleLoginSetting";
    }
}
