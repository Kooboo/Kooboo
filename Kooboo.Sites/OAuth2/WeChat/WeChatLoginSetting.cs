using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.OAuth2.WeChat
{
    public class WeChatLoginSetting : ISiteSetting
    {

        public string Appid { get; set; }
        public string Secret { get; set; }
        public string AfterAuthCallbackName { get; set; }
        public string Name => "WeChatLoginSetting";
    }
}
